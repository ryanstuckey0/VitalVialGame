using UnityEngine;

namespace ViralVial.Player.MonoBehaviourScript
{
    public class TurretHolographController : MonoBehaviour
    {
        [SerializeField] private GameObject TurretRedHolograph;
        [SerializeField] private GameObject TurretGreenHolograph;
        [HideInInspector] public bool CanPlace { get; private set; } = true;

        private readonly Vector3 BoxCastStartPositionOffset = new Vector3(0, 0.771f, -1.207f);
        private readonly Vector3 BoxCastHalfExtents = new Vector3(1.2f, 0.886553f, 0.025f);
        private const float BoxCastDistance = 3.290717f;

        private void Awake()
        {
            bool initialCollisionDetected = Physics.BoxCast(BoxCastStartPositionOffset, BoxCastHalfExtents, transform.forward, Quaternion.identity, BoxCastDistance);
            TurretGreenHolograph.SetActive(!initialCollisionDetected);
            TurretRedHolograph.SetActive(initialCollisionDetected);
        }

        private void OnTriggerEnter(Collider col)
        {
            CanPlace = false;
            TurretRedHolograph.SetActive(true);
            TurretGreenHolograph.SetActive(false);
        }

        private void OnTriggerStay(Collider col)
        {
            OnTriggerEnter(col);
        }

        private void OnTriggerExit(Collider col)
        {
            CanPlace = true;
            TurretRedHolograph.SetActive(false);
            TurretGreenHolograph.SetActive(true);
        }
    }
}