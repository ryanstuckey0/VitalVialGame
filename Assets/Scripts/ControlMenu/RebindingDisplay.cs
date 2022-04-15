using UnityEngine;
using UnityEngine.InputSystem;

namespace ViralVial.ControlMenu
{
    public class RebindingDisplay : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;

        public void Awake()
        {
            string rebinds = RebindingSaverLoader.LoadRebinds();
            if (!string.IsNullOrEmpty(rebinds)) _inputActions.LoadBindingOverridesFromJson(rebinds);
        }

        public void ResetBindings()
        {
            foreach (InputActionMap map in _inputActions.actionMaps)
                map.RemoveAllBindingOverrides();
            RebindingSaverLoader.DeleteRebindsFile();
        }

        public void OnDisable()
        {
            string rebinds = _inputActions.SaveBindingOverridesAsJson();
            if (string.IsNullOrEmpty(rebinds)) return;
            RebindingSaverLoader.SaveRebinds(rebinds);
        }
    }
}
