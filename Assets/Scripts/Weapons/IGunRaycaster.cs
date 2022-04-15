using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ViralVial.Weapons
{
    public interface IGunRaycaster
    {
        void Init(IWeapon gun, JObject configJson, LayerMask layerMask);
        void FireRays(Vector3 origin, Vector3 forwardDirection, float maxDistance);
    }
}
