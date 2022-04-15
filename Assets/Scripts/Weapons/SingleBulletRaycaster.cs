using FORGE3D;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ViralVial.Weapons
{
    public class SingleBulletRaycaster : IGunRaycaster
    {
        private IWeapon gun;
        private LayerMask layerMask;

        public void Init(IWeapon gun, JObject configJson, LayerMask layerMask)
        {
            this.layerMask = layerMask;
            this.gun = gun;
        }

        public void FireRays(Vector3 origin, Vector3 forwardDirection, float maxDistance)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, forwardDirection, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                hitInfo.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(gun.Damage);
                BulletImpactSpawner.SpawnBulletImpact(hitInfo);

#if UNITY_EDITOR
                Debug.DrawRay(origin, forwardDirection * hitInfo.distance, Color.green, 2f);
#endif
            }
#if UNITY_EDITOR
            else Debug.DrawRay(origin, forwardDirection * maxDistance, Color.red, 2f);
#endif
        }
    }
}
