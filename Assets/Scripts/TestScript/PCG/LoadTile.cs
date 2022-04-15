using UnityEngine;
using ViralVial.PCG.Terrain;
using ViralVial.Seed;

namespace ViralVial
{
    public class LoadTile : MonoBehaviour
    {
        public int x, y;

        private void Start()
        {
            FindObjectOfType<MapTraversalMono>().InitializeMap(new SeedGenerator().GetSeed(), x, y);
        }
    }
}
