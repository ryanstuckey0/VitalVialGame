using UnityEngine;
using UnityEngine.InputSystem;

namespace ViralVial.Script.TestScript.EnemyZombTest
{
    public class TestMovementPlayer : MonoBehaviour
    {
        public float speed = 0;

        private Rigidbody rb;

        private float movementX;
        private float movementY;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnMove(InputValue movementValue)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();

            movementX = movementVector.x;
            movementY = movementVector.y;
        }

        private void FixedUpdate()
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);

            transform.position += movement * speed;
        }

    }

}