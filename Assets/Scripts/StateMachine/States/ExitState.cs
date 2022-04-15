using UnityEngine;

namespace ViralVial {
    public class ExitState : BaseState {
        public override void Setup()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}