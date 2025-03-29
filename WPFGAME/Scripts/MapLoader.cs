using System;
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

        public MapLoader(int nrPostaci, int characterX, int characterY)
        {
            this.nrPostaci = nrPostaci;
            this.characterX = characterX;
            this.characterY = characterY;
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

            // Create a Grid to display the map
            Grid mapGrid = new Grid();

            // Set row definitions based on the number of rows in the map
            for (int i = 0; i < map.GetLength(0); i++)
            {
                mapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) }); // Set row height to 50
            }

            // Set column definitions based on the number of columns in the map
            for (int j = 0; j < map.GetLength(1); j++)
            {
                mapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) }); // Set column width to 50
            }

            // Iterate through the map and add the tiles as images
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Image tile = new Image
                    {
                        Width = 50,
                        Height = 50,
                        Source = new BitmapImage(new Uri(GetTileImagePath(map[i, j]))) // Set the image source based on tile type
                    };

                    // Set the position of each tile in the grid
                    Grid.SetRow(tile, i);
                    Grid.SetColumn(tile, j);
                    mapGrid.Children.Add(tile);
                }
            }

            // Display the character as an image
            Image character = new Image
            {
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri($"pack://application:,,,/Images/gracz{nrPostaci}.png")) // Character image
            };

            // Set the character's position in the grid
            Grid.SetRow(character, characterY);
            Grid.SetColumn(character, characterX);

            mapGrid.Children.Add(character);

            // Set the grid as the window content
            Application.Current.MainWindow.Content = mapGrid;
        }

        private string GetTileImagePath(int tileType)
        {
            // Return the correct path for each tile's image
            return tileType switch
            {
                1 => "pack://application:,,,/Images/water.png", // Water
                2 => "pack://application:,,,/Images/grass.png", // Grass (or forest, if you have such an image)
                3 => "pack://application:,,,/Images/sand.png",  // Sand
                4 => "pack://application:,,,/Images/stone.png", // Mountain
                5 => "pack://application:,,,/Images/sky.png",   // sky
                6 => "pack://application:,,,/Images/dirt.png",  // dirt
                _ => "pack://application:,,,/Images/water.png"  // Default fallback image
            };
        }
    }
}

