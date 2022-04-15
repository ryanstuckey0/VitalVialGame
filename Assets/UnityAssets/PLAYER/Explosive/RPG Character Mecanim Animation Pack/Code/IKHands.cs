﻿using System.Collections;
using UnityEngine;
using ViralVial.Player.Animation;

namespace RPGCharacterAnims
{
    public class IKHands : MonoBehaviour
    {
        private Animator animator;
        private PlayerWeaponAnimationController playerWeaponAnimationController;
        public Transform leftHandObj;
        public Transform attachLeft;
        public bool canBeUsed;
        public bool isUsed;
        [Range(0, 1)] public float leftHandPositionWeight;
        [Range(0, 1)] public float leftHandRotationWeight;
        private Transform blendToTransform;
        private Coroutine co;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerWeaponAnimationController = GetComponentInParent<PlayerWeaponAnimationController>();
        }

        /// <summary>
        /// If there is movement and/or rotation data in the animation for the Left Hand, use IK to 
        /// set the position of the Left Hand of the character.
        /// </summary>
        private void OnAnimatorIK(int layerIndex)
        {
            if (leftHandObj)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
                if (attachLeft)
                {
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, attachLeft.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, attachLeft.rotation);
                }
            }
        }

        /// <summary>
        /// Smoothly blend IK on and off so there's no snapping into position.
        /// </summary>
        public void BlendIK(bool blendOn, float delay, float timeToBlend, int weapon)
        {
            // If using 2 handed weapon.
            if (weapon == 31 || weapon == 32 || weapon == 33 || weapon == 34)
            {
                if (blendOn) { isUsed = true; }
            }
            if (canBeUsed & isUsed)
            {
                StopAllCoroutines();
                co = StartCoroutine(_BlendIK(blendOn, delay, timeToBlend, weapon));
            }
            if (!blendOn) { isUsed = false; }
        }

        private IEnumerator _BlendIK(bool blendOn, float delay, float timeToBlend, int weapon)
        {
            GetCurrentWeaponAttachPoint(weapon);
            yield return new WaitForSeconds(delay);
            float t = 0f;
            float blendTo = 0;
            float blendFrom = 0;
            if (blendOn) { blendTo = 1; }
            else { blendFrom = 1; }
            while (t < 1)
            {
                t += Time.deltaTime / timeToBlend;
                attachLeft = blendToTransform;
                leftHandPositionWeight = Mathf.Lerp(blendFrom, blendTo, t);
                leftHandRotationWeight = Mathf.Lerp(blendFrom, blendTo, t);
                yield return null;
            }
        }

        /// <summary>
        /// Pauses IK while character uses Left Hand during an animation.
        /// </summary>
        public void SetIKPause(float pauseTime)
        {
            if (canBeUsed && isUsed)
            {
                StopAllCoroutines();
                co = StartCoroutine(_SetIKPause(pauseTime));
            }
        }

        private IEnumerator _SetIKPause(float pauseTime)
        {
            float t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / 0.1f;
                leftHandPositionWeight = Mathf.Lerp(1, 0, t);
                leftHandRotationWeight = Mathf.Lerp(1, 0, t);
                yield return null;
            }
            yield return new WaitForSeconds(pauseTime - 0.2f);
            t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / 0.1f;
                leftHandPositionWeight = Mathf.Lerp(0, 1, t);
                leftHandRotationWeight = Mathf.Lerp(0, 1, t);
                yield return null;
            }
        }

        private void GetCurrentWeaponAttachPoint(int weapon)
        {
            switch (weapon)
            {
                case 31: blendToTransform = playerWeaponAnimationController.AR.transform.GetChild(0).transform; break;
                case 32: blendToTransform = playerWeaponAnimationController.SMG.transform.GetChild(0).transform; break;
                case 33: blendToTransform = playerWeaponAnimationController.Shotgun.transform.GetChild(0).transform; break;
                case 34: blendToTransform = playerWeaponAnimationController.LMG.transform.GetChild(0).transform; break;
            }
        }
    }
}