using System.Collections.Generic;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class MeleeController : MonoBehaviour, IWeapon
    {
        [SerializeField] private float baseDamage;
        public float Damage { get => baseDamage * damageMultiplier; }

        [SerializeField] private GameObject AttackStartPosition;
        [SerializeField] private Vector3 halfWidths;
        [SerializeField] private float attackWidth;
        [SerializeField] private WeaponUtilities.WeaponSide weaponSide;

        private string eventName;
        private float damageMultiplier = 1;

        private void Awake()
        {
            eventName = "Animator_MeleeHit" + weaponSide.ToString();
        }

        public void UseMelee()
        {
            EventManager.Instance.SubscribeToEvent(eventName, OnAnimator_MeleeHit);
        }

        public void AddDamageMultiplier(float multiplier)
        {
            damageMultiplier += multiplier;
        }

        private void OnAnimator_MeleeHit()
        {
            RaycastHit[] raycastHits = Physics.BoxCastAll(AttackStartPosition.transform.position, halfWidths, AttackStartPosition.transform.right, Quaternion.identity, attackWidth, LayerMask.GetMask(Constants.EnemyLayerName));
            foreach (var hit in raycastHits)
            {
                hit.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(Damage);
                BulletImpactSpawner.SpawnBulletImpact(hit);
            }
            EventManager.Instance.UnsubscribeFromEvent(eventName, OnAnimator_MeleeHit);
        }
    }
}
