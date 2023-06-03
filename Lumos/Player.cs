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

        public Texture2D[] PlayerTextures;

        private Animation animation;

        private const float Gravity = 0.2f;
        public float velocityY { get; set; }
        public bool IsOnGround { get; internal set; }

        public float velocityX { get; internal set; }
        public bool isCollidingLeft { get; internal set; } = false;
        public bool isCollidingRight { get; internal set; } = false;

        private int facingDirection = 1;

        private float moveBackOffset = 0.01f;

        private float animationTime = 0f;

        private float animationFps = 0.5f;

        private float animationSpeedIdle = 2f;

        private float animationSpeedWalk = 4f;

        private int currentFrame;

        public Player()
        { }

        public Player(Vector2 pos, string name, Texture2D playerTex)
        {
            animation = new Animation();
            Pos = pos;
            Name = name;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, playerTex.Width, playerTex.Height);
            PlayerTex = playerTex;
            PreviousPos = pos;
            int width = (int)(playerTex.Width * 0.4);
            int left = (playerTex.Width - width) / 2;
            int height = (int)(playerTex.Height * 0.8);
            int top = playerTex.Width - height;
            LocalBounds = new Rectangle(left, top, width, height);
            PlayerTextures = TileTextures.PlayerIdle;
            Texture2D[] playerIdleFrames = TileTextures.PlayerIdle;
            Texture2D[] playerWalkFrames = TileTextures.PlayerWalk;
            animation.AddAnimation("idle", playerIdleFrames, animationSpeedIdle);
            animation.AddAnimation("walk", playerWalkFrames, animationSpeedWalk);
        }

        public void Draw(SpriteBatch _sb, Vector2 cameraPos, Viewport viewport)
        {
            Vector2 drawPosition = Pos - cameraPos;
            Rectangle drawRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, PlayerTex.Width, PlayerTex.Height);
            SpriteEffects flipEffect = (facingDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw the player texture with the appropriate flip effect
            //_sb.Draw(PlayerTextures[currentFrame], drawRectangle, null, Color.White, 0f, Vector2.Zero, flipEffect, 0f);
            animation.Draw(_sb, drawPosition, flipEffect);
            // _sb.Draw(PlayerTex, drawPosition, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PreviousPos = Pos;
            animation.Update(gameTime);

            //float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity only if the player is not on the ground
            if (!IsOnGround)
            {
                //      velocityY += Gravity * deltaTime;
            }

            CheckInput();

            Pos += new Vector2(velocityX * deltaTime, velocityY * deltaTime);

            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, PlayerTex.Width, PlayerTex.Height);
            isCollidingLeft = false;
            isCollidingRight = false;
            // IsOnGround = false;
        }

        private bool isJumping = false; // Flag to track if the player is jumping

        private void CheckInput()
        {
            bool isOnGround = false;
            MouseState mouseState = Mouse.GetState();
            float moveSpeedX = MoveSpeed;
            float moveSpeedY = MoveSpeed;
            float gravity = 2f; // Adjust the gravity value as needed

            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            Vector2 newPosition = Pos; // Store the modified position in a separate variable

            if (pressedKeys.Length > 0)
            {
                // Handle player movement
                if (pressedKeys.Contains(Keys.D))
                {
                    if (animation.CurrentAnimationName != "walk")
                    {
                        animation.PlayAnimation("walk");
                    }
                    newPosition.X += MoveSpeed; // Move the player to the right
                    facingDirection = 1;
                    if (CollisionManager.HandleCollision(new Rectangle((int)newPosition.X, (int)newPosition.Y, Rect.Width, Rect.Height), out isOnGround))
                    {
                        newPosition.X = Pos.X - moveBackOffset; // Move player back by 1 unit in the opposite direction
                    }
                    //  PlayerTextures = TileTextures.PlayerWalk;
                }
                else if (pressedKeys.Contains(Keys.A))
                {
                    facingDirection = -1;
                    if (animation.CurrentAnimationName != "walk")
                    {
                        // Play the "walk" animation
                        animation.PlayAnimation("walk");
                    }
                    newPosition.X -= MoveSpeed; // Move the player to the left
                    if (CollisionManager.HandleCollision(new Rectangle((int)newPosition.X, (int)newPosition.Y, Rect.Width, Rect.Height), out isOnGround))
                    {
                        newPosition.X = Pos.X + moveBackOffset; // Move player back by 1 unit in the opposite direction
                    }
                    // PlayerTextures = TileTextures.PlayerWalk;
                }
                else if (pressedKeys.Contains(Keys.S))
                {
                    newPosition.Y += MoveSpeed; // Move the player downwards
                    if (CollisionManager.HandleCollision(new Rectangle((int)newPosition.X, (int)newPosition.Y, Rect.Width, Rect.Height), out isOnGround))
                    {
                        newPosition.Y = Pos.Y - moveBackOffset; // Move player back by 1 unit in the opposite direction
                    }
                }

                // Apply gravity to the player when moving right or left
                newPosition.Y += gravity;

                // Check collision with the updated position
                if (CollisionManager.HandleCollision(new Rectangle((int)newPosition.X, (int)newPosition.Y, Rect.Width, Rect.Height), out isOnGround))
                {
                    newPosition.Y = Pos.Y - moveBackOffset; // Move player back by 1 unit in the opposite direction
                    IsOnGround = true;
                }
                if (pressedKeys.Contains(Keys.Space) && IsOnGround)
                {
                    newPosition.Y -= 20f;
                }
            }
            else
            {
                // No keys are pressed
                // PlayerTextures = TileTextures.PlayerIdle;
                if (animation.CurrentAnimationName != "idle")
                {
                    // Play the "idle" animation
                    animation.PlayAnimation("idle");
                }
                // Apply gravity to the player
                newPosition.Y += gravity;

                // Check collision with the updated position
                if (CollisionManager.HandleCollision(new Rectangle((int)newPosition.X, (int)newPosition.Y, Rect.Width, Rect.Height), out isOnGround))
                {
                    newPosition.Y = Pos.Y - moveBackOffset; // Move player back by 1 unit in the opposite direction
                    IsOnGround = true;
                }
            }

            // Assign the new position back to the Pos property
            Pos = newPosition;

            // Update player position based on velocity
            PreviousPos = Pos;

            // Update the grounded state
            IsOnGround = isOnGround;

            foreach (Enemy e in Game1.Instance._enemies)
            {
                if (CollisionManager.HandleCollision(this, e))
                {
                    e.TakeDamage(100, Game1.Instance);
                }
            }
        }
    }
}