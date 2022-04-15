using System.Collections.Generic;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Player.Pickups
{
    public class GlobalPickupSpawner : MonoBehaviour
    {
        public static GlobalPickupSpawner Instance;
        [SerializeField] private List<Pickup> pickups;
        [SerializeField] private float chanceToSpawnAnyPickup = 0.2f;
        [SerializeField] private SerializableDictionary<Pickup> UnlockablePickups;

        private float weightSum = 0;

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < pickups.Count; i++)
            {
                weightSum += pickups[i].SpawnWeight;
                pickups[i].SpawnWeight = weightSum;
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.SubscribeToEvent("PlayerUnlockedThrowable", OnPlayerUnlockedThrowable);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnsubscribeFromEvent("PlayerUnlockedThrowable", OnPlayerUnlockedThrowable);
        }

        private void OnPlayerUnlockedThrowable(Dictionary<string, object> args)
        {
            string value = (string)args["value"];
            if (!UnlockablePickups.ContainsKey(value)) return;
            AddPickupToList(UnlockablePickups[value]);
        }

        private void AddPickupToList(Pickup pickup)
        {
            weightSum += pickup.SpawnWeight;
            pickup.SpawnWeight = weightSum;
            pickups.Add(pickup);
        }

        public static void SpawnPickup(Vector3 position)
        {
            Instance.SpawnPickupLocal(position);
        }

        //spawns pickup at enemies current position at time of death
        private void SpawnPickupLocal(Vector3 position)
        {
            if (Random.Range(0f, 1f) <= chanceToSpawnAnyPickup)
            {
                float pickupToSpawn = Random.Range(0.0f, weightSum);
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (pickups[i].SpawnWeight >= pickupToSpawn)
                    {
                        Instantiate(pickups[i].prefab, position, Quaternion.identity);
                        break;
                    }
                }
            }
        }

        [System.Serializable]
        private class Pickup
        {
            public GameObject prefab;
            public float SpawnWeight;
        }
    }
}