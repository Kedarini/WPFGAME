using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPFGAME
{
    public class CharacterMovement
    {
        private int[,]? map;
        private int characterX;
        private int characterY;
        private int nrPostaci;
        private bool isJumping = false;
        private int jumpHeight = 3; // Maximum jump height
        private int jumpCount = 0;
        private CollisionConfig collisionConfig;
        private bool isMoving = false; // To prevent multiple movements at the same time

        public CharacterMovement(int[,]? map, int characterX, int characterY, int nrPostaci, CollisionConfig collisionConfig)
        {
            this.map = map;
            this.characterX = characterX;
            this.characterY = characterY;
            this.nrPostaci = nrPostaci;
            this.collisionConfig = collisionConfig;
            StartGravity();
        }

        public async void MoveCharacter(Key key)
        {
            if (map == null || isMoving) return;

            isMoving = true;
            switch (key)
            {
                case Key.Left:
                    await MoveLeft();
                    break;
                case Key.Right:
                    await MoveRight();
                    break;
                case Key.Up:
                    Jump();
                    break;
            }
            isMoving = false;
        }

        private async Task MoveLeft()
        {
            if (map != null && characterX > 0 && !collisionConfig.IsCollidable(map[characterY, characterX - 1]))
            {
                characterX--;
                DisplayMap();
                await Task.Delay(50); // Small delay for fast but small steps
            }
        }

        private async Task MoveRight()
        {
            if (map != null && characterX < map.GetLength(1) - 1 && !collisionConfig.IsCollidable(map[characterY, characterX + 1]))
            {
                characterX++;
                DisplayMap();
                await Task.Delay(50); // Small delay for fast but small steps
            }
        }

        private void Jump()
        {
            if (!isJumping)
            {
                isJumping = true;
                jumpCount = 0;
                JumpStep();
            }
        }

        private async void JumpStep()
        {
            if (map != null && jumpCount < jumpHeight && characterY > 0 && !collisionConfig.IsCollidable(map[characterY - 1, characterX]))
            {
                characterY--;
                jumpCount++;
                DisplayMap();
                await Task.Delay(50); // Small delay for fast but small steps
                JumpStep();
            }
            else
            {
                isJumping = false;
            }
        }

        private void Fall()
        {
            if (map == null) return;

            while (characterY < map.GetLength(0) - 1 && !collisionConfig.IsCollidable(map[characterY + 1, characterX]))
            {
                characterY++;
                DisplayMap();
            }
        }

        private void StartGravity()
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                while (true)
                {
                    if (!isJumping)
                    {
                        Fall();
                    }
                    await Task.Delay(50); // Adjust the delay as needed
                }
            }, System.Windows.Threading.DispatcherPriority.Background);
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.MainWindow.Content = mapGrid;
            });
        }

        private string GetTileImagePath(int tileType)
        {
            // Return the correct path for each tile's image
            return tileType switch
            {
                1 => "pack://application:,,,/Images/water.png", // Water
                2 => "pack://application:,,,/Images/grass.png", // Grass (or forest, if you have such an image)
                3 => "pack://application:,,,/Images/sand.png",  // Sand
                4 => "pack://application:,,,/Images/stone.png", // Stone
                5 => "pack://application:,,,/Images/sky.png",   // Sky
                6 => "pack://application:,,,/Images/dirt.png",  // Dirt
                _ => "pack://application:,,,/Images/water.png"  // Default fallback image
            };
        }
    }
}
