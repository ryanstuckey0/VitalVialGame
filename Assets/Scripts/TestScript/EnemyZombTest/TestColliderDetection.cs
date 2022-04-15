using System;
using UnityEngine;

namespace ViralVial.Script.TestScript.EnemyZombTest
{
    public class TestColliderDetection : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnter_Action;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnter_Action?.Invoke(other);
        }
    }
}
