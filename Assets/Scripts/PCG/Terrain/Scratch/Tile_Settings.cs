using System.Collections.Generic;
using UnityEngine;

namespace ViralVial
{
    public class Tile_Settings : MonoBehaviour
    {
        public static Tile_Settings instance;
        public List<GameObject> trees;
        public List<Texture2D> grass;
        public TerrainLayer[] terrainLayers;

        public Tile_Settings()
        {
            if (instance == null) instance = this;
        }
    }
}
