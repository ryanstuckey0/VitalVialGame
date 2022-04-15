using System;
using System.Threading.Tasks;

namespace ViralVial.PCG.Terrain
{
    public interface IBiomeGenerator
    {
        public Biome[,] GenerateBiomeMap(int size, Random random, int maxBiomes = 4, bool gauranteeCity = false);
        public Task<Biome[,]> GenerateBiomeMapAsync(int size, Random random, int maxBiomes = 4, bool gauranteeCity = false);
    }
}
