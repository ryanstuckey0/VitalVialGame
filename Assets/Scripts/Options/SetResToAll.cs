using UnityEngine;

namespace ViralVial.Options
{
    public class SetResToAll : MonoBehaviour
    {
        public void Start()
        {
            GetRes();
        }
        public void GetRes() 
        {
            Resolution res = ResolutionScreenSetting.instance.LoadResDataFromJson();
            bool isFull = ResolutionScreenSetting.instance.LoadScreenDataFromJson();
            Screen.SetResolution(res.width, res.height, isFull);
        }
    }
}
