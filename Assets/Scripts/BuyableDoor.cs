using System.Collections;
using System.Linq;
using UnityEngine;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

namespace ViralVial
{
    public class BuyableDoor : MonoBehaviour
    {
        public bool Opened = false;
        public int levelCost = 1;
        public GameObject door;
        public Renderer prompt;
        public TextMesh textMesh;
        public float transparencySpeed = 0.0025f;

        private Collider[] colliders;
        private Renderer[] renderers;
        private bool isOpening = false;
        private BasePlayerController player;

        private CoroutineRunner fadeOutCoroutine;

        private void Start()
        {
            colliders = door.GetComponentsInChildren<Collider>();
            renderers = door.GetComponentsInChildren<Renderer>();
            prompt.enabled = false;
            textMesh.text = "Buy for [" + levelCost + "] skill point(s).";
            player = FindObjectOfType<BasePlayerController>();

            fadeOutCoroutine = new CoroutineRunner(this);

            if (Opened) OpenDoor();
        }

        public void OpenDoor()
        {
            isOpening = true;
            foreach (var collider in colliders)
            {
                if (collider is MeshCollider) collider.GetComponent<BSPTree>().enabled = false;
                collider.enabled = false;
            }
            fadeOutCoroutine.Start(FadeOutDoorAndDestroy());
            Opened = true;
        }

        public void OnInteract()
        {
            if (prompt.enabled && !isOpening && player.SkillPoints >= levelCost)
            {
                player.SkillPoints -= levelCost;
                OpenDoor();
            }
        }

        private IEnumerator FadeOutDoorAndDestroy()
        {
            while (renderers.All(r => r.material.color.a > 0))
            {
                foreach (var renderer in renderers)
                {
                    var color = renderer.material.color;
                    renderer.material.color = new Color(color.r, color.g, color.b, color.a - transparencySpeed);
                }
                yield return null;
            }
            Destroy(door);
            if (this) Destroy(this);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                prompt.enabled = true;
                EventManager.Instance.SubscribeToEvent("InteractPerformed", OnInteract);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                prompt.enabled = false;
                EventManager.Instance.UnsubscribeFromEvent("InteractPerformed", OnInteract);
            }
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("InteractPerformed", OnInteract);
        }
    }
}
