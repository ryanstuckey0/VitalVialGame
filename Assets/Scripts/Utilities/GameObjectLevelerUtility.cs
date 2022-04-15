using System.Collections;
using UnityEngine;

namespace ViralVial.Utilities
{
    public class GameObjectLevelerUtility : MonoBehaviour
    {
        [SerializeField] private Rigidbody Rigidbody;
        [SerializeField] bool levelOnStart = false;

        private void Start()
        {
            if (levelOnStart) StartCoroutine(LevelerCoroutine());
        }

        public void StartLeveler()
        {
            StartCoroutine(LevelerCoroutine());
        }

        private IEnumerator LevelerCoroutine()
        {
            yield return new WaitUntil(() => Rigidbody.velocity.magnitude > 0.01f);
            while (Rigidbody.velocity.magnitude > 0.02)
            {
                if (Rigidbody.velocity.y < 0)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1f, LayerMask.GetMask(Constants.GroundLayerName)))
                        transform.up = hitInfo.normal;
                }
                yield return new WaitForSeconds(0.3f);
            }
            Rigidbody.isKinematic = true;
            Destroy(this);
        }
    }
}
