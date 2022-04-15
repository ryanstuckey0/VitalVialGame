using System.Collections.Generic;
using UnityEngine;

namespace ViralVial.SaveSystem
{
    public class BuyableDoorController : MonoBehaviour
    {
        private static BuyableDoorController instance;
        public List<BuyableDoor> BuyableDoors;

        public void Awake()
        {
            instance = this;
        }

        public static BuyableDoorsStatus GetDoorProgress()
        {
            return instance.SaveDoorsProgress();
        }

        public void LoadDoorsProgress(BuyableDoorsStatus unlockedDoors)
        {
            for (int i = 0; i < unlockedDoors.DoorStatusList.Count; i++)
                if (unlockedDoors.DoorStatusList[i]) BuyableDoors[i].OpenDoor();
        }

        public BuyableDoorsStatus SaveDoorsProgress()
        {
            List<bool> unlockedDoors = new List<bool>();
            for (int i = 0; i < BuyableDoors.Count; i++)
                unlockedDoors.Add(BuyableDoors[i].Opened);
            return new BuyableDoorsStatus
            {
                DoorStatusList = unlockedDoors
            };
        }
    }
}
