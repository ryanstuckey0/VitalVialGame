using System;

namespace ViralVial.Seed
{
    public class SeedGenerator : ISeed
    {
        public int GetSeed()
        {
            return new Random().Next(int.MinValue, int.MaxValue);
        }

        public int GetSeed(int x)
        {
            return new Random(x).Next(int.MinValue, int.MaxValue);
        }

        public int GetSeed(int x, int y)
        {

            return GetSeed(x) + GetSeed(y);
        }

        public int GetSeed(int x, int y, int z)
        {
            return GetSeed(x) + GetSeed(y) + GetSeed(z);
        }
    }
}
