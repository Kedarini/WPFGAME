using System;

namespace WPFGAME
{
    public class BlockBreaker
    {
        private readonly MapLoader mapLoader;
        private readonly Dictionary<int, int> collectedResources;

        public BlockBreaker(MapLoader mapLoader)
        {
            this.mapLoader = mapLoader;
            collectedResources = new Dictionary<int, int>();
        }

        public void BreakBlock(int x, int y)
        {
            var map = mapLoader.GetMap();
            if (map == null)
            {
                Console.WriteLine("Map is not loaded.");
                return;
            }

            if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
            {
                Console.WriteLine($"Block coordinates ({x}, {y}) are out of bounds.");
                return;
            }

            int blockType = map[x, y];
            Console.WriteLine($"Breaking block at ({x}, {y}) of type {blockType}");

            // Collect the resource
            if (collectedResources.ContainsKey(blockType))
            {
                collectedResources[blockType]++;
            }
            else
            {
                collectedResources[blockType] = 1;
            }

            // Set the block to the sky texture (e.g., 5 for sky)
            map[x, y] = 5;

            // Refresh the map display
            mapLoader.DisplayMap();
        }

        public bool IsWithinRange(int characterX, int characterY, int x, int y)
        {
            bool withinRange = Math.Abs(characterX - x) <= 3 && Math.Abs(characterY - y) <= 3;
            Console.WriteLine($"Block at ({x}, {y}) is within range: {withinRange}");
            return withinRange;
        }

        public Dictionary<int, int> GetCollectedResources()
        {
            return new Dictionary<int, int>(collectedResources);
        }
    }
    public class Game
    {
        private readonly MapLoader mapLoader;
        private readonly BlockBreaker blockBreaker;

        public Game()
        {
            mapLoader = new MapLoader(1, 5, 5);
            blockBreaker = new BlockBreaker(mapLoader);
        }

        public void BreakBlock(int x, int y)
        {
            blockBreaker.BreakBlock(x, y);
        }

        public void DisplayCollectedResources()
        {
            var resources = blockBreaker.GetCollectedResources();
            foreach (var resource in resources)
            {
                Console.WriteLine($"Block Type: {resource.Key}, Count: {resource.Value}");
            }
        }
    }
}
