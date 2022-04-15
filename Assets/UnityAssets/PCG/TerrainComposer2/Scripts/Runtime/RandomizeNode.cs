using System.Collections;
using System.Collections.Generic;
using TerrainComposer2;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
public class RandomizeNode : MonoBehaviour 
{
    [Header("SEED")]
    public int addSeed = 0;

    [Header("RANDOM POSITION")]
    public bool useTerrainAreaBounds = true;
    public Vector2 posXRange = new Vector2(-512,512);
    public Vector2 posYRange = new Vector2(-64,64);
    public Vector2 posZRange = new Vector2(-512, 512);
    [Header("RANDOM ROTATION")]
    public Vector2 rotXRange = new Vector2(-5, 5);
    public Vector2 rotYRange = new Vector2(-180, 180);
    public Vector2 rotZRange = new Vector2(-5, 5);
    [Header("RANDOM SCALE")]
    public Vector2 scaleXZRange = new Vector2(0.5f, 2.5f);
    public Vector2 scaleYRange = new Vector2(0.5f, 2.5f);
    
    TC_ItemBehaviour node;
    Transform t;

    void OnValidate()
    {
        if (useTerrainAreaBounds) SetTerrainAreaBounds();
    }

    void OnEnable()
    {
        t = transform;
        node = GetComponent<TC_ItemBehaviour>();
        
        TC_Generate.AddEvent(node, Randomize);
        if (useTerrainAreaBounds) SetTerrainAreaBounds();
    }

    void SetTerrainAreaBounds()
    {
        TC_TerrainArea terrainArea = TC_Area2D.instance.currentTerrainArea;
        terrainArea.CalcTotalArea();

        Rect area = terrainArea.area;

        posXRange = new Vector2(area.xMin, area.xMax);
        posZRange = new Vector2(area.yMin, area.yMax);
    }

    void OnDisable()
    {
        TC_Generate.RemoveEvent(node, Randomize);
    }

    void Randomize()
    {
        Random.InitState((int)((TC_Settings.instance.seed * 100) + addSeed));

        t.position = GetRandomValue(posXRange, posYRange, posZRange);
        t.eulerAngles = GetRandomValue(rotXRange, rotYRange, rotZRange);
        
        float scaleXZ = GetRandomValue(scaleXZRange);
        float scaleY = GetRandomValue(scaleYRange);
        t.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
    }

    float GetRandomValue(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }

    Vector3 GetRandomValue(Vector2 rangeX, Vector2 rangeY, Vector2 rangeZ)
    {
        return new Vector3(GetRandomValue(rangeX), GetRandomValue(rangeY), GetRandomValue(rangeZ));
    }

}
