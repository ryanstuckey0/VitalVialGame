using UnityEngine;

namespace ViralVial.PCG.Terrain.Scratch
{
    public class ForestBiomeGenerator : IGenerateBiome
    {
        public void AddGrass(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            //Add grass
            terrain.terrainData.SetDetailLayer(0, 0, 0, MaskToDetailMap(mask, rand));
            terrain.Flush();
        }

        private int[,] MaskToDetailMap(Texture2D mask, System.Random rand)
        {
            int[,] map = new int[mask.width, mask.height];
            for (int i = 0; i < mask.width; i++)
            {
                for (int j = 0; j < mask.height; j++)
                {
                    if (rand.NextDouble() > ForestConfig.grassPatchDensity)
                    {
                        map[i, j] = 0;
                    }
                    else
                    {
                        map[i, j] = Mathf.RoundToInt(mask.GetPixel(i, j).maxColorComponent * ForestConfig.grassDensity * 100);
                    }
                }
            }
            return map;
        }

        public void AddHeight(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            var heightMap = new float[mask.width, mask.height];
            var seed = (float)rand.NextDouble();
            for (int x = 0; x < mask.width; x++)
            {
                for (int y = 0; y < mask.height; y++)
                {
                    var maskVal = mask.GetPixel(x, y).maxColorComponent;
                    if (maskVal > 0)
                    {
                        var perlinX = (x * 1f) / (terrain.terrainData.size.x * 1f) * ForestConfig.perlinScale * seed;
                        var perlinY = (y * 1f) / (terrain.terrainData.size.z * 1f) * ForestConfig.perlinScale * seed;
                        var perlinVal = Mathf.PerlinNoise(perlinX, perlinY);
                        heightMap[x, y] = maskVal * perlinVal * ForestConfig.heightOpacity;
                    }
                    heightMap[x, y] += terrain.terrainData.GetHeight(x, y);
                }
            }
            terrain.terrainData.SetHeights(0, 0, heightMap);
        }

        public void AddObjects(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            // Forest will have no objects
        }

        public void AddSplat(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            var data = terrain.terrainData;
            var alphaMaps = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);

            for (int x = 0; x < data.alphamapWidth; x++)
            {
                for (int y = 0; y < data.alphamapHeight; y++)
                {
                    alphaMaps[x, y, 4] = mask.GetPixel(x,y).maxColorComponent;
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
        }

        public void AddTrees(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand)
        {
            // Add trees
            for (int i = 0; i < mask.width; i++)
            {
                for (int j = 0; j < mask.height; j++)
                {
                    if (mask.GetPixel(i, j).maxColorComponent < 0.5f || rand.NextDouble() > ForestConfig.treeDensity) continue;

                    var tree = new TreeInstance();
                    tree.prototypeIndex = 0;
                    tree.position = new Vector3((j + (float) rand.NextDouble() - 0.5f) / mask.height, 0f, (i + (float) rand.NextDouble() - 0.5f) / mask.width);
                    tree.rotation = (float) rand.NextDouble() * 6;
                    tree.color = Color.white;
                    tree.lightmapColor = Color.white;
                    tree.heightScale = 1f;
                    tree.widthScale = 1f;
                    terrain.AddTreeInstance(tree);
                }
            }
            terrain.Flush();
        }
    }
}
