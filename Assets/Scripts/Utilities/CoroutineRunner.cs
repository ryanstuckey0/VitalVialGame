using System.Collections;
using UnityEngine;

namespace ViralVial.Utilities
{
    public class CoroutineRunner
    {
        /// <summary>
        /// Return reference coroutine if it is running, else null
        /// </summary>
        /// <value>running coroutine if running, else null</value>
        public Coroutine Coroutine { get; private set; }

        /// <summary>
        /// True if coroutine is running, else null.
        /// </summary>
        /// <value>true if coroutine is running, else null</value>
        public bool Running { get; private set; } = false;

        private IEnumerator routine;
        private MonoBehaviour host;

        /// <summary>
        /// Creates a new coroutine and uses UtilityMonoBehaviour.Instance as its host.
        /// </summary>
        public CoroutineRunner()
        {
            this.host = UtilityMonoBehaviour.Instance;
        }

        /// <summary>
        /// Creates a new coroutine with the specified host.
        /// </summary>
        /// <param name="host">MonoBehaviour that will be used to run this coroutine</param>
        public CoroutineRunner(MonoBehaviour host)
        {
            this.host = host;
        }

        /// <summary>
        /// Start this coroutine with the updated routine and return it. Returns currently running coroutine if it is already running.
        /// </summary>
        /// <param name="newRoutine">routine to use in coroutine</param>
        /// <returns>reference to new coroutine if one is not running, or reference to the currently running coroutine if one is running</returns>
        public bool Start(IEnumerator newRoutine)
        {
            if (Running) return false;
            routine = newRoutine;
            Coroutine = host.StartCoroutine(LocalCoroutine());
            return true;
        }

        /// <summary>
        /// Stop the currently running coroutine.
        /// </summary>
        public void Stop()
        {
            if (!Running) return;
            host.StopCoroutine(Coroutine);
            Running = false;
            Coroutine = null;
        }

        /// <summary>
        /// Stops the currently running coroutine and then starts it again, returning the new running coroutine.
        /// </summary>
        /// <param name="newRoutine">restart coroutine with the specified routine</param>
        /// <returns>reference to new running coroutine</returns>
        public bool Restart(IEnumerator newRoutine)
        {
            Stop();
            return Start(newRoutine);
        }

        private IEnumerator LocalCoroutine()
        {
            Running = true;
            yield return routine;
            Running = false;
        }
    }
}