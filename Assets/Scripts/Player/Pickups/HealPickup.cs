using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Player.Pickups
{
    public class HealPickup : MonoBehaviour, IPickup
    {
        public string PickupName { get; } = "Heal";
        public float rotationAngle { get; } = 0.5f;
        public Vector3 rotationAxis { get; } = new Vector3(0, 1, 0);
        [SerializeField] private ItemPickupController ItemPickupController;
        public AudioClip pickupSound;
        public AudioSource pickupAudio;
        private SphereCollider sphereCollider;
        private MeshRenderer meshRend;

        void Start()
        {
            //display the pickup name
            gameObject.AddComponent(typeof(TextMesh));
            TextMesh textMeshComponent = gameObject.GetComponent(typeof(TextMesh)) as TextMesh;
            textMeshComponent.text = PickupName;
            textMeshComponent.anchor = TextAnchor.UpperCenter;
            textMeshComponent.characterSize = 0.25f;

            sphereCollider = (SphereCollider)gameObject.GetComponent(typeof(SphereCollider));
            meshRend = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));

            ItemPickupController.SetPickup(this);
        }

        //activates the pickup affect
        public void PickupActivation()
        {
            //heal player to max health, max health is inside the base player class
            EventManager.Instance.InvokeEvent("AddHeal");
            sphereCollider.enabled = false;
            meshRend.enabled = false;
            pickupAudio.PlayOneShot(pickupSound);
            Destroy(gameObject, pickupSound.length);
        }
    }
}
