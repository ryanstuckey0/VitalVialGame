using ViralVial.Ability.Supernatural.MindControl;
using UnityEngine;

namespace ViralVial.TestScript.MindControl
{
    public class EnemyFollowerController : MonoBehaviour, IMindControllable
    {
        float IMindControllable.Health { get; set; }
        float IMindControllable.Damage { get; set; }

        [SerializeField]
        private GameObject target;
        public GameObject Target { get => target; set => target = value; }
        public float MaxSpeed = 2f;
        public float Force = 20f;

        private Rigidbody rb;
        private RigidbodyConstraints storedConstraints;

        void Start()
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (Target == null) return;
            rb.AddForce((Target.transform.position - transform.position).normalized * Force);
            if (rb.velocity.magnitude > MaxSpeed) rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
        public void StartMindControl() { }

        public void ForceDeath() { Destroy(gameObject); }

        public void StopMindControl()
        {
        }
    }
}
