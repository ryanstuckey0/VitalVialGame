using UnityEngine;

namespace ViralVial
{
    public class StateMachine : MonoBehaviour
    {
        private BaseState currentState;

        private void Start()
        {
            ChangeState(new MainMenuState());
        }

        private void Update()
        {
            UpdateState();
        }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        private void UpdateState()
        {
            if (currentState != null) currentState.UpdateState();
        }

        /// <summary>
        /// Changes the current state of this to $newState
        /// </summary>
        /// <param name="newState">The state to change the current state to.</param>
        public void ChangeState(BaseState newState)
        {
            if (currentState != null) currentState.Cleanup();
            //var tempState = currentState;

            currentState = newState;

            if (currentState != null)
            {
                currentState.StateMachine = this;
                currentState.Setup();
            }

            //if (tempState != null) tempState.Cleanup();
        }
    }
}
