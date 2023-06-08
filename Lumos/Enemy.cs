using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public class Enemy : Entity
    {
        private float elapsedTime;
        private int health;
        private int maxHealth;
        private float rotationAngle;
        private Vector2 velocity;
        private Vector2 origin;
        public Vector2 Destination { get; set; }
        public float MovementSpeed { get; set; } = 10f;
        public Rectangle boundingBox { get; set; } = Rectangle.Empty;

        public Rectangle CollisionRect { get; set; }

        private Vector2 previousPosition;

        private Vector2 Position;

        public float animatonFps = 1f / 8;

        public Texture2D[] Textures { get; set; }

        public bool Animated { get; set; }

        private float animationTime = 0f;

        private int currentFrame;

        private bool Moving = false;
        private float movementTime = 1f;

        private Random rand = new Random();

        public Enemy(Texture2D texture, Vector2 position) : base(texture, position)
        {
            health = 100;
            maxHealth = 100;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            Destination = position;
            Animated = false;
        }

        public Enemy(Texture2D[] textures, Vector2 position) : base(textures[0], position)
        {
            health = 100;
            maxHealth = 100;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, textures[0].Width, textures[0].Height);
            Destination = position;
            Textures = textures;
            Animated = true;
            Position = position;
        }

        private void CheckCollision(Tile[,] map, Vector2 cameraPos)
        {
            int minX = Math.Max(0, (int)(boundingBox.Left / 16) - 1);
            int minY = Math.Max(0, (int)(boundingBox.Top / 16) - 1);
            int maxX = Math.Min(texture.Width - 1, (int)(boundingBox.Right / 16) + 1);
            int maxY = Math.Min(texture.Height - 1, (int)(boundingBox.Bottom / 16) + 1);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle tileRect = new Rectangle(x * 16, y * 16, 16, 16);

                    if (boundingBox.Intersects(tileRect))
                    {
                        // Handle collision with the tile
                        if (map[x, y].Collision)
                        {
                            // Position = new Vector2(-10, -10);
                            // Reset the enemy's position to their previous position
                            Position = previousPosition;
                        }
                        else if (map[x, y].MapTile == MapTiles.water)
                        {
                            // Example: Handle collision with water tile
                            // Do something when enemy collides with water
                        }
                        // Add more collision handling for other tile types if needed
                    }
                }
            }
            boundingBox = new Rectangle((int)(Position.X - cameraPos.X), (int)(Position.Y - cameraPos.Y), texture.Width, texture.Height);
        }

        public override void Update(GameTime gameTime, Vector2 cameraPos, Player player, Game1 game)
        {
            boundingBox = new Rectangle((int)(Position.X - cameraPos.X), (int)(Position.Y - cameraPos.Y), texture.Width, texture.Height);
            CollisionRect = new Rectangle((int)Position.X, (int)Position.Y, Textures[currentFrame].Width, Textures[currentFrame].Height);
            previousPosition = Position;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float gravity = 2f;
            bool isOnGround = false;
            Vector2 direction = Destination - position;
            if (Moving)
            {
                float distanceToMove = MovementSpeed * deltaTime;
                previousPosition = Position;

                Position = new Vector2(Position.X - distanceToMove, Position.Y);
                CollisionRect = new Rectangle((int)Position.X, (int)Position.Y, Textures[currentFrame].Width, Textures[currentFrame].Height);
                boundingBox = new Rectangle((int)(Position.X - cameraPos.X), (int)(Position.Y - cameraPos.Y), texture.Width, texture.Height);
                previousPosition.X = Position.X;

                // CheckCollision(game._map.MapData, cameraPos);
                Textures = TileTextures.Enemy1Walk;
            }
            //Position = new Vector2(Position.X, Position.Y + 0.1f);
            //  CheckCollision(game._map.MapData, cameraPos);
            Position = new Vector2(Position.X, Position.Y + gravity);
            if (CollisionManager.HandleCollision(CollisionRect, out isOnGround))
            {
                Position = previousPosition;
                isOnGround = true;
                CollisionRect = new Rectangle((int)Position.X, (int)Position.Y, Textures[currentFrame].Width, Textures[currentFrame].Height);
                boundingBox = new Rectangle((int)(Position.X - cameraPos.X), (int)(Position.Y - cameraPos.Y), texture.Width, texture.Height);
            }

            if (elapsedTime > movementTime)
            {
                Moving = !Moving;
                Textures = TileTextures.Enemy1Animated;
                //  CheckCollision(game._map.MapData, cameraPos);
                elapsedTime = 0;
                movementTime = rand.Next(1, 5);
            }

            if (Animated)
            {
                animationTime += deltaTime;
                if (animationTime > animatonFps)
                {
                    currentFrame = (currentFrame + 1) % Textures.Length;
                    animationTime = 0f;
                }
            }

            elapsedTime += deltaTime;

            //position += velocity;

            // Vector2 direction = Destination - position;
            /* if (Vector2.Distance(position, Destination) < 50f)
              {
                  // Set a new random destination
                  SetNewDestination();
              } */

            // Calculate the rotation angle in radians
            rotationAngle = (float)Math.Atan2(direction.Y, direction.X);

            // Calculate the origin point for rotation (assuming the center of the texture)
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public bool IsVisible(Game1 game)
        {
            Rectangle screenBounds = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            return boundingBox.Intersects(screenBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos, GameTime gameTime)
        {
            if (Animated)

            {
                // Game1.Instance.penumbra.BeginDraw();
                Color color2 = new Color(Color.White.R, Color.White.G, Color.White.B, (byte)128);
                spriteBatch.Draw(Textures[currentFrame], Position - cameraPos, null, color2);
            }
            else
            {
                spriteBatch.Draw(texture, position - cameraPos, null, Color.White, rotationAngle + 90, origin, 1.0f, SpriteEffects.None, 0);
            }
            // Game1.Instance.penumbra.Draw(gameTime);
            DrawHealthText(spriteBatch, position);
        }

        public void DrawHealthText(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.DrawString(TileTextures.MyFont, $"{health}/{maxHealth}", position - cameraPos, Color.Red);
        }

        private void SetNewDestination()
        {
            // Set a new random destination within a certain range
            Random random = new Random();
            int range = 300; // Adjust the range as needed
            Destination = position + new Vector2(random.Next(-range, range), random.Next(-range, range));
        }

        public void TakeDamage(int amount, Game1 game)
        {
            if (health > 0)
            {
                health -= amount;
            }
            if (health <= 0)
            {
                int lootAmount = rand.Next(1, 5);

                for (int i = 0; i < lootAmount; i++)
                {
                    int lootType = rand.Next(0, 2);
                    if (lootType == 0)
                    {
                        game._items.Add(new Item("apple", "apple", TileTextures.Apple, Position + new Vector2(rand.Next(-10, 10), rand.Next(-100, -2))));
                    }
                    else
                    {
                        game._items.Add(new Item("jorma", "jorma", TileTextures.WaterTexture, Position + new Vector2(rand.Next(-10, 10), rand.Next(-100, -2))));
                    }
                }
                game._enemies.Remove(this);
            }
            game._damageMessageList.Add(new DamageMessage(amount.ToString() + $" {health} health left", 2f, Position, game));
            // game._player.Pos = new Vector2(game._player.Pos.X - 5f, game._player.Pos.Y);
        }
    }
}