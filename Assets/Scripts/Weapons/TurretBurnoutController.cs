using System;
using UnityEngine;
using System.Collections;

namespace ViralVial.Weapons
{
    public class TurretBurnoutController : MonoBehaviour
    {
        [SerializeField] private float burnoutStartDelay = 3;
        [SerializeField] private GameObject turretSwivel;
        [SerializeField] private float burnoutRate = 0.05f;
        [SerializeField] private ParticleSystem[] Heat;
        [SerializeField] private float destoryAtBurnAmount = 0.6f;

        private MeshRenderer[] _turretParts;
        private int _burnoutId;
        private float burnAmount = 0;

        // Use this for initialization
        private void Start()
        {
            _burnoutId = Shader.PropertyToID("_Burnout");
            _turretParts = GetComponentsInChildren<MeshRenderer>();
            Array.ForEach<ParticleSystem>(Heat, (particleSystem) => particleSystem.Play());
            StartCoroutine(BurnoutCoroutine());
        }

        public void Init(Quaternion headRotation)
        {
            turretSwivel.transform.rotation = headRotation;
        }

        private IEnumerator BurnoutCoroutine()
        {
            yield return new WaitForSeconds(burnoutStartDelay);
            while (burnAmount < destoryAtBurnAmount)
            {
                for (var i = 0; i < _turretParts.Length; i++)
                    _turretParts[i].material.SetFloat(_burnoutId, burnAmount);
                burnAmount += burnoutRate * Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}