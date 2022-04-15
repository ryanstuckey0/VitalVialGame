using ViralVial.Ability.Supernatural.TimeFreeze;
using UnityEngine;

namespace ViralVial.TestScript.FreezeTime
{
    public class EnemyFollowerController : MonoBehaviour, ITimeFreezable
    {
        public GameObject ObjectToFollow;
        public float Speed = 2f;

        // this is in here only for testing- we'll have to decide if we want enemies
        // to remain kinematic while frozen; currently, enemies 

        private Rigidbody rb;
        private bool timeIsFrozen = false;
        private RigidbodyConstraints storedConstraints;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (timeIsFrozen) return;
            rb.AddForce((ObjectToFollow.transform.position - transform.position).normalized * Speed);
        }

        public void Freeze()
        {
            timeIsFrozen = true;
            storedConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        public void Unfreeze()
        {
            rb.constraints = storedConstraints;
            timeIsFrozen = false;
        }
    }
}
