using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Player.Pickups
{
    public class AmmoPickup : MonoBehaviour, IPickup
    {
        public string PickupName { get; } = "Ammo Refill";
        [SerializeField] private ItemPickupController ItemPickupController;
        public AudioClip pickupSound;
        public AudioSource pickupAudio;
        private SphereCollider sphereCollider;
        private MeshRenderer meshRend;

        void Start()
        {
            //display the pickup name
            TextMesh textMeshComponent = gameObject.AddComponent<TextMesh>();
            textMeshComponent.text = PickupName;
            textMeshComponent.anchor = TextAnchor.UpperCenter;
            textMeshComponent.characterSize = 0.25f;

            sphereCollider = gameObject.GetComponent<SphereCollider>();
            meshRend = gameObject.GetComponent<MeshRenderer>();

            ItemPickupController.SetPickup(this);
        }

        //activates the pickup affect
        public void PickupActivation()
        {
            //refills all player ammo to maximum
            EventManager.Instance.InvokeEvent("AddMaxAmmo");
            sphereCollider.enabled = false;
            meshRend.enabled = false;
            pickupAudio.PlayOneShot(pickupSound);
            Destroy(gameObject, pickupSound.length);
        }
    }
}
