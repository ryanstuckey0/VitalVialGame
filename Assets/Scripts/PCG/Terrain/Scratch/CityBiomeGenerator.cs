using UnityEngine;

namespace ViralVial.PCG.Terrain.Scratch
{
    public class CityBiomeGenerator : IGenerateBiome
    {
        public void AddGrass(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            //City should have no grass
        }

        public void AddHeight(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            // City should be flat
        }

        public void AddObjects(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            //TODO: Generate city buildings
        }

        public void AddSplat(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            var data = terrain.terrainData;
            var alphaMaps = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);

            for (int x = 0; x < data.alphamapWidth; x++)
            {
                for (int y = 0; y < data.alphamapHeight; y++)
                {
                    alphaMaps[x, y, 0] = mask.GetPixel(x, y).maxColorComponent;
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
        }

        public void AddTrees(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            // City should have no trees
        }
    }
}
