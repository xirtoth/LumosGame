using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Buffers.Text;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;

namespace Lumos
{
    public class Player
    {
        public Vector2 Pos { get; set; }

        public Vector2 PreviousPos { get; set; }
        public string Name { get; set; }

        private readonly int Size = 16;
        public float previousBottom { get; set; }
        public MapTiles selectedTile { get; set; } = MapTiles.water;

        public float MoveSpeed { get; set; } = 2f;

        public Rectangle Rect { get; set; }

        public Rectangle HorizontalCollisionRect { get; set; }
        public Rectangle VerticalCollisionRect { get; set; }
        public Rectangle LocalBounds { get; set; }

        public Texture2D PlayerTex;

        private const float Gravity = 9f;
        public float velocityY { get; set; }
        public bool IsOnGround { get; internal set; }
        public float velocityX { get; internal set; }
        public bool isCollidingLeft { get; internal set; } = false;
        public bool isCollidingRight { get; internal set; } = false;

        public Player()
        { }

        public Player(Vector2 pos, string name, Texture2D playerTex)
        {
            Pos = pos;
            Name = name;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, Size, Size);
            PlayerTex = playerTex;
            PreviousPos = pos;
            int width = (int)(playerTex.Width * 0.4);
            int left = (playerTex.Width - width) / 2;
            int height = (int)(playerTex.Height * 0.8);
            int top = playerTex.Width - height;
            LocalBounds = new Rectangle(left, top, width, height);
        }

        public void Draw(SpriteBatch _sb, Vector2 cameraPos, Viewport viewport)
        {
            Vector2 drawPosition = Pos - cameraPos;
            _sb.Draw(PlayerTex, drawPosition, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            PreviousPos = Pos;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity only if the player is not on the ground
            if (!IsOnGround)
            {
                velocityY += Gravity * deltaTime;
            }

            CheckInput();

            Pos += new Vector2(velocityX * deltaTime, velocityY * deltaTime);

            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, Size, Size);
            isCollidingLeft = false;
            isCollidingRight = false;
            IsOnGround = false;
        }

        private void CheckInput()
        {
            MouseState mouseState = Mouse.GetState();
            float moveSpeedX = MoveSpeed;
            float moveSpeedY = MoveSpeed;

            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            if (pressedKeys.Length > 0)
            {
                // Handle player movement
                if (pressedKeys.Contains(Keys.D))
                {
                    velocityX = MoveSpeed; // Set velocity to move right
                }
                else if (pressedKeys.Contains(Keys.A))
                {
                    velocityX = -MoveSpeed; // Set velocity to move left
                }
                else if (pressedKeys.Contains(Keys.S))
                {
                    velocityY = MoveSpeed;
                }
                else
                {
                    velocityX = 0; // No horizontal movement
                    velocityY = 0;
                }

                // Handle jumping
                if (pressedKeys.Contains(Keys.W) && IsOnGround)
                {
                    PreviousPos = Pos;
                    velocityY -= 10f;
                }
            }
            else
            {
                velocityX = 0; // No horizontal movement when no keys are pressed
            }

            // Update player position based on velocity
            PreviousPos = Pos;
            Pos += new Vector2(velocityX, velocityY);
        }
    }
}