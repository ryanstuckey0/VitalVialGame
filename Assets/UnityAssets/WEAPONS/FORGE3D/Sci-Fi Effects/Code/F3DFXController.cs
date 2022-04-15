using UnityEngine;

namespace FORGE3D
{
    public class F3DFXController : MonoBehaviour
    {
        // Singleton instance
        public static F3DFXController instance;

        // Current firing socket
        private int curSocket = 0;

        [Header("Turret setup")] public Transform[] TurretSocket; // Sockets reference
        public ParticleSystem[] ShellParticles; // Bullet shells particle system
        [Header("Vulcan")] public Transform vulcanProjectile; // Projectile prefab
        public Transform vulcanMuzzle; // Muzzle flash prefab  
        public Transform vulcanImpact; // Impact prefab
        public float vulcanOffset;

        private void Awake()
        {
            // Initialize singleton  
            instance = this;

            // Initialize bullet shells particles
            for (int i = 0; i < ShellParticles.Length; i++)
            {
                var em = ShellParticles[i].emission;
                em.enabled = false;
                ShellParticles[i].Stop();
                ShellParticles[i].gameObject.SetActive(true);
            }
        }

        // Advance to next turret socket
        private void AdvanceSocket()
        {
            curSocket++;
            if (curSocket >= TurretSocket.Length)
                curSocket = 0;
        }

        // Fire vulcan weapon
        public void Vulcan()
        {
            // Get random rotation that offset spawned projectile
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            // Spawn muzzle flash and projectile with the rotation offset at current socket position
            F3DPoolManager.instance.GetPool("GeneratedPool").Spawn(vulcanMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanProjectile,
                    TurretSocket[curSocket].position + TurretSocket[curSocket].forward,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;

            var proj = newGO.gameObject.GetComponent<F3DProjectile>();
            if (proj)
            {
                proj.SetOffset(vulcanOffset);
            }

            // Emit one bullet shell
            if (ShellParticles.Length > 0)
                ShellParticles[curSocket].Emit(1);

            // Play shot sound effect
            F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

            // Advance to next turret socket
            AdvanceSocket();
        }
    }
}