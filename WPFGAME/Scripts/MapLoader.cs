using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFGAME
{
    public class MapLoader
    {
        private int[,]? map;
        private int nrPostaci;
        private int characterX;
        private int characterY;
        private readonly BlockBreaker blockBreaker;

        public Grid MapGrid { get; private set; }

        public MapLoader(int nrPostaci, int characterX, int characterY)
        {
            this.nrPostaci = nrPostaci;
            this.characterX = characterX;
            this.characterY = characterY;
            MapGrid = new Grid();
            blockBreaker = new BlockBreaker(this);
        }

        public int[,]? GetMap()
        {
            return map;
        }

        public void LoadMapFromFile(string filePath)
        {
            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Count rows based on lines, but ensure they are non-empty
                int rows = lines.Length;
                if (rows == 0)
                {
                    throw new Exception("Map file is empty.");
                }

                // Split the first non-empty line to determine the column count
                string[] firstLineValues = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int cols = firstLineValues.Length;

                // Initialize the map array
                map = new int[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    // Trim each line to remove any leading/trailing spaces and skip empty lines
                    string line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty or whitespace-only lines

                    string[] values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // If the line has a different column count, handle it (optional validation)
                    if (values.Length != cols)
                    {
                        throw new Exception($"Line {i + 1} has an incorrect number of columns.");
                    }

                    for (int j = 0; j < cols; j++)
                    {
                        map[i, j] = int.Parse(values[j]); // Parse the integer value from the string
                    }
                }

                // Display the map once it's loaded successfully
                DisplayMap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading map: " + ex.Message);
            }
        }

        public void DisplayMap()
        {
            if (map == null) return;

            // Clear previous children
            MapGrid.Children.Clear();
            MapGrid.RowDefinitions.Clear();
            MapGrid.ColumnDefinitions.Clear();

            // Set row definitions based on the number of rows in the map
            for (int i = 0; i < map.GetLength(0); i++)
            {
                MapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) }); // Set row height to 32
            }

            // Set column definitions based on the number of columns in the map
            for (int j = 0; j < map.GetLength(1); j++)
            {
                MapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32) }); // Set column width to 32
            }

            // Iterate through the map and add the tiles as images
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    string imagePath = GetTileImagePath(map[i, j]);
                    Console.WriteLine($"Tile at ({i}, {j}) with type {map[i, j]} uses image path: {imagePath}");

                    try
                    {
                        Image tile = new Image
                        {
                            Width = 32,
                            Height = 32,
                            Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)) // Set the image source based on tile type
                        };

                        // Set the position of each tile in the grid
                        Grid.SetRow(tile, i);
                        Grid.SetColumn(tile, j);
                        MapGrid.Children.Add(tile);

                        // Add mouse event handler for breaking blocks
                        if (blockBreaker.IsWithinRange(characterX, characterY, i, j))
                        {
                            tile.MouseLeftButtonDown += (sender, e) =>
                            {
                                Console.WriteLine($"Mouse clicked on tile at ({i}, {j})");
                                blockBreaker.BreakBlock(i, j);
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading image at ({i}, {j}) with path {imagePath}: {ex.Message}");
                    }
                }
            }

            // Display the character as an image
            Image character = new Image
            {
                Width = 32,
                Height = 64,
                Source = new BitmapImage(new Uri($"pack://application:,,,/Images/gracz{nrPostaci}.png", UriKind.RelativeOrAbsolute)) // Character image
            };

            // Set the character's position in the grid
            Grid.SetRow(character, characterY);
            Grid.SetColumn(character, characterX);

            MapGrid.Children.Add(character);
        }

        private string GetTileImagePath(int tileType)
        {
            // Log the tileType value
            Console.WriteLine($"Tile type: {tileType}");

            // Return the correct path for each tile's image
            return tileType switch
            {
                1 => "pack://application:,,,/Images/water.png", // Water
                2 => "pack://application:,,,/Images/grass.png", // Grass
                3 => "pack://application:,,,/Images/sand.png",  // Sand
                4 => "pack://application:,,,/Images/stone.png", // Mountain
                5 => "pack://application:,,,/Images/sky.png",   // Sky
                6 => "pack://application:,,,/Images/dirt.png",  // Dirt
                7 => "pack://application:,,,/Images/grass_corner_left.png", // Grass corner left
                8 => "pack://application:,,,/Images/grass_corner_right.png", // Grass corner right
                9 => "pack://application:,,,/Images/stone_background.png", // Stone background
                10 => "pack://application:,,,/Images/leaves.png", // Leaves
                11 => "pack://application:,,,/Images/log.png", // Log
                12 => "pack://application:,,,/Images/sugarcane.png", // Log
                13 => "pack://application:,,,/Images/coal_ore.png", // Log
                14 => "pack://application:,,,/Images/iron_ore.png", // Log
                15 => "pack://application:,,,/Images/lapis_ore.png", // Log
                16 => "pack://application:,,,/Images/emerald_ore.png", // Log
                17 => "pack://application:,,,/Images/diamond_ore.png", // Log
                18 => "pack://application:,,,/Images/copper_ore.png", // Log
                19 => "pack://application:,,,/Images/redstone_ore.png", // Log
                20 => "pack://application:,,,/Images/gold_ore.png", // Log
                21 => "pack://application:,,,/Images/bedrock.png", // Log
                _ => "pack://application:,,,/Images/water.png"  // Default fallback image
            };
        }

        public void UpdateCharacterPosition(int newX, int newY)
        {
            characterX = newX;
            characterY = newY;
            DisplayMap();
        }
    }
}