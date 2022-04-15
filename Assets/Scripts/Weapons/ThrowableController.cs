using UnityEngine;
using ViralVial.Player;

namespace ViralVial.Weapons
{
    public abstract class ThrowableController : MonoBehaviour
    {

        public Rigidbody Rigidbody;
        public ThrowAnimationStyleCodes ThrowAnimationStyle;
        [SerializeField] protected Vector2 throwForce;

        [HideInInspector] public GameObject HandToTrack;

        protected bool hasBeenThrown = false;

        protected virtual void Update()
        {
            if (!hasBeenThrown)
            {
                transform.position = HandToTrack.transform.position;
                transform.rotation = HandToTrack.transform.rotation;
            }
        }
        public virtual void Throw(Transform baseTransform)
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.velocity = baseTransform.forward * throwForce.x + baseTransform.up * throwForce.y;
            hasBeenThrown = true;
        }
    }
}