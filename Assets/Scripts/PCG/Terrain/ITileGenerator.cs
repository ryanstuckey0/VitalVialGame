using UnityEngine;

namespace ViralVial.PCG.Terrain
{
    public interface ITileGenerator
    {
        public GameObject GenerateTile(Vector3 size, GameObject tile, int seed);
    }
}
