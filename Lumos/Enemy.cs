using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public float MovementSpeed { get; set; } = 2f;
        public Rectangle boundingBox { get; set; } = Rectangle.Empty;

        public Vector2 Position { get; set; }

        public Enemy(Texture2D texture, Vector2 position) : base(texture, position)
        {
            health = 100;
            maxHealth = 100;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            Destination = position;
        }

        public override void Update(GameTime gameTime, Vector2 cameraPos, Player player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += deltaTime;

            // Calculate the distance to move based on the elapsed time and movement speed
            float distanceToMove = MovementSpeed * deltaTime;
            Vector2 direction = Destination - position;
            if (elapsedTime > 0.01f)
            {
                position += direction * distanceToMove;
                elapsedTime = 0;
            }
            else
            {
                velocity.Y = 0;
            }

            //position += velocity;
            boundingBox = new Rectangle((int)(position.X - cameraPos.X), (int)(position.Y - cameraPos.Y), texture.Width, texture.Height);
            // Vector2 direction = Destination - position;
            if (Vector2.Distance(position, Destination) < 50f)
            {
                // Set a new random destination
                SetNewDestination();
            }

            // Calculate the rotation angle in radians
            rotationAngle = (float)Math.Atan2(direction.Y, direction.X);

            // Calculate the origin point for rotation (assuming the center of the texture)
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            base.Update(gameTime, cameraPos, player);
        }

        public bool IsVisible(Game1 game)
        {
            Rectangle screenBounds = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            return boundingBox.Intersects(screenBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.Draw(texture, position - cameraPos, null, Color.White, rotationAngle + 90, origin, 1.0f, SpriteEffects.None, 0);
            DrawHealthText(spriteBatch, cameraPos);
        }

        public void DrawHealthText(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.DrawString(TileTextures.MyFont, $"{health}/{maxHealth}", position - cameraPos + new Vector2(-20, -texture.Height), Color.Red);
        }

        private void SetNewDestination()
        {
            // Set a new random destination within a certain range
            Random random = new Random();
            int range = 100; // Adjust the range as needed
            Destination = position + new Vector2(random.Next(-range, range), random.Next(-range, range));
        }

        public void TakeDamage(int amount, Game1 game)
        {
            if (health > 0)
            {
                health -= amount;
            }
            if (health < 0)
            {
                game._enemies.Remove(this);
            }
            game._damageMessageList.Add(new DamageMessage(amount.ToString(), 2f, position, game));
        }
    }
}