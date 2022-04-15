using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ViralVial
{
    public class Test : MonoBehaviour
    {
        void Start()
        {
            //SaveDataToJson();
            LoadDataFromJson();
        }

        public class PeopleData
        {
            public float Health = 100f;
            public float currentMaxHealth = 100f;
            public float Exp = 0f;
            public int Level = 1;
            public int SkillPoints = 0;


            public string playerName = "p1";
            public int levelData = 3;


            public List<SkillInfo> skillInfo = new List<SkillInfo>();
        }

        public class SkillInfo
        {
            public string skillName = "skill1";
            public string skillTime = "20";
        }


        public void SaveDataToJson()
        {
            PeopleData pd = new PeopleData();
            pd.playerName = "p2";
            pd.levelData = 2;
            pd.SkillPoints = 100;

            SkillInfo sf1 = new SkillInfo();
            sf1.skillName = "s1";
            sf1.skillTime = "10";

            SkillInfo sf2 = new SkillInfo();
            sf2.skillName = "s2";
            sf2.skillTime = "20";

            SkillInfo sf3 = new SkillInfo();
            sf3.skillName = "s3";
            sf3.skillTime = "30";

            pd.skillInfo.Add(sf1);
            pd.skillInfo.Add(sf2);
            pd.skillInfo.Add(sf3);

            string json = JsonConvert.SerializeObject(pd);
            WriteIntoTxt(json);
        }


        public void WriteIntoTxt(string message)
        {
            StreamWriter writer;
            FileInfo file = new FileInfo(Application.dataPath + "/StreamingAssets/playerStatus.json");
            file.Delete();
            //Debug.Log(Application.dataPath + "/StreamingAssets/playerStatus.txt");
            if (!file.Exists)
            {
                 writer = file.CreateText();
            }
            else
            {
                Debug.Log("Error: file already exist");
                writer = file.AppendText();
            }
            writer.WriteLine(message);
            writer.Flush();
            writer.Dispose();
            writer.Close();
        }


        public void LoadDataFromJson()
        {
            string json = ReadOutTxt();
            PeopleData pd = JsonConvert.DeserializeObject<PeopleData>(json);
            Debug.Log(pd.SkillPoints);
        }

        public string ReadOutTxt()
        {
            StreamReader reader;
            reader = new StreamReader(Application.dataPath + "/StreamingAssets/playerStatus.json", Encoding.UTF8);
            string text;
            text = reader.ReadToEnd();

            reader.Dispose();
            reader.Close();
            return text;
        }

      
    }
}
