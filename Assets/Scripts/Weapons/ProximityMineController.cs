using System.Collections;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class ProximityMineController : ThrowableController, IWeapon
    {
        public float Damage { get; private set; } = 100;

        [SerializeField] private GameObject ExplosionPrefab;
        [SerializeField] private GameObject BlinkerLight;
        [SerializeField] private GameObjectLevelerUtility GameObjectLevelerUtility;
        [SerializeField] AudioSource audioSource;
        [SerializeField] private GameObject rangeIndicator;
        [SerializeField] private float range = 5;
        [SerializeField] private LayerMask layerMask;

        private bool blinkLight = false;
        private float detectionRange = 3;
        private float motionCheckInterval = 1;
        private float lightBlinkOnTime = 0.5f;
        private const float timeDelayUntilDestroy = 3;
        private IGunRaycaster mineRaycaster;

        protected override void Update()
        {
            base.Update();
            if (rangeIndicator != null) rangeIndicator.transform.position = transform.position + Vector3.up * 0.1f;
        }

        public override void Throw(Transform baseTransform)
        {
            base.Throw(baseTransform);
            rangeIndicator = Instantiate(rangeIndicator, transform.position, Quaternion.identity);
            rangeIndicator.transform.localScale = new Vector3(2 * range, rangeIndicator.transform.localScale.y, 2 * range);
            mineRaycaster = new GrenadeRaycaster();
            mineRaycaster.Init(this, null, layerMask);
            StartCoroutine(MotionCheckCoroutine());
            GameObjectLevelerUtility.StartLeveler();
        }

        private IEnumerator MotionCheckCoroutine()
        {
            StartCoroutine(LightBlinkCoroutine());
            Collider[] colliders;
            while (true)
            {
                colliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask(Constants.EnemyLayerName));
                blinkLight = true;
                if (colliders.Length > 0) break;
                yield return new WaitForSeconds(motionCheckInterval);
            }
            Explode();
        }

        private IEnumerator LightBlinkCoroutine()
        {
            while (true)
            {
                yield return new WaitUntil(() => blinkLight);
                blinkLight = false;
                BlinkerLight.SetActive(true);
                yield return new WaitForSeconds(lightBlinkOnTime);
                BlinkerLight.SetActive(false);
            }
        }

        private void Explode()
        {
            audioSource.Play();
            Destroy(gameObject, 0.5f);
            Destroy(rangeIndicator);
            Destroy(Object.Instantiate(ExplosionPrefab, transform.position, transform.rotation), timeDelayUntilDestroy);
            mineRaycaster.FireRays(transform.position, Vector3.zero, range);
        }
    }
}