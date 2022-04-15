using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Sound;
using ViralVial.Utilities;

namespace ViralVial.Player.TechTreeCode
/***
 * This class is used to read player data in .json file, and get player current ability info.
 * Display these info on TechTree UI. (skill points, ability level, ability description, ability name, etc)
 */
{
    public class TechTreeManager : MonoBehaviour
    {
        [SerializeField] private GameObjectAudioController audioController;
        [SerializeField] private Button[] equipButtons;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Image infoPanelImage;

        [Header("Switching Trees")]
        [SerializeField] private GameObject SupernaturalTechTree;
        [SerializeField] private Button SupernaturalButton;
        [SerializeField] private Button SupernaturalFirstSelectedButton;
        [SerializeField] private GameObject HumanTechTree;
        [SerializeField] private Button HumanButton;
        [SerializeField] private Button HumanFirstSelectedButton;
        [SerializeField] private bool startWithSupernaturalTechTree = true;

        [Header("Skill Icon Images")]
        [SerializeField] private SerializableDictionary<SkillIcon> SupernaturalSkillIcons;
        [SerializeField] private SerializableDictionary<SkillIcon> HumanSkillIcons;

        [Header("Text Fields")]
        public Text SkillPointsText;
        public Text DescriptionText;
        public Text SkillNameText;
        public Text CostPointsText;

        private bool supernaturalTreeSelected;
        private SkillIcon selectedSkillIcon;
        private string selectedAbilityId;
        private string selectedAbilityLevel;

        private Image selectedSkillImage;
        private AbilitySlot abilitySlot;
        private IPlayer owningPlayer;

        private string m_skillName;
        private string m_skillnumber;
        private AbilityLevel m_abilityLevel;

        void Start()
        {
            owningPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<BasePlayerController>().OwningPlayer;
            SkillPointsText.text = "Points:  " + owningPlayer.SkillPoints;

            string abilityString;
            SkillIcon tryValue;
            // iterate over unlocked abilities and change icons in tech tree
            foreach (var ability in owningPlayer.TechTree.AbilitiesProgress)
            {
                foreach (var level in ability.Value)
                {
                    if (!level.Value || level.Key == "DEBUG") continue;
                    abilityString = $"{ability.Key}.{level.Key}";
                    if (SupernaturalSkillIcons.TryGetValue(abilityString, out tryValue)) tryValue.LockImage.gameObject.SetActive(false);
                    else if (HumanSkillIcons.TryGetValue(abilityString, out tryValue)) tryValue.LockImage.gameObject.SetActive(false);
                }
            }

            if (startWithSupernaturalTechTree) OnClickSupernatural();
            else OnClickHuman();
        }

        // Utility Functions (not called by UI, Input System, or EventManager) --------------------
        private void UpdateEquipUnlockButtons(string abilityId, string upgradeId)
        {
            unlockButton.interactable = owningPlayer.TechTree.CanUnlockAbility(abilityId, upgradeId);
            for (int i = 0; i < PlayerConstants.NumberHotbarSlots; i++)
                equipButtons[i].interactable = owningPlayer.EquipmentManager.CanEquipToSlot(abilityId, i + 1);
        }

        private void UpdateInfoPanelImage(Sprite spriteToUse)
        {
            infoPanelImage.sprite = spriteToUse;
            infoPanelImage.color = Color.white;
        }

        //this is for supernatural abilities data
        private AbilityLevel ReadAllSkillData(string skillName, string skillNumber)
        {
            owningPlayer.TechTree.SupernaturalAbilitySlots.TryGetValue(skillName, out abilitySlot);
            AbilityLevel abilityLevel;
            abilitySlot.AbilityLevelsList.TryGetValue(skillNumber, out abilityLevel);
            return abilityLevel;
        }

        //this is for human abilities data
        public AbilityLevel ReadAllSkillData_Human(string skillName, string skillNumber)
        {
            owningPlayer.TechTree.HumanAbilitySlots.TryGetValue(skillName, out abilitySlot);
            AbilityLevel abilityLevel;
            abilitySlot.AbilityLevelsList.TryGetValue(skillNumber, out abilityLevel);
            return abilityLevel;
        }

        // Event Response Functions (called by UI, Input System, Event Manager) - Equip and Unlock 
        public void OnClickEquip(int slotNumber)
        {
            if (owningPlayer.EquipmentManager.AssignAbilityToSlot(selectedAbilityId, slotNumber))
                audioController.PlayAudio("OnAbilityEquip");
            else audioController.PlayAudio("OnCantEquip");
            UpdateEquipUnlockButtons(selectedAbilityId, selectedAbilityLevel);
        }

        public void OnEquipToSlot1(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            if (!equipButtons[0].interactable)
            {
                audioController.PlayAudio("OnCantEquip");
                return;
            }
            OnClickEquip(1);
        }

        public void OnEquipToSlot2(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            if (!equipButtons[1].interactable)
            {
                audioController.PlayAudio("OnCantEquip");
                return;
            }
            OnClickEquip(2);
        }

        public void OnEquipToSlot3(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            if (!equipButtons[2].interactable)
            {
                audioController.PlayAudio("OnCantEquip");
                return;
            }
            OnClickEquip(3);
        }

        public void OnEquipToSlot4(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            if (!equipButtons[3].interactable)
            {
                audioController.PlayAudio("OnCantEquip");
                return;
            }
            OnClickEquip(4);
        }

        public void OnUnlockAbility(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            if (!unlockButton.interactable)
            {
                audioController.PlayAudio("OnCantUnlock");
                return;
            }
            OnClickUnlock();
        }

        public void OnClickUnlock()
        {
            bool successful = owningPlayer.TechTree.UnlockAbility(selectedAbilityId, selectedAbilityLevel);
            if (successful)
            {
                UpdateEquipUnlockButtons(selectedAbilityId, selectedAbilityLevel);
                selectedSkillIcon.LockImage.gameObject.SetActive(false);
                audioController.PlayAudio("OnAbilityUnlock");
            }
            else audioController.PlayAudio("OnCantUnlock");
            SkillPointsText.text = "Points:  " + owningPlayer.SkillPoints;
        }

        // Event Response Functions (called by UI, Input System, Event Manager) - Switch Tech Tree 
        public void OnClickSupernatural()
        {
            supernaturalTreeSelected = true;

            SupernaturalTechTree.SetActive(true);
            SupernaturalButton.interactable = false;
            HumanTechTree.SetActive(false);
            HumanButton.interactable = true;

            SupernaturalFirstSelectedButton.Select();

            audioController.PlayAudio("OnTreeSwitch");
        }

        public void OnClickHuman()
        {
            supernaturalTreeSelected = false;

            SupernaturalTechTree.SetActive(false);
            SupernaturalButton.interactable = true;
            HumanTechTree.SetActive(true);
            HumanButton.interactable = false;

            HumanFirstSelectedButton.Select();

            audioController.PlayAudio("OnTreeSwitch");
        }

        public void SwitchToSupernatural(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            OnClickSupernatural();
        }

        public void SwitchToHuman(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            OnClickHuman();
        }

        // Event Response Functions (called by UI, Input System, Event Manager) - Click Skill Icons

        public void ClickSkillIcon(string iconId)
        {
            if (supernaturalTreeSelected) selectedSkillIcon = SupernaturalSkillIcons[iconId];
            else selectedSkillIcon = HumanSkillIcons[iconId];

            string[] tempString = iconId.Split('.');
            selectedAbilityId = tempString[0];
            selectedAbilityLevel = tempString[1];

            AbilitySlot abilitySlot = supernaturalTreeSelected
                ? owningPlayer.TechTree.SupernaturalAbilitySlots[selectedAbilityId]
                : owningPlayer.TechTree.HumanAbilitySlots[selectedAbilityId];
            SkillNameText.text = abilitySlot.AbilityName;
            DescriptionText.text = abilitySlot.AbilityLevelsList[selectedAbilityLevel].description;
            CostPointsText.text = $"Cost: + {abilitySlot.AbilityLevelsList[selectedAbilityLevel].costSkillPoints}";

            UpdateEquipUnlockButtons(selectedAbilityId, selectedAbilityLevel);
            UpdateInfoPanelImage(selectedSkillIcon.Image.sprite);
        }

        [System.Serializable]
        private class SkillIcon
        {
            public Button Button;
            public Image Image;
            public Image LockImage;
        }
    }
}
