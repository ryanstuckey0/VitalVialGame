using UnityEngine;

namespace ViralVial.Script.TestScript.EnemyZombTest
{
    public class TestMovementButch : MonoBehaviour
    {
        public AnimationInstancing.AnimationInstancing animationInstancing;

        private void Start()
        {
            animationInstancing.PlayAnimation(1);
        }

        //0, 1, 2, 3, 4, 5 animation functions
        //idle, walk, scream, run, attack, death
        //1, 2, 3, 5, 0, 4 animation indices
        //*************************************************************************
        public void AnimationZero()
        {
            //idle, 1
            //animationInstancing.PlayAnimation(1);
            animationInstancing.CrossFade(1, 0.5f);
        }

        public void AnimationOne()
        {
            //walk, 2
            //animationInstancing.PlayAnimation(1);
            animationInstancing.CrossFade(2, 0.5f);
        }

        public void AnimationTwo()
        {
            //scream, 3
            //animationInstancing.PlayAnimation(1);
            animationInstancing.CrossFade(3, 0.2f);
        }

        public void AnimationThree()
        {
            //run, 5
            //animationInstancing.PlayAnimation(1);
            animationInstancing.CrossFade(5, 0.5f);
        }

        public void AnimationFour()
        {
            //attack, 0
            //animationInstancing.PlayAnimation(0);
            animationInstancing.CrossFade(0, 0.5f);
        }

        public void AnimationFive()
        {
            //death, 4
            //animationInstancing.PlayAnimation(1);
            animationInstancing.CrossFade(4, 0.5f);
        }

        ////waits until animation has finished before transitioning
        //IEnumerator AnimationCheckAction()
        //{
        //    bool isPlaying = true;
        //    while (isPlaying)
        //    {
        //        isPlaying = animationInstancing.IsDone();
        //        yield return new WaitForSeconds(.2f);
        //    }
        //    animationInstancing.CrossFade(5, 0.2f);
        //}
    }
}
