using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ViralVial.Weapons
{
    public class ShotgunShellRaycaster : IGunRaycaster
    {
        private IWeapon gun;

        private float spreadRange;
        private int numberOfPellets;
        private LayerMask layerMask;

        public void Init(IWeapon gun, JObject configJson, LayerMask layerMask)
        {
            this.layerMask = layerMask;
            this.gun = gun;
            spreadRange = ((float)configJson.GetValue("spreadRange")) / 2;
            numberOfPellets = (int)configJson.GetValue("numberOfPellets");
        }

        public void FireRays(Vector3 origin, Vector3 forwardDirection, float maxDistance)
        {
            for (int i = 0; i < numberOfPellets; i++)
            {
                Vector3 pelletOriginOffset = new Vector3(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange));
                Vector3 newDirection = (forwardDirection.normalized * 2 + pelletOriginOffset).normalized;
                RaycastHit hitInfo;
                if (Physics.Raycast(origin, newDirection, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
                {
                    hitInfo.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(gun.Damage);
                    BulletImpactSpawner.SpawnBulletImpact(hitInfo);

#if UNITY_EDITOR
                    Debug.DrawRay(origin, newDirection.normalized * hitInfo.distance, Color.green, 2f);
#endif
                }
#if UNITY_EDITOR
                else Debug.DrawRay(origin, newDirection.normalized * maxDistance, Color.red, 2f);
#endif
            }
        }
    }
}
