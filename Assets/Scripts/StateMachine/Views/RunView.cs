using UnityEngine.Events;
using UnityEngine.InputSystem;
using ViralVial.Enemy.Spawner;
using ViralVial.SaveSystem;

namespace ViralVial
{
    public class RunView : BaseView
    {
        public UnityAction OnPause;
        public UnityAction OnTechTree;
        public UnityAction OnGameWin;
        public UnityAction OnGameLoss;
        public UnityAction OnInventory;

        public WaveSpawner WaveSpawner;
        public BuyableDoorController BuyableDoorController;

        public override void Hide() { }

        public void GoToInventory()
        {
            OnInventory?.Invoke();
        }

        public void GoToInventory(InputAction.CallbackContext value)
        {
            if (!value.performed) return;
            GoToInventory();
        }

        public void GoToPause()
        {
            OnPause?.Invoke();
        }

        public void GoToPause(InputAction.CallbackContext value)
        {
            OnPause?.Invoke();
        }

        public void GoToTechTree()
        {
            OnTechTree?.Invoke();
        }
        public void GoToTechTree(InputAction.CallbackContext value)
        {
            if (!value.performed) return;
            OnTechTree?.Invoke();
        }

        public void GoToGameWin()
        {
            OnGameWin?.Invoke();
        }

        public void GoToGameLoss()
        {
            OnGameLoss?.Invoke();
        }
    }
}
