using Newtonsoft.Json.Linq;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class GrenadeRaycaster : IGunRaycaster
    {
        private IWeapon grenade;
        private LayerMask layerMask;

        public void Init(IWeapon grenade, JObject config, LayerMask layerMask)
        {
            this.layerMask = layerMask;
            this.grenade = grenade;
        }

        public void FireRays(Vector3 origin, Vector3 forwardDirection, float maxDistance)
        {
            Collider[] colliders = Physics.OverlapSphere(origin, maxDistance, layerMask);
            foreach (var collider in colliders)
            {
                collider.GetComponent<HitBox>()?.OnWeaponHit(grenade.Damage * (maxDistance - Vector3.Distance(collider.transform.position, origin)) / maxDistance);
            }
        }
    }
}