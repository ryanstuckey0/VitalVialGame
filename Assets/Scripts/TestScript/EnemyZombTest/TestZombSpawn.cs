using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZombSpawn : MonoBehaviour
{
    public int SpawnZombs;
    public GameObject butchZombPrefab;
    public GameObject oldZombPrefab;
    public GameObject grayZombPrefab;

    private void Start()
    {
        Vector3 SpawnLocation = Random.insideUnitSphere * 20;
        SpawnLocation.y = 0;
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        for (int i = 0; i < SpawnZombs; i++)
        {
            GameObject zombie1 = Instantiate(butchZombPrefab, SpawnLocation, rot);
            SpawnLocation = Random.insideUnitSphere * 20;
            SpawnLocation.y = 0;

        }
    }
}
