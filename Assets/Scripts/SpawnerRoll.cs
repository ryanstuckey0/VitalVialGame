using System.Collections.Generic;
using UnityEngine;

public class SpawnerRoll : MonoBehaviour
{
    public int spawnAmount = 12;
    public float xRange = 10f, yRange = 0f, zRange = 10f;
    public GameObject spawnObject;
    public float lifeSpan = 10f;

    [HideInInspector]
    public Dictionary<GameObject, float> spawnedLife;

    void Start()
    {
        spawnedLife = new Dictionary<GameObject, float>();
        for (var i = 0; i < spawnAmount; i++) CreateNewSpawn(Random.Range(0f, lifeSpan/2f));
    }

    void FixedUpdate()
    {
        var spawns = new List<GameObject>(spawnedLife.Keys);
        foreach (var spawn in spawns)
        {
            if (!spawn.activeSelf) continue;

            spawnedLife[spawn] += Time.deltaTime;
            if (spawnedLife[spawn] > lifeSpan)
            {
                ReSpawn(spawn);
            }
            else
            {
                UpdateColor(spawn);
            }
        }
    }

    private void UpdateColor(GameObject spawn)
    {
        var materialRender = spawn.GetComponent<Renderer>();
        var newColor = materialRender.material.color;
        newColor.a = 1f - (spawnedLife[spawn] / lifeSpan);
        materialRender.material.color = newColor;
    }

    public void ReSpawn(GameObject spawn)
    {
        spawnedLife.Remove(spawn);
        Destroy(spawn);
        CreateNewSpawn(0f);
    }

    private void CreateNewSpawn(float time)
    {
        var position = new Vector3(
            Random.Range(-1 * xRange, xRange),
            Random.Range(-1 * yRange, yRange),
            Random.Range(-1 * zRange, zRange));
        spawnedLife.Add(Instantiate(spawnObject, transform.position + position, Quaternion.identity, transform), time);
    }

    public GameObject GetFirstActiveSpawn()
    {
        foreach (var ob in spawnedLife)
        {
            if (ob.Key.gameObject.activeSelf) return ob.Key;
        }

        return null;
    }
}
