using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

using System.Buffers.Text;

using System.Net.Mime;
using System.Reflection.Metadata;
using Lumos.Tools;

namespace Lumos
{
    public class Player
    {
        public Vector2 Pos { get; set; }

        public Vector2 PreviousPos { get; set; }
        public string Name { get; set; }
        public float previousBottom { get; set; }
        public MapTiles selectedTile { get; set; } = MapTiles.water;

        public float MoveSpeed { get; set; } = 4f;

        public Rectangle Rect { get; set; }

        public Rectangle HorizontalCollisionRect { get; set; }
        public Rectangle VerticalCollisionRect { get; set; }
        public Rectangle LocalBounds { get; set; }

        public Inventory Inventory { get; set; }

        public Texture2D PlayerTex;

        public Texture2D[] PlayerTextures;

        private Animation animation;

        public float velocityY { get; set; }
        public bool IsOnGround { get; internal set; }

        public Tool tool { get; set; }

        public bool InventoryToggled { get; internal set; }

        public float velocityX { get; internal set; }
        public bool isCollidingLeft { get; internal set; } = false;
        public bool isCollidingRight { get; internal set; } = false;

        private int facingDirection = 1;

        private float moveBackOffset = 0.01f;
        private float animationSpeedIdle = 2f;

        private float animationSpeedWalk = 4f;
        private bool inventoryToggleCooldown = false;
        private float inventoryToggleCooldownTimer = 0f;
        private float inventoryToggleCooldownDuration = 0.2f;

        public Player()
        { }

        public Player(Vector2 pos, string name, Texture2D playerTex)
        {
            tool = new Gun();
            Inventory = new Inventory();

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
            tool.Update(gameTime);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Game1.Instance.light.Position = Pos - Game1.Instance._cameraPosition;
            Game1.Instance.light2.Position = Pos - Game1.Instance._cameraPosition;
            if (InventoryToggled)
            {
                Inventory.UI.UpdateInventory(Inventory);
            }
            PreviousPos = Pos;
            animation.Update(gameTime);

            //float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity only if the player is not on the ground
            if (!IsOnGround)
            {
                //      velocityY += Gravity * deltaTime;
            }

            CheckInput(gameTime);

            Pos += new Vector2(velocityX * deltaTime, velocityY * deltaTime);

            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, PlayerTex.Width, PlayerTex.Height);
            isCollidingLeft = false;
            isCollidingRight = false;
            // IsOnGround = false;
        }

        private void CheckInput(GameTime gameTime)
        {
            HandleInventoryToggle(gameTime);
            HandlePlayerMovement(gameTime);
            HandlePlayerActions(gameTime);
        }

        private void HandleInventoryToggle(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                if (!inventoryToggleCooldown)
                {
                    InventoryToggled = !InventoryToggled;
                    inventoryToggleCooldown = true;
                    inventoryToggleCooldownTimer = 0f;
                }
            }

            if (inventoryToggleCooldown)
            {
                inventoryToggleCooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (inventoryToggleCooldownTimer >= inventoryToggleCooldownDuration)
                {
                    inventoryToggleCooldown = false;
                }
            }
        }

        private void HandlePlayerMovement(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            Vector2 newPosition = Pos;

            float moveSpeedX = MoveSpeed;
            float gravity = 0f;

            bool isOnGround = false;

            if (pressedKeys.Length > 0)
            {
                if (pressedKeys.Contains(Keys.D))
                {
                    // Handle player movement to the right
                    newPosition.X += MoveSpeed;
                    facingDirection = 1;
                    if (HandleCollisionWithNewPosition(newPosition, out isOnGround))
                    {
                        newPosition.X = Pos.X;
                    }
                    animation.PlayAnimation("walk");
                }
                else if (pressedKeys.Contains(Keys.A))
                {
                    // Handle player movement to the left
                    newPosition.X -= MoveSpeed;
                    facingDirection = -1;
                    if (HandleCollisionWithNewPosition(newPosition, out isOnGround))
                    {
                        newPosition.X = Pos.X + moveBackOffset;
                    }
                    animation.PlayAnimation("walk");
                }
                if (pressedKeys.Contains(Keys.S))
                {
                    // Handle player movement downwards
                    newPosition.Y += MoveSpeed;
                    if (HandleCollisionWithNewPosition(newPosition, out isOnGround))
                    {
                        newPosition.Y = Pos.Y;
                    }
                    animation.PlayAnimation("walk");
                }

                if (pressedKeys.Contains(Keys.W))
                {
                    // Handle player movement downwards
                    newPosition.Y -= MoveSpeed;
                    if (HandleCollisionWithNewPosition(newPosition, out isOnGround))
                    {
                        newPosition.Y = Pos.Y;
                    }
                    animation.PlayAnimation("walk");
                }
                // Apply gravity to the player when moving right or left
                //newPosition.Y += gravity;

                /*    if (HandleCollisionWithNewPosition(newPosition, out isOnGround))
                    {
                        newPosition.Y = Pos.Y;
                        IsOnGround = true;
                    }*/
                if (pressedKeys.Contains(Keys.Space) && IsOnGround)
                {
                    newPosition.Y -= 20f;
                }
            }
            else
            {
                // No keys are pressed
                animation.PlayAnimation("idle");
                newPosition.Y += gravity;
                if (HandleCollisionWithNewPosition(newPosition, out isOnGround))
                {
                    newPosition.Y = Pos.Y;
                    IsOnGround = true;
                }
            }

            if (pressedKeys.Contains(Keys.K))
            {
                if (!inventoryToggleCooldown)
                {
                    Game1.Instance.rainsystem.isActive = !Game1.Instance.rainsystem.isActive;
                    inventoryToggleCooldown = true;
                    inventoryToggleCooldownTimer = 0f;
                }
            }

            Pos = newPosition;
            PreviousPos = Pos;
            IsOnGround = isOnGround;
            int tileSize = 16; // Assuming each tile is 16x16 in size
            int radius = 24; // Radius of the circular area

            int centerX = (int)Pos.X / tileSize; // Calculate center X position in tile coordinates
            int centerY = (int)Pos.Y / tileSize; // Calculate center Y position in tile coordinates

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    int tileX = x;
                    int tileY = y;

                    // Calculate the distance between the tile and the center of the circular area
                    float distance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(tileX, tileY));

                    // Check if the tile falls within the circular area
                    if (distance <= radius && tileX >= 0 && tileX < Game1.Instance._map.MapData.GetLength(0) &&
                        tileY >= 0 && tileY < Game1.Instance._map.MapData.GetLength(1))
                    {
                        Game1.Instance._map.MapData[tileX, tileY].IsVisible = true;
                    }
                }
            }
        }

        private bool HandleCollisionWithNewPosition(Vector2 newPosition, out bool isOnGround)
        {
            Rectangle newRect = new Rectangle((int)newPosition.X, (int)newPosition.Y, Rect.Width, Rect.Height);
            bool collision = CollisionManager.HandleCollision(newRect, out isOnGround);
            return collision;
        }

        private void HandlePlayerActions(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                tool.Use(gameTime);
            }
        }
    }
}