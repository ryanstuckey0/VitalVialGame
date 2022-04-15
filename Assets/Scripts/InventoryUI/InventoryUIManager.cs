using System.ComponentModel;
using System.ComponentModel.Design;
using System;
using UnityEngine;
using UnityEngine.UI;
using ViralVial.Player;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

/***
 * This class is used to read player data in .json file, and get player current weapons info.
 * Display these info on Inventory UI. (Guns, Melees, Throwables).
 */
namespace ViralVial
{
    public class InventoryUIManager : MonoBehaviour
    {
        public InventoryView InventoryView;

        [Header("Button Dictionary")]
        [SerializeField] private SerializableDictionary<Button> EquipButtons;

        [Header("Gun Sprite Fields")]
        [SerializeField] private Sprite pistol;
        [SerializeField] private Sprite smg;
        [SerializeField] private Sprite ar;
        [SerializeField] private Sprite shotgun;
        [SerializeField] private Sprite lmg;

        [Header("Melee Sprite Fields")]
        [SerializeField] private Sprite fist;
        [SerializeField] private Sprite knife;
        [SerializeField] private Sprite machete;
        [SerializeField] private Sprite katana;

        [Header("Throwable Sprite Fields")]
        [SerializeField] private Sprite rock;
        [SerializeField] private Sprite grenade;
        [SerializeField] private Sprite alarmbomb;
        [SerializeField] private Sprite proximitymine;
        [SerializeField] private Sprite turret;

        [Header("Weapon Name Fields")]
        [SerializeField] private Text[] Guns;
        [SerializeField] private Text[] Melees;
        [SerializeField] private Text[] Throwables;

        [Header("Equipped Items Column")]
        public Image EquippedGunImage;
        public Text EquippedGunAmmoCount;
        public Image EquippedMeleeImage;
        public Image EquippedThrowableImage;
        public Text EquippedThrowableCount;

        private IPlayer owningPlayer;

        private int psAmmo = 0;
        private int arAmmo = 0;
        private int shotgunAmmo = 0;
        private int lmgAmmo = 0;

        private Button outButton;
        private Button equippedGunButton;
        private Button equippedMeleeButton;
        private Button equippedThrowableButton;

        private InventoryItem selectedGun = InventoryItem.NoMatch;
        private InventoryItem selectedMelee = InventoryItem.NoMatch;
        private InventoryItem selectedThrowable = InventoryItem.NoMatch;

        private void OnEnable()
        {
            InventoryView.OnGame += FinalizeSelection;

            owningPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<BasePlayerController>().OwningPlayer;
            ReadInventoryData();

            if (EquipButtons.ContainsKey(owningPlayer.EquipmentManager.EquippedGun.ToString()))
            {
                OnClickEquipGun(owningPlayer.EquipmentManager.EquippedGun.ToString());
            }

            if (EquipButtons.ContainsKey(owningPlayer.EquipmentManager.EquippedMelee.ToString()))
            {
                OnClickEquipMelee(owningPlayer.EquipmentManager.EquippedMelee.ToString());
            }

            if (EquipButtons.ContainsKey(owningPlayer.EquipmentManager.EquippedThrowable.ToString()))
            {
                OnClickEquipThrowable(owningPlayer.EquipmentManager.EquippedThrowable.ToString());
            }
        }

        private void OnDestroy()
        {
            InventoryView.OnGame -= FinalizeSelection;
        }

        private void FinalizeSelection()
        {
            if (selectedGun != InventoryItem.NoMatch) owningPlayer.EquipmentManager.EquipGun(selectedGun);
            if (selectedMelee != InventoryItem.NoMatch) owningPlayer.EquipmentManager.EquipMelee(selectedMelee);
            if (selectedThrowable != InventoryItem.NoMatch) owningPlayer.EquipmentManager.EquipThrowable(selectedThrowable);
        }

        public void OnClickEquipGun(string gun)
        {
            selectedGun = (InventoryItem)Enum.Parse(typeof(InventoryItem), gun, true);
            equippedGunButton = EquipButtons[gun];
            EquippedGunImage.sprite = GetSpriteForItem(selectedGun);
            EquippedGunAmmoCount.text = owningPlayer.Inventory.CurrentInventory[PlayerUtilities.GetAmmoForGunType(selectedGun)].CurrentCount.ToString();
            EquippedGunImage.color = Color.white;
        }

        public void OnClickEquipMelee(string melee)
        {
            selectedMelee = (InventoryItem)Enum.Parse(typeof(InventoryItem), melee, true);
            equippedMeleeButton = EquipButtons[melee];
            EquippedMeleeImage.sprite = GetSpriteForItem(selectedMelee);
            EquippedMeleeImage.color = Color.white;
        }

        public void OnClickEquipThrowable(string throwable)
        {
            selectedThrowable = (InventoryItem)Enum.Parse(typeof(InventoryItem), throwable, true);
            equippedThrowableButton = EquipButtons[throwable];
            EquippedThrowableImage.sprite = GetSpriteForItem(selectedThrowable);
            EquippedThrowableImage.color = Color.white;
            EquippedThrowableCount.text = owningPlayer.Inventory.CurrentInventory[selectedThrowable].CurrentCount.ToString();
        }

        //loop to check all weapons are unlocked or not
        //display unlocked weapons to screen with it ammo
        public void ReadInventoryData()
        {
            foreach (var item in owningPlayer.Inventory.CurrentInventory)
            {
                if (!item.Value.Locked)
                {
                    //if item is ammo
                    if (item.Value.InventoryItem.ToString() == "PistolSMGAmmo")
                    {
                        psAmmo = item.Value.CurrentCount;
                    }
                    else if (item.Value.InventoryItem.ToString() == "ARAmmo")
                    {
                        arAmmo = item.Value.CurrentCount;
                    }
                    else if (item.Value.InventoryItem.ToString() == "ShotgunAmmo")
                    {
                        shotgunAmmo = item.Value.CurrentCount;
                    }
                    else if (item.Value.InventoryItem.ToString() == "LMGAmmo")
                    {
                        lmgAmmo = item.Value.CurrentCount;
                    }

                    // if item is gun
                    else if (item.Value.InventoryItem.ToString() == "Pistol")
                    {
                        GameObject.Find("Gun1").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Gun1").transform.GetChild(2).GetComponent<Image>().sprite = pistol;
                        GameObject.Find("Gun1").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = psAmmo.ToString();
                        Guns[0].text = "Pistol";
                        EquipButtons["Pistol"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "SMG")
                    {
                        GameObject.Find("Gun2").transform.GetChild(2).GetComponent<Image>().sprite = smg;
                        GameObject.Find("Gun2").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Gun2").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = psAmmo.ToString();
                        Guns[1].text = "SMG";
                        EquipButtons["SMG"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "AR")
                    {
                        GameObject.Find("Gun3").transform.GetChild(2).GetComponent<Image>().sprite = ar;
                        GameObject.Find("Gun3").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Gun3").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = arAmmo.ToString();
                        Guns[2].text = "AR";
                        EquipButtons["AR"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "Shotgun")
                    {
                        GameObject.Find("Gun4").transform.GetChild(2).GetComponent<Image>().sprite = shotgun;
                        GameObject.Find("Gun4").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Gun4").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = shotgunAmmo.ToString();
                        Guns[3].text = "Shotgun";
                        EquipButtons["Shotgun"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "LMG")
                    {
                        GameObject.Find("Gun5").transform.GetChild(2).GetComponent<Image>().sprite = lmg;
                        GameObject.Find("Gun5").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Gun5").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = lmgAmmo.ToString();
                        Guns[4].text = "LMG";
                        EquipButtons["LMG"].interactable = true;
                    }
                    // if item is melee
                    else if (item.Value.InventoryItem.ToString() == "Knife")
                    {
                        GameObject.Find("Melee1").transform.GetChild(1).GetComponent<Image>().sprite = knife;
                        GameObject.Find("Melee1").transform.GetChild(1).GetComponent<Image>().color = Color.white;
                        Melees[0].text = "Knife";
                        EquipButtons["Knife"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "Machete")
                    {
                        GameObject.Find("Melee2").transform.GetChild(1).GetComponent<Image>().sprite = machete;
                        GameObject.Find("Melee2").transform.GetChild(1).GetComponent<Image>().color = Color.white;
                        Melees[1].text = "Machete";
                        EquipButtons["Machete"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "Katana")
                    {
                        GameObject.Find("Melee3").transform.GetChild(1).GetComponent<Image>().sprite = katana;
                        GameObject.Find("Melee3").transform.GetChild(1).GetComponent<Image>().color = Color.white;
                        Melees[2].text = "Katana";
                        EquipButtons["Katana"].interactable = true;
                    }
                    //if item is throwable
                    else if (item.Value.InventoryItem.ToString() == "Rock")
                    {
                        GameObject.Find("Throwable1").transform.GetChild(2).GetComponent<Image>().sprite = rock;
                        GameObject.Find("Throwable1").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Throwable1").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = item.Value.CurrentCount.ToString();
                        Throwables[0].text = "Rock";
                        EquipButtons["Rock"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "AlarmBomb")
                    {
                        GameObject.Find("Throwable2").transform.GetChild(2).GetComponent<Image>().sprite = alarmbomb;
                        GameObject.Find("Throwable2").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Throwable2").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = item.Value.CurrentCount.ToString();
                        Throwables[1].text = "Alarm Bomb";
                        EquipButtons["AlarmBomb"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "Grenade")
                    {
                        GameObject.Find("Throwable3").transform.GetChild(2).GetComponent<Image>().sprite = grenade;
                        GameObject.Find("Throwable3").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Throwable3").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = item.Value.CurrentCount.ToString();
                        Throwables[2].text = "Grenade";
                        EquipButtons["Grenade"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "ProximityMine")
                    {
                        GameObject.Find("Throwable4").transform.GetChild(2).GetComponent<Image>().sprite = proximitymine;
                        GameObject.Find("Throwable4").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Throwable4").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = item.Value.CurrentCount.ToString();
                        Throwables[3].text = "Proximity Mine";
                        EquipButtons["ProximityMine"].interactable = true;
                    }
                    else if (item.Value.InventoryItem.ToString() == "Turret")
                    {
                        GameObject.Find("Throwable5").transform.GetChild(2).GetComponent<Image>().sprite = turret;
                        GameObject.Find("Throwable5").transform.GetChild(2).GetComponent<Image>().color = Color.white;
                        GameObject.Find("Throwable5").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = item.Value.CurrentCount.ToString();
                        Throwables[4].text = "Turret";
                        EquipButtons["Turret"].interactable = true;
                    }
                }
                else if (EquipButtons.ContainsKey(item.Value.InventoryItem.ToString()))
                    EquipButtons[item.Value.InventoryItem.ToString()].gameObject.SetActive(false);
            }
        }

        private Sprite GetSpriteForItem(InventoryItem item)
        {
            switch (item)
            {
                case InventoryItem.Pistol: return pistol;
                case InventoryItem.SMG: return smg;
                case InventoryItem.AR: return ar;
                case InventoryItem.Shotgun: return shotgun;
                case InventoryItem.LMG: return lmg;
                case InventoryItem.Knife: return knife;
                case InventoryItem.Machete: return machete;
                case InventoryItem.Katana: return katana;
                case InventoryItem.Rock: return rock;
                case InventoryItem.AlarmBomb: return alarmbomb;
                case InventoryItem.Grenade: return grenade;
                case InventoryItem.ProximityMine: return proximitymine;
                case InventoryItem.Turret: return turret;
                default: return null;
            }
        }
    }
}
