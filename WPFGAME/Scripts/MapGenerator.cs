using System;
using System.IO;

namespace WPFGAME
{
    public class MapGenerator
    {
        public void GenerateMapsIfNotExists()
        {
            GenerateMapIfNotExists("map1.txt");
            GenerateMapIfNotExists("map2.txt");
            GenerateMapIfNotExists("map3.txt");
        }

        private void GenerateMapIfNotExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                Console.WriteLine($"{fileName} already exists. Skipping generation.");
                return;
            }

            int rows = 30, cols = 50; // Map size
            int[,] map = new int[rows, cols];
            Random random = new Random();

            // Step 1: Fill the sky (5) on the top half of the map
            int skyHeight = rows / 2; // Sky occupies the top half of the map
            for (int i = 0; i < skyHeight; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = 5; // Sky
                }
            }

            // Step 2: Fill the background with pastel gray (9) below the sky
            for (int i = skyHeight; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = 9; // Pastel gray background
                }
            }

            // Step 3: Generate grass with varying heights
            int currentHeight = skyHeight;
            int sameHeightCount = 0;
            int maxSameHeight = random.Next(2, 6);

            for (int j = 0; j < cols; j++)
            {
                // Change height only if the same height count exceeds the maxSameHeight
                if (sameHeightCount >= maxSameHeight)
                {
                    int heightChange = random.Next(-1, 2);
                    currentHeight = Math.Clamp(currentHeight + heightChange, skyHeight - 2, skyHeight + 1);
                    sameHeightCount = 0;
                    maxSameHeight = random.Next(2, 6); // Randomly choose a new maxSameHeight
                }

                // Check for smooth transitions
                if (j > 0 && map[currentHeight, j - 1] == 2)
                {
                    if (currentHeight < rows - 1 && map[currentHeight + 1, j - 1] == 2)
                    {
                        map[currentHeight, j] = 7; // grass_corner_left
                    }
                    else if (currentHeight > skyHeight && map[currentHeight - 1, j - 1] == 2)
                    {
                        map[currentHeight, j] = 8; // grass_corner_right
                    }
                    else
                    {
                        map[currentHeight, j] = 2; // Grass
                    }
                }
                else
                {
                    map[currentHeight, j] = 2; // Grass
                }

                sameHeightCount++;

                // Step 4: Fill the dirt (6) just below the grass
                for (int i = currentHeight + 1; i < rows; i++)
                {
                    map[i, j] = 6; // Dirt
                }
            }

            // Step 5: Fill the water (1) below the dirt
            for (int i = currentHeight + 1; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (map[i, j] == 9)
                    {
                        map[i, j] = 1;
                    }
                }
            }

            // Step 6: Write the map to a file
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(map[i, j] + " ");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
