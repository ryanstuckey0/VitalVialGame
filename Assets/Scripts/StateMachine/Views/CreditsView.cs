using UnityEngine;

namespace ViralVial
{
    public class CreditsView : BaseView
    {
        public GameObject credits;

        public void GoToMainMenu() 
        {
            if (credits.gameObject.activeSelf)
            {
                credits.gameObject.SetActive(false);
            }
        }
    }
}
