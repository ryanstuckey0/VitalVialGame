using System.Threading.Tasks;
using UnityEngine;

namespace ViralVial.PCG.Terrain
{
    public interface IMapGenerator
    {
        public GameObject LoadTile(int x, int y, Transform parent);

        public void EnableTile(int x, int y);

        public void DisableTile(int x, int y);

        public void UnloadTile(int x, int y);

        public Task<GameObject> LoadTileAsync(int x, int y, Transform parent);

        public Task UnloadTileAsync(int x, int y);
    }
}
