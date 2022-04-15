using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using ViralVial.Seed;
using ViralVial.Utilities;

namespace ViralVial.PCG.Terrain
{
    public class MapGenerator : IMapGenerator
    {
        private int baseSeed;
        private ITileGenerator tileGenerator;
        private Vector3 tileSize;
        private const string terrainBaseName = "Tile", campPrefabPath = "Prefabs/SurvivorCamp";
        private ISeed seedGenerator;


        public MapGenerator(int baseSeed, ITileGenerator tileGenerator, Vector3 tileSize)
        {
            this.baseSeed = baseSeed;
            this.tileGenerator = tileGenerator;
            this.tileSize = tileSize;
            seedGenerator = new SeedGenerator();
        }

        public void DisableTile(int x, int y)
        {
            var tile = GameObject.Find(GetTileName(x, y));
            MoveTileDown(tile);
            var terrain = tile.GetComponentInChildren<UnityEngine.Terrain>();
            if (terrain != null) UtilityMonoBehaviour.StartCoroutineUtility(SetNextFrame(terrain, false));
        }

        private IEnumerator SetNextFrame(UnityEngine.Terrain terrain, bool enabled)
        {
            yield return null;
            terrain.enabled = enabled;
        }

        public void EnableTile(int x, int y)
        {
            var tile = GameObject.Find(GetTileName(x, y));
            MoveTileUp(tile);
            var terrain = tile.GetComponentInChildren<UnityEngine.Terrain>();
            if (terrain != null) UtilityMonoBehaviour.StartCoroutineUtility(SetNextFrame(terrain, true));
        }

        private void MoveTileDown(GameObject tile)
        {
            tile.transform.GetComponentInChildren<UnityEngine.Terrain>().transform.position += Vector3.down * MapConfig.TileSize.y * 5f;
        }

        private void MoveTileUp(GameObject tile)
        {
            tile.transform.GetComponentInChildren<UnityEngine.Terrain>().transform.position += Vector3.up * MapConfig.TileSize.y * 5f;
        }

        public GameObject LoadTile(int x, int y, Transform parent)
        {
            if (x == 0 && y == 0) return LoadSurvivorCamp();
            var tile = new GameObject(GetTileName(x, y));
            tile.transform.parent = parent.transform;
            var terrain = tileGenerator.GenerateTile(tileSize, tile, seedGenerator.GetSeed(baseSeed, x, y));
            MoveTileDown(terrain);
            return terrain;
        }

        public void UnloadTile(int x, int y)
        {
            var tile = GameObject.Find(GetTileName(x, y));
            GameObject.Destroy(tile);
        }

        private GameObject LoadSurvivorCamp()
        {
            var tile = GameObject.Instantiate(Resources.Load(campPrefabPath) as GameObject);
            tile.name = GetTileName(0, 0);
            return tile;
        }

        private string GetTileName(int x, int y)
        {
            return terrainBaseName + "_" + x + "_" + y;
        }

        public Task<GameObject> LoadTileAsync(int x, int y, Transform parent)
        {
            return Task.FromResult(LoadTile(x, y, parent));
        }

        public Task UnloadTileAsync(int x, int y)
        {
            UnloadTile(x, y);
            return Task.CompletedTask;
        }
    }
}
