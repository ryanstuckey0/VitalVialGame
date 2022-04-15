using System.Collections.Generic;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Player.Pickups
{
    public class AttributePickup : MonoBehaviour, IPickup
    {
        public string PickupName { get; } = "Random Attribute";
        public float rotationAngle { get; } = 0.5f;
        public Vector3 rotationAxis { get; } = new Vector3(0, 1, 0);
        [SerializeField] private ItemPickupController ItemPickupController;
        private string[] attribNames = { "movementSpeed", "gunDamageMultiplier", "gunMagazineSizeMultiplier",
            "meleeSpeedMultiplier","meleeDamageMultiplier","reloadSpeedMultiplier","postDashSpeedBoostDuration"};
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
            //selects random player attribute and upgrades
            //movementSpeed
            //gunDamageMultiplier
            //gunMagazineSizeMultiplier
            //meleeSpeedMultiplier
            //meleeDamageMultiplier
            //reloadSpeedMultiplier
            //postDashSpeedBoostDuration
            int ranAttr = Random.Range(0, attribNames.Length);
            EventManager.Instance.InvokeEvent("AddPlayerAttribute", new Dictionary<string, object> { { "attribute", attribNames[ranAttr] } });
            sphereCollider.enabled = false;
            meshRend.enabled = false;
            pickupAudio.PlayOneShot(pickupSound);
            Destroy(gameObject, pickupSound.length);
        }
    }
}
