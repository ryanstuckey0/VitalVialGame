using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViralVial.PCG.Terrain.Scratch;
using ViralVial.Utilities;

namespace ViralVial.PCG.Terrain
{
    public class MapTraversal
    {
        private MapGenerator mapGenerator;
        private int x, y;

        private Transform parent;

        private List<Tile> tiles;

        protected internal class Tile
        {
            private int x, y;
            private TileStatus status, currentState;

            protected internal Tile(int x, int y)
            {
                this.x = x;
                this.y = y;
                status = TileStatus.ToLoad;
                currentState = TileStatus.Unloaded;
            }

            protected internal TileStatus GetStatus() { return status; }

            protected internal (int, int) GetLocation() { return (x, y); }

            protected internal async void Unload(MapGenerator mapGenerator)
            {
                await mapGenerator.UnloadTileAsync(x, y);
                status = TileStatus.Unloaded;
                currentState = TileStatus.Unloaded;
            }

            protected internal async void Load(MapGenerator mapGenerator, Transform parent)
            {
                await mapGenerator.LoadTileAsync(x, y, parent);
                status = TileStatus.Loaded;
                currentState = TileStatus.Loaded;
            }

            protected internal void MarkForUnload()
            {
                if (status != TileStatus.Unloaded) status = currentState == TileStatus.Unloaded ? TileStatus.Unloaded : TileStatus.ToUnload;
            }

            protected internal void MarkForLoad()
            {
                if (status != TileStatus.Loaded) status = currentState == TileStatus.Loaded ? TileStatus.Loaded : TileStatus.ToLoad;
            }

            protected internal enum TileStatus
            {
                Loaded,
                Unloaded,
                ToUnload,
                ToLoad
            }
        }

        public MapTraversal(int seed, int x, int y, Transform parent)
        {
            mapGenerator = new MapGenerator(seed, new TileGenerator(), MapConfig.TileSize);
            this.parent = parent;

            tiles = new List<Tile>();
            UtilityMonoBehaviour.StartCoroutineUtility(LoadTiles());
            UtilityMonoBehaviour.StartCoroutineUtility(UnloadTiles());

            GoToTile(x, y, true);
        }

        public IEnumerator LoadTiles()
        {
            while (true)
            {
                var tile = tiles.FirstOrDefault(t => t.GetStatus() == Tile.TileStatus.ToLoad);
                if (tile != null) tile.Load(mapGenerator, parent);
                yield return null;
            }
        }

        public IEnumerator UnloadTiles()
        {
            while (true)
            {
                var tile = tiles.FirstOrDefault(t => t.GetStatus() == Tile.TileStatus.ToUnload);
                if (tile != null)
                {
                    tile.Unload(mapGenerator);
                    tiles.Remove(tile);
                }
                yield return null;
            }
        }

        private void GoToTile(int x, int y, bool start = false)
        {
            if (!start) mapGenerator.DisableTile(this.x, this.y);
            this.x = x;
            this.y = y;

            // Start loading neighbors
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    var tile = tiles.FirstOrDefault(t => t.GetLocation() == (i, j));
                    if (tile == null)
                    {
                        tile = new Tile(i, j);
                        tiles.Add(tile);
                    }
                    tile.MarkForLoad();
                }
            }

            // Start unloading non-neighbors
            foreach (var nonNeighbor in tiles.Where(t => t.GetLocation().Item1 < x - 1 || t.GetLocation().Item1 > x + 1 || t.GetLocation().Item2 < y - 1 || t.GetLocation().Item2 > y + 1))
            {
                nonNeighbor.MarkForUnload();
            }

            // Wait for the tile we're on to be loaded
            var currentTile = tiles.FirstOrDefault(t => t.GetLocation() == (x, y));
            tiles.Remove(currentTile);
            currentTile.Load(mapGenerator, parent);
            tiles.Add(currentTile);

            mapGenerator.EnableTile(x, y);
        }

        public void Move(WallDirection direction)
        {
            var directionCoords = directionToCoord[direction];
            GoToTile(x + directionCoords.Item1, y + directionCoords.Item2);
        }

        private static Dictionary<WallDirection, (int, int)> directionToCoord = new Dictionary<WallDirection, (int, int)>
        {
            { WallDirection.North, (0, 1) },
            { WallDirection.South, (0, -1) },
            { WallDirection.East, (1, 0) },
            { WallDirection.West, (-1, 0) },
        };
    }

    public enum WallDirection
    {
        North,
        South,
        East,
        West
    }
}
