using FORGE3D;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class BulletImpactSpawner : MonoBehaviour
    {
        [SerializeField] private Transform MetalBulletImpact;
        [SerializeField] private Transform WoodBulletImpact;
        [SerializeField] private Transform ZombieBulletImpace;
        [SerializeField] private Transform StoneBulletImpact;

        private static BulletImpactSpawner instance;

        private void Awake()
        {
            instance = this;
        }

        public static void SpawnBulletImpact(RaycastHit hitInfo)
        {
            instance.SpawnBulletImpactLocal(hitInfo);
        }

        private void SpawnBulletImpactLocal(RaycastHit hitInfo)
        {
            if(hitInfo.transform == null) return;
            GameObject hitGameObject = hitInfo.transform.gameObject;
            if (hitGameObject.CompareTag(Constants.EnemiesTagName))
                F3DPoolManager.MainPool.Spawn(ZombieBulletImpace, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), null);
            else if (hitGameObject.CompareTag(Constants.MetalTagName))
                F3DPoolManager.MainPool.Spawn(MetalBulletImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), null);
            else if (hitGameObject.CompareTag(Constants.WoodTagName))
                F3DPoolManager.MainPool.Spawn(WoodBulletImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), null);
            else if (hitGameObject.CompareTag(Constants.StoneTagName))
                F3DPoolManager.MainPool.Spawn(WoodBulletImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), null);
        }
    }
}
