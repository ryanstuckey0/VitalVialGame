using UnityEngine;

namespace ViralVial.Ability.Supernatural.MindControl
{
    public class ParticleBeamDistanceController : MonoBehaviour
    {
        [SerializeField] private GameObject mainBeam;
        private ParticleSystemRenderer particleSystemRenderer;

        private void Awake()
        {
            particleSystemRenderer = mainBeam.GetComponent<ParticleSystemRenderer>();
        }

        public void UpdateTargetLength(Vector3 targetPosition)
        {
            particleSystemRenderer.lengthScale = Vector3.Distance(transform.position, targetPosition);
        }

    }
}
