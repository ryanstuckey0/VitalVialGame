using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ViralVial
{
    public class AutoScroller : MonoBehaviour
    {
        public ScrollRect ScrollRect;
        public float changeOnSelection;
        public GameObject firstSelected;

        private GameObject previousSelected;

        public void Awake()
        {
            previousSelected = firstSelected;
        }

        public void Scroll(Vector2 value)
        {
            Debug.Log(value);
        }

        public void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null || previousSelected == EventSystem.current.currentSelectedGameObject) return;
            float amountToMove = EventSystem.current.currentSelectedGameObject.transform.position.y > previousSelected.transform.position.y ? -changeOnSelection : changeOnSelection;
            ScrollRect.verticalNormalizedPosition -= amountToMove;
            previousSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
