using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViralVial.Script.Camera
{
    //this camera will rotate the player camera based on player rotation
    public class DirectedCameraController : MonoBehaviour
    {
        public GameObject player;
        private Vector3 startingAngle;
        private float amplitude;
        private float startingHeight;
        private Vector3 offset;

        private void Start()
        {
            //offset = transform.position - player.transform.position;
            //starting transform rotation of camera
            startingAngle = transform.rotation.eulerAngles;
            //starting z value for camera transform position
            amplitude = transform.position.z;
            //starting height for camera transform position
            startingHeight = transform.position.y;
        }

        void Update()
        {
            //transform.position = player.transform.position + offset;
            //get new transform offset
            Vector3 offsetChange = player.transform.position;

            //update camera angle (same as player angle + starting values)
            Vector3 currentAngle = new Vector3(0, player.transform.eulerAngles.y, 0) + startingAngle;

            transform.eulerAngles = currentAngle;

            //calculate x and z position for camera, need to convert angle to radians
            //y value of camera only changes on player transitioning over terrain like hills
            float xCameraPos = Mathf.Sin(player.transform.eulerAngles.y * Mathf.PI / 180);
            float zCameraPos = Mathf.Cos(player.transform.eulerAngles.y * Mathf.PI / 180);

            //multiply by amplitude from model
            xCameraPos *= amplitude;
            zCameraPos *= amplitude;

            Vector3 newPosition = new Vector3(xCameraPos, startingHeight + player.transform.position.y, zCameraPos);

            transform.position = newPosition + offsetChange;

        }
    }
}
