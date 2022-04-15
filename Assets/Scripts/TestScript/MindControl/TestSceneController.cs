using UnityEngine;

namespace ViralVial.TestScript.MindControl
{
    public class TestSceneController : MonoBehaviour
    {
        public GameObject Player;

        private Vector3 playerStartingPosition;

        private void Start()
        {
            playerStartingPosition = Player.transform.position;
        }

        private void OnResetPlayer()
        {
            Player.transform.position = playerStartingPosition;
        }
    }
}
