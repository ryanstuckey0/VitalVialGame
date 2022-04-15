using UnityEngine;

namespace ViralVial.PCG.Terrain
{
    public static class MapConfig
    {
        //Tile settings
        public static Vector3 TileSize = new Vector3(128, 32, 128);
        //public static Vector3 TileSize = new Vector3(32, 32, 32);
        public static int BiomeBlend = 3;
        public static int MaxBiomesPerTile = 4;
        public static bool GauranteeCityPerTile = false;

        //Terrain settings
        public static float wavingGrassSpeed = 0.5f;
        public static float wavingGrassAmount = 0.5f;
        public static float wavingGrassStrength = 0.5f;
        public static Color wavingGrassTint = new Color(0.698f, 0.6f, 0.5f);

        //Map traversal settings
        public static float WallDistanceFromEdge = 25f;
        public static float AdditionalWallHeight = 5f;
    }
}
