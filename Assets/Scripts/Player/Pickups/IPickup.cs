using UnityEngine;

namespace ViralVial.Player.Pickups
{
    public interface IPickup
    {
        string PickupName { get; }
        void PickupActivation();
    }
}
