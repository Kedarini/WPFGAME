using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public int nrPostaci = 1;
        private int characterX = 20;
        private int characterY = 11;
        private MapGenerator mapGenerator;
        private AlertHelper alertHelper;
        private MapLoader mapLoader;
        private CharacterMovement characterMovement;

        public MainWindow()
        {
            InitializeComponent();
            mapGenerator = new MapGenerator();
            alertHelper = new AlertHelper();
            mapLoader = new MapLoader(nrPostaci, characterX, characterY);
            this.KeyDown += Window_KeyDown;
            this.Loaded += Window_Loaded;

            // Initialize the collision configuration
            CollisionConfig collisionConfig = new CollisionConfig();

            // Load the map
            map = mapLoader.GetMap();

            // Check if the map is null and initialize it if necessary
            if (map == null)
            {
                map = new int[20, 20]; // Example size, adjust as needed
            }

            // Initialize the character movement
            characterMovement = new CharacterMovement(map, characterX, characterY, nrPostaci, collisionConfig);
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
                if (Save.Visibility == Visibility.Collapsed)
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
                mapLoader.LoadMapFromFile($"map{LoadedSave}.txt");
                CollisionConfig collisionConfig = new CollisionConfig();
                characterMovement = new CharacterMovement(mapLoader.GetMap(), characterX, characterY, nrPostaci, collisionConfig);
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                AlertText.Text = "Please select a save slot first!";
                ShowAlertText();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (characterMovement != null)
            {
                characterMovement.MoveCharacter(e.Key);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus(); // Ensures the window has focus to receive key events
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
            alertHelper.ShowAlertText(AlertText);
        }

        private void DisplayMap()
        {
            mapLoader.DisplayMap();
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
                mapGenerator.GenerateMapsIfNotExists(); // This will regenerate the map if it doesn't exist
            }
        }

        private void PreviousCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (nrPostaci > 1)
            {
                nrPostaci -= 1;
            }
            else
            {
                nrPostaci = 2;  // Go to the last character when at the first one
            }
            PostacGracza.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/gracz{nrPostaci}.png"));
        }

        private void NextCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (nrPostaci < 2)
            {
                nrPostaci += 1;
            }
            else
            {
                nrPostaci = 1;  // Go back to the first character when at the last one
            }
            PostacGracza.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/gracz{nrPostaci}.png"));
        }
    }
}

