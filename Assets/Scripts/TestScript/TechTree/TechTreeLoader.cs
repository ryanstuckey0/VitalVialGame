using UnityEngine;
using ViralVial.Player.TechTreeCode;
using ViralVial.Player.MonoBehaviourScript;
using UnityEngine.UI;

namespace ViralVial.TestScript.TechTreeTest
{
    public class TechTreeLoader : MonoBehaviour
    {
        public TechTree TechTree;
        public GameObject player;

        public Text skillPointsText;

        public BasePlayerController basePlayerController;
        void Start()
        {
            TechTree = new TechTree(player.GetComponent<BasePlayerController>().OwningPlayer);
            basePlayerController = player.GetComponent<BasePlayerController>().OwningPlayer.BasePlayerController; 
            


            Debug.Log("" + basePlayerController.SkillPoints);
            skillPointsText.text += basePlayerController.SkillPoints;
        }
    }
}
