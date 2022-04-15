using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class ClockController : ThrowableController
    {
        public float Duration;
        public AudioSource AudioSource;

        public override void Throw(Transform baseTransform)
        {
            base.Throw(baseTransform);
            StartCoroutine(AlarmBombCoroutine());
            StartCoroutine(TimeoutCoroutine());
        }

        private IEnumerator AlarmBombCoroutine()
        {
            yield return new WaitUntil(() => Rigidbody.velocity.magnitude < 0.3f);
            EventManager.Instance.InvokeEvent("StartAlarmClock", new Dictionary<string, object> { { "target", gameObject } });
            AudioSource.Play();
            yield break;
        }

        private IEnumerator TimeoutCoroutine()
        {
            yield return new WaitForSeconds(Duration);
            EventManager.Instance.InvokeEvent("EndAlarmClock", new Dictionary<string, object> { { "target", Object.FindObjectOfType<BasePlayerController>().gameObject } });
            Destroy(gameObject);
        }
    }
}