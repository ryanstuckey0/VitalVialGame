using UnityEngine;

namespace ViralVial.PCG.Terrain.Scratch
{
    public class PlainsBiomeGenerator : IGenerateBiome
    {
        public void AddGrass(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            //Add grass
            terrain.terrainData.SetDetailLayer(0, 0, 1, MaskToDetailMap(mask));
            terrain.Flush();
        }

        private int[,] MaskToDetailMap(Texture2D mask)
        {
            int[,] map = new int[mask.width, mask.height];
            for (int i = 0; i < mask.width; i++)
            {
                for (int j = 0; j < mask.height; j++)
                {
                    map[i, j] = Mathf.RoundToInt(mask.GetPixel(i, j).maxColorComponent * PlainsConfig.grassDensity * 100);
                }
            }
            return map;
        }

        public void AddHeight(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            var heightMap = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
            var seed = (float)rand.NextDouble();
            for (int x = 0; x < mask.width; x++)
            {
                for (int y = 0; y < mask.height; y++)
                {
                    var maskVal = mask.GetPixel(x, y).maxColorComponent;
                    if (maskVal > 0)
                    {
                        var perlinX = (x * 1f) / (terrain.terrainData.size.x * 1f) * PlainsConfig.perlinScale * seed;
                        var perlinY = (y * 1f) / (terrain.terrainData.size.z * 1f) * PlainsConfig.perlinScale * seed;
                        var perlinVal = Mathf.PerlinNoise(perlinX, perlinY);
                        heightMap[y, x] = maskVal * perlinVal * PlainsConfig.heightOpacity;
                    }
                    heightMap[y, x] += terrain.terrainData.GetHeight(x, y);
                }
            }
            terrain.terrainData.SetHeights(0, 0, heightMap);
        }

        public void AddObjects(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            // There are no objects in a plains biome
        }

        public void AddSplat(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            var data = terrain.terrainData;
            var alphaMaps = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);

            for (int x = 0; x < data.alphamapWidth; x++)
            {
                for (int y = 0; y < data.alphamapHeight; y++)
                {
                    alphaMaps[x, y, 3] = mask.GetPixel(x, y).maxColorComponent;
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
        }

        public void AddTrees(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            // There are no trees in a plains biome
        }
    }
}
