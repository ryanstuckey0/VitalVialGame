using System;
using UnityEngine;
using ViralVial.PCG.Terrain;

namespace ViralVial
{
    public class BiomeDisplay : MonoBehaviour
    {
        public int seed, size = 128, maxBiomes = 4;
        public bool gauranteeCity = false;
        public bool useSeed = false;

        // Start is called before the first frame update
        void Start()
        {
            var random = useSeed ? new System.Random(seed) : new System.Random(); 
            var generator = new BiomeGenerator();
            var map = generator.GenerateBiomeMap(size, random, maxBiomes, gauranteeCity);
            for (var i = 0; i < map.GetLength(0); i ++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.parent = transform;
                    go.transform.position = new Vector3(i, 0, j);
                    var point = 256 * (((float)map[i, j]) / (Enum.GetNames(typeof(Biome)).Length - 1));
                    go.GetComponent<Renderer>().material.color = new Color(256 - point, point, 0);
                }
            }
        }
    }
}
