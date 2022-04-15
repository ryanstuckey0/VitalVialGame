using UnityEngine;
using UnityEngine.EventSystems;

namespace ViralVial
{
    public class BaseView : MonoBehaviour
    {
        public EventSystem EventSystem;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
