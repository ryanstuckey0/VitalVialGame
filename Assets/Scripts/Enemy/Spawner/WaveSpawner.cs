using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.SaveSystem;
using ViralVial.Utilities;

namespace ViralVial.Enemy.Spawner
{
    public class WaveSpawner : MonoBehaviour
    {
        public GameObject Player;
        public EnemyPrefabWithWeight[] enemyPrefabsToWeights;
        public float timeBetweenWaves = 5f, timeBetweenEnemySpawns = 1f;
        public int maxEnemiesInWave = 500, increasePerWave = 2, startingWaveAmount = 5;
        public Camera mainCamera;

        private List<Vector3> spawnpoints;
        private WaitForSeconds waveBreakTimer;
        private int spawnAmount;
        private float maxEnemyWeight;
        private System.Random rand;
        private bool waveSpawning = false;
        private CoroutineRunner waveSpawningCoroutine;

        private int wavesFinished = 0;

        private Dictionary<string, object> eventDictionary = new Dictionary<string, object> { { "value", null } };

        private IPlayer iplayer;

        private WaveSpawner()
        {
            spawnpoints = new List<Vector3>();
            waveBreakTimer = new WaitForSeconds(timeBetweenWaves);
        }

        void Start()
        {
            spawnAmount = startingWaveAmount;
            maxEnemyWeight = enemyPrefabsToWeights.Sum(x => x.weight);
            rand = new System.Random();
            iplayer = GameObject.FindGameObjectWithTag("Player").GetComponent<BasePlayerController>().OwningPlayer;

            waveSpawningCoroutine = new CoroutineRunner(this);
            waveSpawningCoroutine.Start(SpawningCoroutine());
        }

        public void LoadFromSaveFile(GameProgress gameProgress)
        {
            wavesFinished = gameProgress.WavesFinished;
            spawnAmount = (spawnAmount = startingWaveAmount * (gameProgress.WavesFinished + 1)) < maxEnemiesInWave ? spawnAmount : maxEnemiesInWave;
            EventManager.Instance.InvokeEvent("InitWaveSpawner", new Dictionary<string, object> { { "wavesFinished", wavesFinished }, { "kills", gameProgress.KillCount } });
        }

        private IEnumerator SpawningCoroutine()
        {
            while (true)
            {
                yield return waveBreakTimer;
                SaveLoadSystem.AutoSave(iplayer);
                yield return SpawnWave();
                yield return new WaitUntil(() => !AnyEnemiesRemaining());
                wavesFinished++;
                eventDictionary["value"] = wavesFinished;
                EventManager.Instance.InvokeEvent("WaveFinished", eventDictionary);
                SaveLoadSystem.AutoSave(iplayer);
            }
        }

        private bool AnyEnemiesRemaining()
        {
            return GameObject.FindGameObjectWithTag("Enemy");
        }

        private IEnumerator SpawnWave()
        {

            eventDictionary["value"] = wavesFinished + 1;
            EventManager.Instance.InvokeEvent("WaveStarted", eventDictionary);

            waveSpawning = true;
            for (int i = 0; i < spawnAmount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenEnemySpawns);
            }
            if (spawnAmount < maxEnemiesInWave) spawnAmount += increasePerWave;
            waveSpawning = false;
        }

        private void SpawnEnemy()
        {
            var enemy = Instantiate(GetRandomEnemy(), GetRandomSpawnPoint(), Quaternion.identity, transform);
            (enemy.GetComponent(typeof(IEnemy)) as IEnemy).Target = Player;
            enemy.SetActive(true);
        }

        private Vector3 GetRandomSpawnPoint()
        {
            var index = rand.Next(0, spawnpoints.Count - 1);
            var indexNext = index;
            var spawnpoint = spawnpoints[index];
            while (CanCameraSeePoint(spawnpoint))
            {
                indexNext = (indexNext + 1) % spawnpoints.Count;
                spawnpoint = spawnpoints[indexNext];
                if (indexNext == index) return spawnpoint;
            }
            return spawnpoint;
        }

        private bool CanCameraSeePoint(Vector3 point)
        {
            var cameraPoint = mainCamera.WorldToViewportPoint(point);
            return (cameraPoint.x <= 1 || cameraPoint.x >= 0) && (cameraPoint.y <= 1 || cameraPoint.y >= 0) && (cameraPoint.z >= 0);
        }

        private GameObject GetRandomEnemy()
        {
            var weight = UnityEngine.Random.Range(0f, maxEnemyWeight);
            var runningWeight = 0f;
            foreach (var entry in enemyPrefabsToWeights)
            {
                runningWeight += entry.weight;
                if (weight <= runningWeight) return entry.prefab;
            }
            return enemyPrefabsToWeights.FirstOrDefault().prefab;
        }

        public void AddSpawnpoint(Vector3 location)
        {
            spawnpoints.Add(location);
        }

        [Serializable]
        public struct EnemyPrefabWithWeight
        {
            public GameObject prefab;
            public float weight;
        }
    }
}
