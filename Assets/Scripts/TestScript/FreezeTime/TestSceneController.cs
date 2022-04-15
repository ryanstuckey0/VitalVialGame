using UnityEngine;
using UnityEngine.InputSystem;

namespace ViralVial.TestScript.FreezeTime
{
    public class TestSceneController : MonoBehaviour
    {
        public GameObject Player;
        public GameObject Walls;

        private Vector3 playerStartingPosition;
        

        private void Start()
        {
            playerStartingPosition = Player.transform.position;
        }

        private void OnResetPlayer()
        {
            Player.transform.position = playerStartingPosition;
        }

        private void OnToggleWalls(){
            Walls.SetActive(!Walls.activeInHierarchy);
        }
    }
}
