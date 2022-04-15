using System;
using System.Collections;
using UnityEngine;

namespace ViralVial.Utilities
{
    //used to start coroutines even if you're outside of a MonoBehaviour
    public class UtilityMonoBehaviour : MonoBehaviour
    {
        public static UtilityMonoBehaviour Instance;

        private void Awake()
        {
            Instance = this;
        }

        public static Coroutine StartCoroutineUtility(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        public static void StopCoroutineUtility(Coroutine routine)
        {
            Instance.StopCoroutine(routine);
        }
    }
}
