using UnityEngine;

namespace ViralVial.Player.Animation
{
    public class PlayerAnimationGameObjects : MonoBehaviour
    {
        [SerializeField] private GameObject leftHand;
        public GameObject LeftHand { get => leftHand; }


        [SerializeField] private GameObject rightHand;
        public GameObject RightHand { get => rightHand; }

        [SerializeField] private GameObject leftHandProjectileSpawnPoint;
        public GameObject LeftHandProjectileSpawnPoint { get => leftHandProjectileSpawnPoint; }
    }
}
