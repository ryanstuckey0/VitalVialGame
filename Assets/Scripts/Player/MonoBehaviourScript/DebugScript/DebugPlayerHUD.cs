using UnityEngine;
using UnityEngine.InputSystem;

namespace ViralVial.Player.MonoBehaviourScript.DebugScript
{
    public class DebugPlayerHUD : MonoBehaviour
    {
        [SerializeField] private BasePlayerController BasePlayerController;
        private IPlayer owningPlayer;

        private void Start()
        {
            owningPlayer = BasePlayerController.OwningPlayer;

            //PlayerStatusBar.Instance.SetHealth(Health);
            //PlayerStatusBar.Instance.SetExpAndLv(Exp, Level);
        }

        //health bar test
        public void addDamageToPlayer()
        {
            if (BasePlayerController.Health >= 10f)
            {
                owningPlayer.Health -= 10f;
            }
        }
        public void addHealthToPlayer()
        {
            if (BasePlayerController.Health <= BasePlayerController.CurrentMaxHealth - 10f)
            {
                owningPlayer.Health += 10f;
            }
        }
        //exp bar test
        public void addExpToPlayer()
        {
            if (BasePlayerController.Level <= 33)
            {
                owningPlayer.Experience += 200f;
            }
        }

        //health bar test
        private void OnReduceHP(InputValue hpInput)
        {
            addDamageToPlayer();
        }
        private void OnAddHP(InputValue hpInput)
        {
            addHealthToPlayer();
        }
        //exp bar test
        private void OnAddExp(InputValue expInput)
        {
            addExpToPlayer();
        }

    }
}
