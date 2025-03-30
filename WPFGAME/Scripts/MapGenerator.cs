using System;
using System.IO;
using System.Windows;

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

            // Get the screen resolution
            int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            int screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            // Calculate the number of rows and columns based on 32x32 tiles
            int tileSize = 32;
            int rows = screenHeight / tileSize;
            int cols = screenWidth / tileSize;

            // Add one more row for the additional row with value 21
            int[,] map = new int[rows + 1, cols];
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

            for (int i = skyHeight; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = 1;
                }
            }

            // Step 3: Generate grass with varying heights
            int currentHeight = skyHeight;
            int sameHeightCount = 0;
            int maxSameHeight = random.Next(2, 6); // Randomly choose between 2 and 5 for the same height

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

                map[currentHeight, j] = 2; // Grass
                sameHeightCount++;

                // Step 4: Fill the stone (4) just below the grass if 10 blocks under skyHeight
                for (int i = currentHeight + 1; i < rows; i++)
                {
                    if (i >= skyHeight + 5)
                    {
                        map[i, j] = 4; // Stone
                    }
                    else
                    {
                        map[i, j] = 6; // Dirt
                    }
                }
            }

            // Step 5: Replace grass with grass_corner_right or grass_corner_left
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (map[i, j] == 2) // Grass
                    {
                        if (j > 0 && map[i, j - 1] == 5) // Sky to the left
                        {
                            map[i, j] = 7; // grass_corner_left
                        }
                        else if (j < cols - 1 && map[i, j + 1] == 5) // Sky to the right
                        {
                            map[i, j] = 8; // grass_corner_right
                        }
                    }
                }
            }

            // Step 6: Fill the water (1) below the dirt and surround it with sand (3)
            for (int i = currentHeight + 1; i < rows; i++)
            {
                for (int j = 1; j < cols - 1; j++) // Avoid first and last columns
                {
                    if (map[i, j] == 9) // Only replace pastel gray background with water
                    {
                        map[i, j] = 1; // Water
                    }
                }
            }

            // Surround water with sand
            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    if (map[i, j] == 1) // Water
                    {
                        if (map[i + 1, j] != 1) map[i + 1, j] = 3; // Sand below
                        if (map[i, j - 1] != 1) map[i, j - 1] = 3; // Sand left
                        if (map[i, j + 1] != 1) map[i, j + 1] = 3; // Sand right
                    }
                }
            }

            // Step 8: Add sugarcane with a 50% chance on sand blocks with sky on top
            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    if (map[i, j] == 3 && map[i - 1, j] == 5 && random.NextDouble() < 0.5) // 50% chance to place sugarcane
                    {
                        int sugarcaneHeight = random.Next(1, 4); // Random height between 1 and 3
                        for (int h = 1; h <= sugarcaneHeight; h++)
                        {
                            if (i - h >= 0)
                            {
                                map[i - h, j] = 12; // Sugarcane
                            }
                        }
                    }
                }
            }

            // Step 7: Add trees with a 10% chance on grass blocks
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (map[i, j] == 2 && random.NextDouble() < 0.1) // 10% chance to place a tree
                    {
                        bool canPlaceTree = true;

                        // Check for minimum 3-block space around the tree
                        for (int x = Math.Max(0, i - 3); x <= Math.Min(rows - 1, i + 3); x++)
                        {
                            for (int y = Math.Max(0, j - 3); y <= Math.Min(cols - 1, j + 3); y++)
                            {
                                if (map[x, y] == 10 || map[x, y] == 11) // Tree parts
                                {
                                    canPlaceTree = false;
                                    break;
                                }
                            }
                            if (!canPlaceTree) break;
                        }

                        if (canPlaceTree && i > 4 && i < rows - 2 && j > 0 && j < cols - 1) // Ensure there's enough space for the tree
                        {
                            map[i - 4, j] = 10; // Tree top
                            map[i - 3, j - 1] = 10; // Tree middle left
                            map[i - 3, j] = 10; // Tree middle
                            map[i - 3, j + 1] = 10; // Tree middle right
                            map[i - 2, j] = 11; // Tree trunk
                            map[i - 1, j] = 11; // Tree trunk
                        }
                    }
                }
            }

            // Step 9: Add random chunks of stone (1% chance) replacing dirt
            for (int i = currentHeight + 1; i < rows - 2; i++)
            {
                for (int j = 1; j < cols - 4; j++)
                {
                    if (map[i, j] == 6 && random.NextDouble() < 0.075) // 1% chance to start a stone chunk
                    {
                        int chunkSize = random.Next(20, 30); // Random size between 10 and 20 blocks
                        int x = i;
                        int y = j;

                        for (int k = 0; k < chunkSize; k++)
                        {
                            if (x >= currentHeight + 1 && x < rows - 1 && y >= 1 && y < cols - 1 && map[x, y] == 6)
                            {
                                map[x, y] = 4; // Stone
                                               // Randomly move to a neighboring cell
                                switch (random.Next(4))
                                {
                                    case 0: x++; break; // Move down
                                    case 1: x--; break; // Move up
                                    case 2: y++; break; // Move right
                                    case 3: y--; break; // Move left
                                }
                            }
                        }
                    }
                }
            }


            // Step 11: Randomly change stone to ores
            for (int i = currentHeight + 1; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (map[i, j] == 4) // Stone
                    {
                        double chance = random.NextDouble();
                        if (chance < 0.025) map[i, j] = 13; // 2.5% chance for ore 13
                        else if (chance < 0.05) map[i, j] = 15; // 2.5% chance for ore 15
                        else if (chance < 0.075) map[i, j] = 19; // 2.5% chance for ore 19
                        else if (chance < 0.0875) map[i, j] = 14; // 1.25% chance for ore 14
                        else if (chance < 0.10) map[i, j] = 20; // 1.25% chance for ore 20
                        else if (chance < 0.1125) map[i, j] = 18; // 1.25% chance for ore 18
                        else if (chance < 0.1175) map[i, j] = 16; // 0.5% chance for ore 16
                        else if (chance < 0.1225) map[i, j] = 17; // 0.5% chance for ore 17
                    }
                }
            }

            // Step 12: Add an additional row with value 21
            for (int j = 0; j < cols; j++)
            {
                map[rows, j] = 21;

                // 50% chance to place an additional block on top
                if (random.NextDouble() < 0.5)
                {
                    map[rows - 1, j] = 21;
                }
            }

            // Step 13: Write the map to a file
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < rows + 1; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(map[i, j] + " "); // Write each cell's value
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}