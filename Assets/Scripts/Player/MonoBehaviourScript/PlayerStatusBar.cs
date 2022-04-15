using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViralVial.Player;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

namespace ViralVial.Player.MonoBehaviourScript
{
    public class PlayerStatusBar : MonoBehaviour
    {
        [SerializeField] Slider HPSlider;
        [SerializeField] Gradient gradient;
        [SerializeField] Image HPFill;
        [SerializeField] Slider ExpSlider;

        [Header("Reload Indicator")]
        [SerializeField] private GameObject ammoIndicatorGameObject;
        [SerializeField] private Slider ammoSlider;
        [SerializeField] private Text ammoText;
        private int currentMagazineCount;
        private int ammoAmountInInventory;
        private int magazineCapacity;

        [Header("Text")]
        [SerializeField] Text HPText;
        [SerializeField] Text ExpText;
        [SerializeField] Text LevelText;

        [Header("Sprites To Load")]
        [SerializeField] Sprite[] HotbarSprites;
        [SerializeField] Sprite[] GunSprites;
        [SerializeField] Sprite[] MeleeSprites;
        [SerializeField] Sprite[] ThrowableSprites;

        [Header("Image References")]
        [SerializeField] private Image[] hotbarSlotImages;
        [SerializeField] private Image _Guns;
        [SerializeField] private Image _Throwables;
        [SerializeField] private Image _Melees;
        [SerializeField] private Slider[] cooldownSliders;

        private CoroutineRunner[] cooldownCoroutines;

        private IPlayer player;

        private void Awake()
        {
            cooldownCoroutines = new CoroutineRunner[] {
                new CoroutineRunner(this),
                new CoroutineRunner(this),
                new CoroutineRunner(this),
                new CoroutineRunner(this)
            };

            EventManager.Instance.SubscribeToEventAndPending("HotbarAbilityAssign", OnHotbarAbilityAssign);
            EventManager.Instance.SubscribeToEventAndPending("UpdateHudPlayerInfo", OnUpdateHudPlayerInfo);
            EventManager.Instance.SubscribeToEventAndPending("StartHotbarSlotCooldown", OnStartHotbarSlotCooldown);
            EventManager.Instance.SubscribeToEventAndPending("UpdateGunAmmo", OnUpdateGunAmmo);
            EventManager.Instance.SubscribeToEventAndPending("UpdateInventoryAmmo", OnUpdateInventoryAmmo);

            EventManager.Instance.SubscribeToEvent("ResetCooldowns", ResetHudCooldowns);
        }

        private void OnUpdateHudPlayerInfo(Dictionary<string, object> args)
        {
            UpdateHealthBar((float)args["health"], (float)args["maxHealth"]);
            UpdateLevel((int)args["level"]);
            UpdateExpBar((float)args["experience"], (float)args["experienceToLevelUp"]);
        }

        private void OnHotbarAbilityAssign(Dictionary<string, object> args)
        {
            string slotId = (string)args["slotId"];
            int slotNumber;
            if (int.TryParse(slotId, out slotNumber))
            {
                string assignedItemId = (string)args["assignedItemId"];
                hotbarSlotImages[slotNumber - 1].sprite = GetSpriteForAbility(assignedItemId);
            }
            else
            {
                switch (slotId)
                {
                    case "G":
                        {
                            string assignedItemName = (string)args["assignedItemName"];
                            _Guns.sprite = GetSpriteForWeapon(assignedItemName);
                            _Guns.color = assignedItemName != "NoGun" ? Color.white : new Color(0, 0, 0, 0);
                            ammoIndicatorGameObject.SetActive(assignedItemName != "NoGun");
                            break;
                        }
                    case "M":
                        {
                            string assignedItemName = (string)args["assignedItemName"];
                            _Melees.sprite = GetSpriteForWeapon(assignedItemName);
                            _Melees.color = assignedItemName != "NoMelee" ? Color.white : new Color(0, 0, 0, 0);
                            break;
                        }
                    case "T":
                        {
                            string assignedItemName = (string)args["assignedItemName"];
                            _Throwables.sprite = GetSpriteForWeapon(assignedItemName);
                            _Throwables.color = assignedItemName != "NoThrowable" ? Color.white : new Color(0, 0, 0, 0);
                            break;
                        }
                    default:
                        Debug.LogWarning($"Status bar change not implemented for slotId {slotId}");
                        break;
                }
            }
        }

        public void UpdateHealthBar(float health, float maxHealth)
        {
            HPSlider.value = health / maxHealth;
            HPFill.color = gradient.Evaluate(HPSlider.normalizedValue);
            HPText.text = (int)health + "/" + (int)maxHealth;
        }

        public void UpdateExpBar(float experience, float experienceToLevelUp)
        {
            ExpSlider.value = experience / experienceToLevelUp;
            ExpText.text = (int)experience + "/" + (int)experienceToLevelUp;
        }

        public void UpdateLevel(int level)
        {
            LevelText.text = $"Lv. {level}";
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("HotbarAbilityAssign", OnHotbarAbilityAssign);
            EventManager.Instance.UnsubscribeFromEvent("UpdateHudPlayerInfo", OnUpdateHudPlayerInfo);
            EventManager.Instance.UnsubscribeFromEvent("StartHotbarSlotCooldown", OnStartHotbarSlotCooldown);
            EventManager.Instance.UnsubscribeFromEvent("UpdateGunAmmo", OnUpdateGunAmmo);
            EventManager.Instance.UnsubscribeFromEvent("UpdateInventoryAmmo", OnUpdateInventoryAmmo);

            EventManager.Instance.UnsubscribeFromEvent("ResetCooldowns", ResetHudCooldowns);
        }

        private Sprite GetSpriteForAbility(string abilityId)
        {
            switch (abilityId)
            {
                case "fire-attack": return HotbarSprites[0];
                case "health-boost": return HotbarSprites[1];
                case "blink": return HotbarSprites[2];
                case "shock-wave": return HotbarSprites[3];
                case "mind-control": return HotbarSprites[4];
                case "time-freeze": return HotbarSprites[5];
                case null: return HotbarSprites[6];
                default: return null;
            }
        }

        private Sprite GetSpriteForWeapon(string assignedItemName)
        {
            switch (assignedItemName)
            {
                case "Pistol": return GunSprites[0];
                case "SMG": return GunSprites[1];
                case "AR": return GunSprites[2];
                case "Shotgun": return GunSprites[3];
                case "LMG": return GunSprites[4];

                case "Knife": return MeleeSprites[1];
                case "Machete": return MeleeSprites[2];
                case "Katana": return MeleeSprites[3];

                case "Rock": return ThrowableSprites[0];
                case "Grenade": return ThrowableSprites[1];
                case "AlarmBomb": return ThrowableSprites[2];
                case "ProximityMine": return ThrowableSprites[3];
                case "Turret": return ThrowableSprites[4];
                case null:
                default: return null;
            }

        }

        private void ResetHudCooldowns()
        {
            for (int i = 0; i < cooldownCoroutines.Length; i++)
            {
                cooldownCoroutines[i].Stop();
                cooldownSliders[i].value = 0;
            }
        }

        private void OnStartHotbarSlotCooldown(Dictionary<string, object> args)
        {
            int slotNumber = (int)args["slotNumber"];
            cooldownCoroutines[slotNumber - 1].Start(StartCooldownCoroutine(slotNumber, (float)args["time"]));
        }

        private IEnumerator StartCooldownCoroutine(int slotNumber, float time)
        {
            Slider slider = cooldownSliders[slotNumber - 1];
            float timeRemaining = time;
            while (timeRemaining > 0)
            {
                slider.value = timeRemaining / time;
                timeRemaining -= Time.deltaTime;
                yield return null;
            }
            slider.value = 0;
        }

        private void OnUpdateGunAmmo(Dictionary<string, object> args)
        {
            currentMagazineCount = (int)args["currentCount"];
            magazineCapacity = (int)args["capacity"];
            UpdateGunAmmoGraphics();
        }

        private void OnUpdateInventoryAmmo(Dictionary<string, object> args)
        {
            ammoAmountInInventory = (int)args["ammoInInventory"];
            UpdateGunAmmoGraphics();
        }

        private void UpdateGunAmmoGraphics()
        {
            ammoText.text = $"{currentMagazineCount} / {ammoAmountInInventory}";
            ammoSlider.value = (float)currentMagazineCount / (float)magazineCapacity;
        }
    }
}
