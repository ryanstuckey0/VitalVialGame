using UnityEngine;
using ViralVial.Weapons;

namespace ViralVial.TestScript.Weapons
{
    public class DamageableGameObject : MonoBehaviour, IDamageable
    {
        public float Health = 5000;
        public float MaxHealth = 5000;
        public Gradient HealthGradient;
        private MeshRenderer meshRenderer;

        public void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material.color = HealthGradient.Evaluate(Health/MaxHealth);
        }

        public void TakeDamage(float damageAmount) {
            meshRenderer.material.color = HealthGradient.Evaluate(Health / MaxHealth);
            Health -= Health < damageAmount ? Health : damageAmount;
            if(Health <= 0) Destroy(gameObject);
        }
    }
}
