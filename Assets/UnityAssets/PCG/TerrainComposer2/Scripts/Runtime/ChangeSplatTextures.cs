using System.Collections;
using System.Collections.Generic;
using TerrainComposer2;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeSplatTextures : MonoBehaviour 
{
	public TC_TerrainArea terrainArea;
	public Texture2D[] newSplatTextures;

	public bool change;

	void Update () 
	{
		if (change)
        {
			change = false;
			ChangeSplatTexture();
        }
	}

	void ChangeSplatTexture()
    {
		TCUnityTerrain unityTerrain = terrainArea.terrains[0];

		for (int i = 0; i < newSplatTextures.Length; i++)
		{
			unityTerrain.splatPrototypes[i].texture = newSplatTextures[i];
		}

		terrainArea.ApplySplatTextures(unityTerrain);
    }
}
