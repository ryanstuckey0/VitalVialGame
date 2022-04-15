using UnityEngine;

namespace ViralVial.Utilities
{
    /// <summary>
    /// Simulates and object falling downwards even if Rigidbody.isKinematic = true.
    /// </summary>
    public class RigidbodyIsKinematicUtility : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 50;

        private float distanceToFall;
        private float distanceFallen = 0;
        private Vector3 velocity = Vector3.zero;
        private Vector3 acceleration = Physics.gravity;
        private Vector3 startingPosition;
        private Vector3 targetNormal;

        private void Start()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, maxDistance, LayerMask.GetMask(Constants.GroundLayerName)))
            {
                startingPosition = transform.position;
                distanceToFall = hitInfo.distance;
                targetNormal = hitInfo.normal;
            }
            else Destroy(this);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            velocity += acceleration * Time.fixedDeltaTime;
            Vector3 oldPosition = transform.position;
            transform.position += velocity * Time.fixedDeltaTime;
            distanceFallen += Vector3.Distance(oldPosition, transform.position);
            if (distanceFallen >= distanceToFall)
            {
                // transform.up = targetNormal;
                transform.position = startingPosition + acceleration.normalized * distanceToFall;
                Destroy(this);
            }
        }
    }
}
