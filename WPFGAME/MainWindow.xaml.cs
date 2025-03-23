using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPFGAME
{
    public partial class MainWindow : Window
    {
        public int LoadedSave;
        private int[,]? map;

        public MainWindow()
        {
            InitializeComponent();
            GenerateMapsIfNotExists();  // Generate maps when the app starts
        }

        // Map generation logic that ensures the maps are generated only if they don't exist
        private void GenerateMapsIfNotExists()
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

            // Step 1: Fill the sky (5) on the top half of the map
            int skyHeight = rows / 2; // Sky occupies the top half of the map
            for (int i = 0; i < skyHeight; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = 5; // Sky
                }
            }

            // Step 2: Fill the grass (2) just under the sky
            int grassHeight = skyHeight; // Grass starts immediately below the sky
            for (int i = grassHeight; i < rows - 1; i++) // Stop one row before the last row (dirt row)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = 2; // Grass
                }
            }

            // Step 3: Fill the dirt (6) just below the grass
            int dirtHeight = grassHeight + 1; // Dirt directly under grass
            for (int i = dirtHeight; i < rows; i++) // Dirt fills the bottom row
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = 6; // Dirt
                }
            }

            // Step 4: Write the map to a file
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(map[i, j] + " "); // Write each cell's value
                    }
                    writer.WriteLine();
                }
            }
        }



        // Method to load a map from a file
        private void LoadMapFromFile(string filePath)
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

        private void DisplayMap()
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

            // Set the grid as the window content
            this.Content = mapGrid;
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

        // Handling save slot selection for save1
        private void Save1_Click(object sender, RoutedEventArgs e)
        {
            if (save1bg.Background == Brushes.LightGray)
            {
                save1bg.Background = Brushes.Transparent;
                Save.Visibility = Visibility.Collapsed;
                LoadedSave = 0;
            }
            else if (save1bg.Background == Brushes.Transparent)
            {
                save1bg.Background = Brushes.LightGray;
                save2bg.Background = Brushes.Transparent;
                save3bg.Background = Brushes.Transparent;
                LoadedSave = 1;
                SaveName.Text = "Save 1";
                DateTime creationDate = File.GetCreationTime("map1.txt").Date;
                SaveDate.Text = $"{creationDate.ToShortDateString()}";
                if (Save.Visibility == Visibility.Collapsed)
                {
                    Save.Visibility = Visibility.Visible;
                    AnimateSaveGrid();
                }
            }
        }

        // Handling save slot selection for save2
        private void Save2_Click(object sender, RoutedEventArgs e)
        {
            if (save2bg.Background == Brushes.LightGray)
            {
                save2bg.Background = Brushes.Transparent;
                Save.Visibility = Visibility.Collapsed;
                LoadedSave = 0;
            }
            else if (save2bg.Background == Brushes.Transparent)
            {
                save1bg.Background = Brushes.Transparent;
                save2bg.Background = Brushes.LightGray;
                save3bg.Background = Brushes.Transparent;
                LoadedSave = 2;
                SaveName.Text = "Save 2";
                DateTime creationDate = File.GetCreationTime("map2.txt").Date;
                SaveDate.Text = $"{creationDate.ToShortDateString()}";
                if (Save.Visibility == Visibility.Collapsed)
                {
                    Save.Visibility = Visibility.Visible;
                    AnimateSaveGrid();
                }
            }
        }

        // Handling save slot selection for save3
        private void Save3_Click(object sender, RoutedEventArgs e)
        {
            if (save3bg.Background == Brushes.LightGray)
            {
                save3bg.Background = Brushes.Transparent;
                Save.Visibility = Visibility.Collapsed;
                LoadedSave = 0;
            }
            else if (save3bg.Background == Brushes.Transparent)
            {
                save1bg.Background = Brushes.Transparent;
                save2bg.Background = Brushes.Transparent;
                save3bg.Background = Brushes.LightGray;
                LoadedSave = 3;
                SaveName.Text = "Save 3";
                DateTime creationDate = File.GetCreationTime("map3.txt").Date;
                SaveDate.Text = $"{creationDate.ToShortDateString()}";
                if(Save.Visibility == Visibility.Collapsed)
                {
                    Save.Visibility = Visibility.Visible;
                    AnimateSaveGrid();
                }
            }
        }

        // Handling map load based on the selected save slot
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (LoadedSave != 0)
            {
                LoadMapFromFile($"map{LoadedSave}.txt");
            }
            else
            {
                AlertText.Text = "Please select a save slot first!";
                ShowAlertText();
            }
        }
        private void AnimateSaveGrid()
        {
            // Make sure the RenderTransformOrigin is set to (0.3, 0.3) to animate from the center
            Save.RenderTransformOrigin = new Point(0.3, 0.3);

            // Create a DoubleAnimation for opacity (fade in effect)
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0, // Start at 0 opacity (invisible)
                To = 1,   // End at 1 opacity (fully visible)
                Duration = TimeSpan.FromSeconds(0.25), // Faster animation duration (0.25 seconds)
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // Smooth ease-in and ease-out
            };

            // Apply the fade-in animation to the opacity of the Save grid
            Save.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            // Apply scaling transformation from the center
            ScaleTransform scaleTransform = new ScaleTransform(0, 0);  // Start with 0 scale (invisible)
            Save.RenderTransform = scaleTransform;  // Apply the scale transform to the Save grid

            // Create scale animations (for both X and Y axes)
            DoubleAnimation scaleXAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25)); // 0.25 seconds for faster animation
            DoubleAnimation scaleYAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25)); // 0.25 seconds for faster animation

            // Start the scaling animation
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
        }

        private void ShowAlertText()
        {
            // Make the alert text visible
            AlertText.Visibility = Visibility.Visible;

            // Set the initial opacity to 0 (in case it starts invisible)
            AlertText.Opacity = 0;

            // Create a Storyboard to hold the animations
            Storyboard storyboard = new Storyboard();

            // Create a fade-in animation (0 to 1)
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5) // Fade-in duration
            };
            Storyboard.SetTarget(fadeInAnimation, AlertText);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Add fade-in animation to the storyboard
            storyboard.Children.Add(fadeInAnimation);

            // Start the fade-in animation
            storyboard.Begin();

            // Create a DispatcherTimer to wait for 5 seconds
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Set the timer interval to 5 seconds
            };

            // When the timer ticks, start the fade-out animation
            timer.Tick += (sender, e) =>
            {
                // Stop the timer as we only need it once
                timer.Stop();

                // Create a fade-out animation (1 to 0)
                DoubleAnimation fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5) // Fade-out duration
                };
                Storyboard.SetTarget(fadeOutAnimation, AlertText);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

                // Create a new storyboard for the fade-out animation
                Storyboard fadeOutStoryboard = new Storyboard();
                fadeOutStoryboard.Children.Add(fadeOutAnimation);

                // Begin the fade-out animation
                fadeOutStoryboard.Begin();

                // Optionally, hide the AlertText after the fade-out is complete
                fadeOutStoryboard.Completed += (s, args) =>
                {
                    AlertText.Visibility = Visibility.Collapsed; // Hide the TextBlock after the animation completes
                };
            };

            // Start the timer
            timer.Start();
        }


        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (LoadedSave != 0)
            {
                // Determine the map file path based on the selected save slot
                string mapFileName = $"map{LoadedSave}.txt";

                // Check if the file exists and delete it if it does
                if (File.Exists(mapFileName))
                {
                    try
                    {
                        File.Delete(mapFileName);
                        AlertText.Text = $"Save{LoadedSave} restarted successfully.";
                        ShowAlertText();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting the file: {ex.Message}");
                        return;
                    }
                }

                // Regenerate the map after deletion
                GenerateMapIfNotExists(mapFileName); // This will regenerate the map if it doesn't exist
            }
        }
    }
}