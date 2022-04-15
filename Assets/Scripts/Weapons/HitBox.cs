using UnityEngine;

namespace ViralVial.Weapons
{
    /// <summary>
    /// This script is used to pass damage info onto an entity when it gets hit with a bullet or melee weapon.
    /// </summary>
    public class HitBox : MonoBehaviour
    {

        [Tooltip("Drag a script here that inherits ViralVial.Weapons.IDamageable.")]
        [SerializeField] private MonoBehaviour damageableScript;
        private IDamageable damageable;

        private void Awake()
        {
            damageable = (IDamageable)damageableScript;
        }

        public void OnWeaponHit(float damageAmount)
        {
            damageable.TakeDamage(damageAmount);
        }
    }
}
