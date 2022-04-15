using System;
using UnityEngine;

namespace ViralVial.Enemy.Zombie
{
    public class PlayerColliderDetection : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnter_Action;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnter_Action?.Invoke(other);
        }
    }
}
