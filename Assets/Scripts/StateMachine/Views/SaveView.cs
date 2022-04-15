using UnityEngine.Events;

namespace ViralVial
{
    public class SaveView : BaseView
    {
        public UnityAction OnPause;

        public void GoToPause()
        {
            OnPause?.Invoke();
        }
    }
}
