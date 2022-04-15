using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViralVial
{
    public class NavObstacleDoor : MonoBehaviour
    {
        public GameObject DoorOne;
        public GameObject DoorTwo;

        private const float moveDistance = 0.5f;
        private bool doorOneOpen = false;
        private bool doorTwoOpen = false;

        private void Start()
        {
            if (doorOneOpen) DoorOne.transform.Translate(0, moveDistance, 0);
            if (doorTwoOpen) DoorTwo.transform.Translate(0, moveDistance, 0);
        }

        private void OnDoorToggleOne()
        {
            doorOneOpen = !doorOneOpen;
            DoorOne.transform.Translate(0, doorOneOpen ? moveDistance : -moveDistance, 0);
        }

        private void OnDoorToggleTwo()
        {
            doorTwoOpen = !doorTwoOpen;
            DoorTwo.transform.Translate(0, doorTwoOpen ? moveDistance : -moveDistance, 0);
        }
    }
}
