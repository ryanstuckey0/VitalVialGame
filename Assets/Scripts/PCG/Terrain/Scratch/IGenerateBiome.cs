using UnityEngine;

namespace ViralVial.PCG.Terrain.Scratch
{
    public interface IGenerateBiome
    {
        public void AddHeight(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand);

        public void AddSplat(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand);

        public void AddTrees(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand);

        public void AddGrass(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand);

        public void AddObjects(Texture2D mask, UnityEngine.Terrain terrain, System.Random rand);
    }
}
