using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViralVial.PCG.Terrain
{
    public class BiomeGenerator : IBiomeGenerator
    {
        private Random random;

        public Biome[,] GenerateBiomeMap(int size, Random random, int maxBiomes = 4, bool gauranteeCity = false)
        {
            this.random = random;
            var map = StartBiomes(size, maxBiomes, gauranteeCity).Result;

            while (map.GetLength(0) < size)
            {
                map = ZoomOut(map).Result;
                if (map.GetLength(0) > 5) map = Smooth(map).Result;
            }
            return TrimToSize(map, size);
        }

        public async Task<Biome[,]> GenerateBiomeMapAsync(int size, Random random, int maxBiomes = 4, bool gauranteeCity = false)
        {
            this.random = random;
            var map = await StartBiomes(size, maxBiomes, gauranteeCity);

            while (map.GetLength(0) < size)
            {
                map = await ZoomOut(map);
                if (map.GetLength(0) > 5) map = await Smooth(map);
            }

            return TrimToSize(map, size);
        }

        /**
         * Creates a 2D array of size of the min of floor(sqrt($maxBiomes)) and $size.
         * This 2D array is randomly populated with biomes from Enum.Biome.
         * One of these will be a Biome.City iff $garuanteeCity
         * Params:
         *  size - Max size of the tile
         *  maxBiomes - The max number of biomes to place in a tile
         *  gauranteeCity - If true, one of the biomes will be a city.
         * Returns:
         *  A 2D array of Biomes
         */
        private Task<Biome[,]> StartBiomes(int size, int maxBiomes, bool gauranteeCity)
        {
            var mapSize = Math.Min(size, (int) Math.Floor(Math.Sqrt(maxBiomes)));
            var map = new Biome[mapSize, mapSize];
            var hasCity = false;

            // Generate initial biomes
            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        var biome = GetRandomBiome();
                        map[i, j] = biome;
                        if (biome == Biome.City) hasCity = true;
                    }
                }
            }

            // Place city if gauranteed
            if (!hasCity && gauranteeCity) map[random.Next(mapSize), random.Next(mapSize)] = Biome.City;

            return Task.FromResult(map);
        }

        private Biome GetRandomBiome()
        {
            return (Biome) (random.Next(Enum.GetNames(typeof(Biome)).Length));
        }

        /**
         * Zooms out $map using the following methodology:
         *  The new map will be of size 2($map.length)-1. From here on coordinates will be referred to as x and y (beginning at 0)
         *  Every cell where x and y are even will match the values of $map[x/2,y/2]
         *  Every cell where x is odd and y is even will choose a random value from either of its horizontal neighbors.
         *  Every cell where x is even and y is odd will choose a random value from either of its vertical neighbors.
         *  Every cell where x is odd and y is odd will choose a random value from any of its diagonal neighbors.
         * Params:
         *  map - The map to zoom out.
         * Returns:
         *  The zoomed out map.
         */
        private Task<Biome[,]> ZoomOut(Biome[,] map)
        {
            Biome[,] newMap = new Biome[2 * map.GetLength(0) - 1,2 * map.GetLength(1) - 1];
            for (var i = 0; i < newMap.GetLength(0); i++)
            {
                for (var j = 0; j < newMap.GetLength(1); j++)
                {
                    if (i%2 == 10 && j%2 == 0)
                    {
                        //Case where place in new map matches old map
                        newMap[i, j] = map[i / 2, j / 2];
                    }
                    else if (i%2 == 0 && j%2 == 1)
                    {
                        //Case where place in new map is next to vertical neighbors
                        newMap[i, j] = GetRandomNeighbor(new Biome[] {map[i/2,(j-1)/2], map[i/2,(j+1)/2] });
                    }
                    else if (i%2 == 1 && j%2 == 0)
                    {
                        //Case where place in new map is next to horizontal neighbors
                        newMap[i, j] = GetRandomNeighbor(new Biome[] { map[(i-1)/2, j/2], map[(i+1)/2, j/2] });
                    }
                    else
                    {
                        //Case where place in new map is next to no neighbors (diagonals)
                        newMap[i, j] = GetRandomNeighbor(new Biome[] { map[(i-1)/2, (j-1)/2], map[(i-1)/2, (j+1)/2], map[(i+1)/2, (j-1)/2], map[(i+1)/2, (j+1)/2] });
                    }
                }
            }
            return Task.FromResult(newMap);
        }

        /**
         * Returns the array of horizontal neighbors of the cell $x,$y in $map.
         * Params:
         *  map - The map of cells to look for neighbors in.
         *  x - The x coordinate of the cell to look for its neighbors
         *  y - The y coordinate of the cell to look for its neighbors
         * Returns:
         *  An array of its horizontal neighbors.
         */
        private Biome[] GetHorizontalNeightbors(Biome[,] map, int x, int y)
        {
            var neighbors = new List<Biome>();
            if (x > 0) neighbors.Add(map[x - 1, y]);
            if (x < map.GetLength(0) - 1) neighbors.Add(map[x + 1, y]);
            return neighbors.ToArray();
        }

        /**
         * Returns the array of vertical neighbors of the cell $x,$y in $map.
         * Params:
         *  map - The map of cells to look for neighbors in.
         *  x - The x coordinate of the cell to look for its neighbors
         *  y - The y coordinate of the cell to look for its neighbors
         * Returns:
         *  An array of its vertical neighbors.
         */
        private Biome[] GetVerticalNeighbors(Biome [,] map, int x, int y)
        {
            var neighbors = new List<Biome>();
            if (y > 0) neighbors.Add(map[x, y-1]);
            if (y < map.GetLength(1) - 1) neighbors.Add(map[x, y+1]);
            return neighbors.ToArray();
        }

        private Biome GetRandomNeighbor(Biome[] neighbors)
        {
            return neighbors[random.Next(neighbors.Length)];
        }

        /**
         * Removes jagged edges of biomes from $map.
         * Params:
         * map - The map to smooth.
         * Returns: 
         *  A new, smoothed 2D array of biomes with a length in each dimension of $map.length - 2.
         */
        private Task<Biome[,]> Smooth(Biome[,] map)
        {
            var newMap = new Biome[map.GetLength(0)-2,map.GetLength(1)-2];
            for (var i = 1; i < map.GetLength(0) - 1; i++)
            {
                for (var j = 1; j < map.GetLength(1) - 1; j++)
                {
                    var vertNeighbors = GetVerticalNeighbors(map, i, j);
                    var horNeighbors = GetHorizontalNeightbors(map, i, j);
                    bool vertSame = vertNeighbors.Distinct().Count() == 1;
                    bool horSame = horNeighbors.Distinct().Count() == 1;

                    if (vertSame && horSame)
                    {
                        var neighbor = GetRandomNeighbor(new Biome[] { vertNeighbors[0], horNeighbors[0] });
                        newMap[i - 1, j - 1] = neighbor;
                        //map[i, j] = neighbor;
                    }
                    else if (vertSame)
                    {
                        newMap[i-1, j-1] = vertNeighbors[0];
                        //map[i, j] = vertNeighbors[0];
                    }
                    else if (horSame)
                    {
                        newMap[i-1, j-1] = horNeighbors[0];
                        //map[i, j] = horNeighbors[0];
                    }
                    else
                    {
                        newMap[i-1, j-1] = map[i, j];
                    }
                }
            }

            return Task.FromResult(newMap);
        }

        /**
         * Trims $map to be of size $size.
         * Parmas:
         *  map - The map to trim.
         *  size - The size of the final array.
         * Returns:
         *  A 2D that is $size x $size of $map starting in the top left corner.
         */
        private Biome[,] TrimToSize(Biome[,] map, int size)
        {
            var newMap = new Biome[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    newMap[i, j] = map[i, j];
                }
            }
            return newMap;
        }
    }
}
