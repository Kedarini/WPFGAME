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
        private MapLoader mapLoader;

        public CharacterMovement(int[,]? map, int characterX, int characterY, int nrPostaci, CollisionConfig collisionConfig, MapLoader mapLoader)
        {
            this.map = map;
            this.characterX = characterX;
            this.characterY = characterY;
            this.nrPostaci = nrPostaci;
            this.collisionConfig = collisionConfig;
            this.mapLoader = mapLoader;
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
                mapLoader.UpdateCharacterPosition(characterX, characterY);
                await Task.Delay(50); // Small delay for fast but small steps
            }
        }

        private async Task MoveRight()
        {
            if (map != null && characterX < map.GetLength(1) - 1 && !collisionConfig.IsCollidable(map[characterY, characterX + 1]))
            {
                characterX++;
                mapLoader.UpdateCharacterPosition(characterX, characterY);
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
                mapLoader.UpdateCharacterPosition(characterX, characterY);
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
                mapLoader.UpdateCharacterPosition(characterX, characterY);
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
    }
}
