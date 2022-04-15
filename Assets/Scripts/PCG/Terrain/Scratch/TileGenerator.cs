using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.PCG.Terrain.Scratch
{
    public class TileGenerator : ITileGenerator
    {
        private Dictionary<Biome, IGenerateBiome> generators = new Dictionary<Biome, IGenerateBiome>()
        {
            {Biome.City, new CityBiomeGenerator() },
            {Biome.Plains, new PlainsBiomeGenerator() },
            {Biome.Forest, new ForestBiomeGenerator() }
        };

        private IGenerateBiome defaultGenerator = new PlainsBiomeGenerator();

        public GameObject GenerateTile(Vector3 size, GameObject tile, int seed)
        {
            var biomeMap = new BiomeGenerator().GenerateBiomeMap(Mathf.FloorToInt(size.x), new System.Random(seed), MapConfig.MaxBiomesPerTile, MapConfig.GauranteeCityPerTile);
            var biomes = Enum.GetValues(typeof(Biome)).Cast<Biome>();

            var terrain = CreateTerrain(size, tile.transform);
            terrain.enabled = false;

            foreach (var biome in biomes)
            {
                var generator = generators.ContainsKey(biome) ? generators[biome] : defaultGenerator;
                var mask = CreateTextureFromBiomeMap(biomeMap, biome);
                var rand = new System.Random(seed);
                generator.AddHeight(mask, terrain, rand);
                generator.AddSplat(mask, terrain, rand);
                generator.AddTrees(mask, terrain, rand);
                generator.AddGrass(mask, terrain, rand);
                generator.AddObjects(mask, terrain, rand);
            }

            return tile;
        }

        private UnityEngine.Terrain CreateTerrain(Vector3 size, Transform parent)
        {
            var data = new TerrainData();
            data.size = size;
            data.heightmapResolution = Mathf.CeilToInt(Mathf.Max(size.x, size.z)) + 1;
            data.baseMapResolution = Mathf.CeilToInt(Mathf.Max(size.x, size.z));

            //Setup splats
            data.terrainLayers = Tile_Settings.instance.terrainLayers;
            data.alphamapResolution = Mathf.CeilToInt(Mathf.Max(size.x, size.z)) + 1;

            //Setup trees
            var trees = Tile_Settings.instance.trees;
            var treePrototypes = new TreePrototype[trees.Count];
            for (int i = 0; i < trees.Count; i++)
            {
                treePrototypes[i] = new TreePrototype();
                treePrototypes[i].prefab = trees[i];
            }
            data.treePrototypes = treePrototypes;

            //Setup grass
            var grass = Tile_Settings.instance.grass;
            var detailPrototypes = new DetailPrototype[grass.Count];
            for (int i = 0; i < grass.Count; i++)
            {
                detailPrototypes[i] = new DetailPrototype();
                detailPrototypes[i].prototypeTexture = grass[i];
            }
            data.detailPrototypes = detailPrototypes;
            data.SetDetailResolution (Mathf.CeilToInt(Mathf.Max(size.x, size.z)) + 1, 16);
            data.wavingGrassSpeed = MapConfig.wavingGrassSpeed;
            data.wavingGrassAmount = MapConfig.wavingGrassAmount;
            data.wavingGrassStrength = MapConfig.wavingGrassStrength;
            data.wavingGrassTint = MapConfig.wavingGrassTint;

            //Initialize height
            var heightMap = new float[data.heightmapResolution, data.heightmapResolution];
            data.SetHeights(0, 0, heightMap);
            

            var terrainGo = UnityEngine.Terrain.CreateTerrainGameObject(data);
            terrainGo.transform.parent = parent;
            terrainGo.transform.position -= new Vector3(MapConfig.TileSize.x * 2f, 0f, MapConfig.TileSize.z * 2f);
            terrainGo.layer = LayerMask.NameToLayer(Constants.GroundLayerName);

            return terrainGo.GetComponent<UnityEngine.Terrain>();
        }

        private Texture2D CreateTextureFromBiomeMap(Biome[,] map, Biome maskTo)
        {
            var tex = new Texture2D(map.GetLength(0), map.GetLength(1));
            for (int i = 0; i < tex.width; i++)
            {
                for (int j = 0; j < tex.height; j++)
                {
                    tex.SetPixel(i, j, GetSmoothedColor(map, maskTo, i,j));
                }
            }
            tex.Apply();
            return tex;
        }

        private Color GetSmoothedColor(Biome[,] map, Biome maskTo, int x, int y)
        {
            var count = 0;
            var sum = 0f;
            for (int i = 0 > x - MapConfig.BiomeBlend? 0 : x - MapConfig.BiomeBlend; i < x + MapConfig.BiomeBlend && i < map.GetLength(0); i++)
            {
                for (int j = 0 > y - MapConfig.BiomeBlend ? 0 : y - MapConfig.BiomeBlend; j < y + MapConfig.BiomeBlend && j < map.GetLength(1); j++)
                {
                    count++;
                    sum += (map[i, j] == maskTo) ? 1f : 0f;
                }
            }

            var val = sum / count;
            return new Color(val, val, val);
        }
    }
}
