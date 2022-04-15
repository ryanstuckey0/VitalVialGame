namespace ViralVial.Seed
{
    public interface ISeed
    {
        public int GetSeed();

        public int GetSeed(int x);

        public int GetSeed(int x, int y);

        public int GetSeed(int x, int y, int z);
    }
}
