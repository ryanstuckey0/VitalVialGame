using System.Collections;
using UnityEngine;

namespace ViralVial.Weapons
{
    public class GrenadeController : ThrowableController, IWeapon
    {
        public float Damage { get; private set; } = 100;

        [SerializeField] private GameObject ExplosionPrefab;
        [SerializeField] private float timeUntilExplosion = 2;
        [SerializeField] private float range = 5;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject rangeIndicator;
        [SerializeField] private LayerMask layerMask;

        private const float timeDelayUntilDestroy = 3;
        private IGunRaycaster grenadeRaycaster;

        public override void Throw(Transform baseTransform)
        {
            base.Throw(baseTransform);
            rangeIndicator = Instantiate(rangeIndicator, transform.position, Quaternion.identity);
            rangeIndicator.transform.localScale = new Vector3(2 * range, rangeIndicator.transform.localScale.y, 2 * range);
            grenadeRaycaster = new GrenadeRaycaster();
            grenadeRaycaster.Init(this, null, layerMask);
            StartCoroutine(PullPinCoroutine());
        }

        protected override void Update()
        {
            base.Update();
            if (rangeIndicator != null) rangeIndicator.transform.position = transform.position + Vector3.up * 0.1f;
        }

        private IEnumerator PullPinCoroutine()
        {
            yield return new WaitForSeconds(timeUntilExplosion);
            Explode();
        }

        private void Explode()
        {
            grenadeRaycaster.FireRays(transform.position, Vector3.zero, range);
            audioSource.Play();
            Destroy(gameObject, 0.5f);
            Destroy(rangeIndicator);
            Destroy(Object.Instantiate(ExplosionPrefab, transform.position, transform.rotation), timeDelayUntilDestroy);
        }
    }
}