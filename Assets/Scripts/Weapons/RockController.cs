using UnityEngine;
using ViralVial.Player.Pickups;

namespace ViralVial.Weapons
{
    public class RockController : ThrowableController
    {
        [SerializeField] private ItemPickupController itemPickupController;
        [SerializeField] private Collider rockCollider;

        public override void Throw(Transform baseTransform)
        {
            base.Throw(baseTransform);
            rockCollider.enabled = true;
            itemPickupController.CanBePickedUp = true;
        }
    }
}
