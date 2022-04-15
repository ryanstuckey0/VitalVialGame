using UnityEngine;
using UnityEngine.InputSystem;

namespace ViralVial.TestScript.MindControl
{
    public class PlayerMovementController : MonoBehaviour
    {
        public float MovementForce = 20f;
        public float MaxSpeed = 5f;

        private Rigidbody rb;
        private Vector2 movementVector = Vector2.zero;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void OnMove(InputValue movementValue)
        {
            movementVector = movementValue.Get<Vector2>();
        }

        void FixedUpdate()
        {
            rb.AddForce(new Vector3(movementVector.x, 0.0f, movementVector.y).normalized * MovementForce);
            if(rb.velocity.magnitude > MaxSpeed) rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
    }
}
