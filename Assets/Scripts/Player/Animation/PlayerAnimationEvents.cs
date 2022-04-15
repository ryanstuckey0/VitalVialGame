using UnityEngine;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

namespace ViralVial.Player.Animation
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        public bool Animating { get; set; } = false;
        [SerializeField] private BasePlayerController BasePlayerController;

        private void SpawnShockWaveAnimation()
        {
            EventManager.Instance.InvokeEvent("SpawnShockWaveAnimation");
        }

        private void ActivateBlinkMotion()
        {
            EventManager.Instance.InvokeEvent("ActivateBlinkMotion");
        }

        private void ActivateTimeFreezeAction()
        {
            EventManager.Instance.InvokeEvent("ActivateTimeFreezeAction");
        }

        private void ReadyToFire()
        {
            EventManager.Instance.InvokeEvent("Animator_ReadyToFire");
        }

        private void NotReadyToFire()
        {
            EventManager.Instance.InvokeEvent("Animator_NotReadyToFire");
        }

        private void ReleaseThrowable()
        {
            EventManager.Instance.InvokeEvent("Animator_ReleaseThrowable");
        }

        private void FinishedMelee()
        {
            EventManager.Instance.InvokeEvent("Animator_FinishedMelee");
        }

        private void MeleeHit(int side) // 0 = left, 1 = right
        {
            EventManager.Instance.InvokeEvent("Animator_MeleeHit" + (side == 0 ? "Left" : "Right"));
        }

        private void FinishedReload()
        {
            EventManager.Instance.InvokeEvent("Animator_FinishedReload");
        }

        private void Reload()
        {
            EventManager.Instance.InvokeEvent("Animator_Reload");
        }

        private void CockGun()
        {
            EventManager.Instance.InvokeEvent("Animator_CockGun");
        }

        private void FinishedDash()
        {
            EventManager.Instance.InvokeEvent("Animator_FinishedDash");
        }

        public void FootR()
        {
            BasePlayerController.PlayerAudioController.PlayAudio($"OnFootstep{Random.Range(1, 11)}");
        }

        public void FootL()
        {
            BasePlayerController.PlayerAudioController.PlayAudio($"OnFootstep{Random.Range(1, 11)}");
        }

        private void StartedAnimation()
        {
            Animating = true;
        }

        private void FinishedAnimation()
        {
            Animating = false;
        }
    }
}
