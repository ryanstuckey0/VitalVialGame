using UnityEngine;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

namespace ViralVial.Player.Pickups
{
    public class ItemPickupController : MonoBehaviour
    {
        public bool CanBePickedUp = true;
        [SerializeField] private InventoryItem itemType;
        [SerializeField] private IPickup pickup;
        [SerializeField] private int NumberToAdd = 1;

        private void OnCollisionEnter(Collision collision)
        {
            OnPickup(collision.collider);
        }

        private void OnTriggerEnter(Collider collision)
        {
            OnPickup(collision);
        }

        private void OnPickup(Collider collider)
        {
            if (!CanBePickedUp) return;
            if (Functions.LayerMaskIncludes(LayerMask.GetMask(Constants.PlayerLayerName), collider.gameObject.layer))
            {
                if (InventoryItem.Pickup == itemType)
                    pickup.PickupActivation();
                else
                {
                    collider.gameObject.GetComponent<BasePlayerController>().OwningPlayer.Inventory.AddToInventory(itemType, NumberToAdd);
                    Destroy(gameObject);
                }
            }
        }

        //spawning pickup references the pickup
        public void SetPickup(IPickup selectPickup)
        {
            pickup = selectPickup;
        }
    }
}