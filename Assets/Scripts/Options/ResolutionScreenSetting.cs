using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ViralVial.Options
{
    public class ResolutionScreenSetting : MonoBehaviour
    {
        public static ResolutionScreenSetting instance;
        public class ResulutionScreenData
        {
            public Resolution ResolutionValue;
            public bool isFullScreen;
            public int resIndex;
        }
        void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public Resolution LoadResDataFromJson()
        {
            string json = ReadOutTxt();
            if (json == null) return Screen.currentResolution;
            ResulutionScreenData m_ResulutionScreenData = JsonConvert.DeserializeObject<ResulutionScreenData>(json);
            return m_ResulutionScreenData.ResolutionValue;
        }

        public bool LoadScreenDataFromJson()
        {
            string json = ReadOutTxt();
            if (json == null) return true;
            ResulutionScreenData m_ResulutionScreenData = JsonConvert.DeserializeObject<ResulutionScreenData>(json);
            return m_ResulutionScreenData.isFullScreen;
        }
        public int LoadResIndexFromJson()
        {
            string json = ReadOutTxt();
            ResulutionScreenData m_ResulutionScreenData = JsonConvert.DeserializeObject<ResulutionScreenData>(json);
            return m_ResulutionScreenData.resIndex;
        }


        public string ReadOutTxt()
        {
            StreamReader reader;
            if (!File.Exists($"{Application.persistentDataPath}/ResolutionSetting.json")) return null;
            reader = new StreamReader($"{Application.persistentDataPath}/ResolutionSetting.json", Encoding.UTF8);
            string text;
            text = reader.ReadToEnd();
            reader.Dispose();
            reader.Close();
            return text;
        }
    }
}
