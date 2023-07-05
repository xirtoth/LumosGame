using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
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

        public string Name { get; private set; }
        public Vector2 Destination { get; set; }
        public float MovementSpeed { get; set; }
        public Rectangle boundingBox { get; set; } = Rectangle.Empty;

        public Rectangle CollisionRect { get; set; }

        private Vector2 previousPosition;

        private Vector2 Position;

        public float animatonFps = 1f / 8;

        public Texture2D[] Textures { get; set; }

        public bool Animated { get; set; }

        private float animationTime = 0f;

        private PathFinding PathFinding { get; set; }

        private int currentFrame;

        private bool canChangeDirection = false;

        private float maxDirectionChangeTime = 2f;
        private float directionChangeTime = 2f;

        private float pathUpdateTime = 0f;
        private float maxPathUpdateTime;

        private bool Moving = false;
        private float movementTime = 1f;
        private Color EnemyColor;
        private readonly object pathLock = new object();

        private bool isOnGround = false;

        private int movementDir = 0;

        private Random rand = new Random();

        private bool hasPath = false;
        private List<(int, int)> path = new List<(int, int)>();

        public Enemy(Texture2D texture, Vector2 position) : base(texture, position)
        {
            health = 100;
            maxHealth = 100;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            Destination = position;
            Animated = false;
            Position = position;
            MovementSpeed = rand.Next(100, 250);
            Name = "Slime";
            EnemyColor = new Color(rand.Next(1, 256), rand.Next(1, 256), rand.Next(1, 256));
            PathFinding = new PathFinding(Game1.Instance._map.MapData);
            /* float minValue = 1f;
             float maxValue = 2f;
             float maxPathUpdateTime = (float)rand.NextDouble() * (maxValue - minValue) + minValue;*/
            maxPathUpdateTime = 1f;
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
            MovementSpeed = rand.Next(100, 250);
            Name = "Slime";
            EnemyColor = new Color(rand.Next(1, 256), rand.Next(1, 256), rand.Next(1, 256));
            PathFinding = new PathFinding(Game1.Instance._map.MapData);
            UpdatePath(Game1.Instance._player.Pos);
            /* float minValue = 1f;
             float maxValue = 2f;
             float maxPathUpdateTime = (float)rand.NextDouble() * (maxValue - minValue) + minValue;*/
            maxPathUpdateTime = 1f;
        }

        private async void UpdatePath(Vector2 playerPosition)
        {
            int startX = (int)Position.X / 16;
            int startY = (int)Position.Y / 16;
            int endX = (int)playerPosition.X / 16;
            int endY = (int)playerPosition.Y / 16;
            if (Vector2.Distance(Position, playerPosition) < 4000f)
            {
                // Run the pathfinding operation asynchronously
                path = await Task.Run(() => PathFinding.FindPath((startX, startY), (endX, endY)));
            }

            // Pathfinding operation completed, update the path in the game
            //  UpdatePathInGame();
        }

        public override void Update(GameTime gameTime, Vector2 cameraPos, Player player, Game1 game)
        {
            // Update bounding box and collision rectangle
            boundingBox = new Rectangle((int)(Position.X - cameraPos.X), (int)(Position.Y - cameraPos.Y), texture.Width, texture.Height);
            CollisionRect = new Rectangle((int)Position.X, (int)Position.Y, Textures[currentFrame].Width, Textures[currentFrame].Height);

            // Define left collision rectangle
            Rectangle leftRect = new Rectangle((int)Position.X - 2, (int)Position.Y + 8, Textures[currentFrame].Width - 20, Textures[currentFrame].Height - 20);
            Rectangle rightRect = new Rectangle((int)Position.X + Textures[currentFrame].Width + 2, (int)Position.Y + 8, Textures[currentFrame].Width - 20, Textures[currentFrame].Height - 20);
            //Rectangle rightRect = new Rectangle((int)Position.X + 2, (int)Position.Y + 8, Textures[currentFrame].Width - 20, Textures[currentFrame].Height - 20);

            // Store previous position and calculate delta time
            previousPosition = Position;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            pathUpdateTime += deltaTime;
            if (pathUpdateTime > maxPathUpdateTime)
            {
                UpdatePath(player.Pos);
                pathUpdateTime = 0f;
            }

            // Update direction change timer and gravity
            directionChangeTime -= deltaTime;
            float gravity = 2f;

            // Calculate movement direction
            // Vector2 direction = Destination - position;

            MoveEnemy(cameraPos, deltaTime);

            // Update animation and elapsed time
            if (elapsedTime > movementTime)
            {
                Moving = !Moving;
                Textures = TileTextures.Enemy1Animated;
                elapsedTime = 0;
                movementTime = rand.Next(0, 2);
                //  movementTime = 0;
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

            // Calculate rotation angle and origin

            //DebugDrawRectangle(leftRect, Color.Red);
        }

        private void MoveEnemy(Vector2 cameraPos, float deltaTime)
        {
            //      if (Moving)
            //      {
            if (directionChangeTime < 0)
            {
                canChangeDirection = true;
                directionChangeTime = maxDirectionChangeTime;
            }

            float distanceToMove = MovementSpeed * deltaTime;
            previousPosition = Position;

            if (path != null)
            {
                if (path.Count > 0)
                {
                    // Get the next path node
                    var nextNode = path[0];
                    var nextPosition = new Vector2(nextNode.Item1, nextNode.Item2);

                    // Calculate the direction towards the next node

                    if (new Vector2((int)Position.X / 16, (int)Position.Y / 16) != nextPosition)
                    {
                        int x = (int)Position.X / 16;
                        int y = (int)Position.Y / 16;
                        Vector2 direction = nextPosition - new Vector2(x, y);
                        direction.Normalize();

                        Position += direction * distanceToMove;
                        // Rest of the code...
                    }
                    else
                    {
                        // Remove the reached node from the path
                        if (path.Count > 0)
                            path.RemoveAt(0);
                    }

                    //  Vector2 direction = new Vector2(x, y) - nextPosition;
                    //  direction.Normalize();
                    //   Position += direction * distanceToMove;

                    // Move towards the next node

                    CollisionRect = new Rectangle((int)Position.X, (int)Position.Y, Textures[currentFrame].Width, Textures[currentFrame].Height);
                    boundingBox = new Rectangle((int)(Position.X - cameraPos.X), (int)(Position.Y - cameraPos.Y), texture.Width, texture.Height);

                    // Check if the enemy has reached the next node
                    if (Vector2.Distance(Position / 16, nextPosition) < distanceToMove)
                    {
                        // Remove the reached node from the path
                        if (path.Count > 0)
                            path.RemoveAt(0);
                    }
                }
            }

            // Update animation and elapsed time
            if (elapsedTime > movementTime)
            {
                Moving = !Moving;
                Textures = TileTextures.Enemy1Animated;
                elapsedTime = 0;
                movementTime = rand.Next(0, 200);
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

            // Calculate rotation angle and origin
            Vector2 directionToPlayer = Game1.Instance._player.Pos - Position;
            rotationAngle = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            //}
        }

        public bool IsVisible(Game1 game)
        {
            Rectangle screenBounds = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            return boundingBox.Intersects(screenBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos, GameTime gameTime)
        {
            spriteBatch.Draw(Textures[currentFrame], Position - cameraPos, null, EnemyColor);
            Vector2 oldPos = Vector2.Zero;
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var startPos = new Vector2((path[i].Item1 * 16) - cameraPos.X, (path[i].Item2 * 16) - cameraPos.Y);
                    var endPos = new Vector2((path[i + 1].Item1 * 16) - cameraPos.X, (path[i + 1].Item2 * 16) - cameraPos.Y);
                    spriteBatch.DrawLine(startPos, endPos, Color.Red);
                }
            }

            DrawHealthText(spriteBatch, position);
        }

        public void DrawHealthText(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.DrawString(TileTextures.MyFont, $"{health}/{maxHealth}", position - cameraPos, Color.Red);
        }

        private void DebugDrawRectangle(Rectangle rectangle, Color color)
        {
            Texture2D debugTexture = new Texture2D(Game1.Instance.GraphicsDevice, 1, 1);
            debugTexture.SetData(new[] { color });

            Game1.Instance._spriteBatch.Begin();
            Game1.Instance._spriteBatch.Draw(debugTexture, new Rectangle(rectangle.X - (int)Game1.Instance._cameraPosition.X, rectangle.Y - (int)Game1.Instance._cameraPosition.Y, rectangle.Width, rectangle.Height), color);
            Game1.Instance._spriteBatch.End();
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