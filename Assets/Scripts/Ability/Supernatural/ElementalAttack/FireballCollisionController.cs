using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.ElementalAttack
{
    public class FireballCollisionController : MonoBehaviour
    {
        private float burnTime = 0f;
        private float damageInterval = 0f;
        private float damage = 0f;

        // Spreading Fire
        private bool fireSpreads = false;
        private float spreadRange;
        private float spreadChance;

        public void Init(float burnTime, float damageInterval, float damage, bool fireSpreads = false, float spreadRange = 0, float spreadChance = 0)
        {
            this.burnTime = burnTime;
            this.damageInterval = damageInterval;
            this.damage = damage;
            this.fireSpreads = fireSpreads;
            this.spreadRange = spreadRange;
            this.spreadChance = spreadChance;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Functions.LayerMaskIncludes(LayerMask.GetMask(Constants.EnemyLayerName), collision.gameObject.layer))
            {
                collision.gameObject.GetComponent<AbilitiesReactionController>().ApplyFireEffect(burnTime, damage, damageInterval, fireSpreads, spreadRange, spreadChance);
            }
        }
    }
}
