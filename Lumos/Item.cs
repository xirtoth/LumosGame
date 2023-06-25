using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using System;

namespace Lumos
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Texture2D Icon { get; set; }

        public Texture2D Texture { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle Rect { get; set; }

        private bool shouldUpdatePosition = true;

        private float AttractSpeed { get; set; } = 4f;

        private float AttractRange { get; set; } = 60f;

        public int Count { get; set; } = 1;

        public Light light = new PointLight
        {
            Scale = new Vector2(16f),
            ShadowType = ShadowType.Solid
        };

        public Item(string name, string description, Texture2D texture, Vector2 position)
        {
            Name = name;
            Description = description;
            Icon = texture;
            Texture = texture;
            Position = position;
            light.Color = Color.OrangeRed;
            light.Intensity = 1f;
            Game1.Instance.penumbra.Lights.Add(light);
            //Rect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scale = 0.5f; // Scale factor for 50% scaling

            // Calculate the scaled position
            Vector2 scaledPosition = Position - Game1.Instance._cameraPosition;

            // Calculate the scaled size
            Vector2 scaledSize = new Vector2(Texture.Width, Texture.Height) * scale;

            spriteBatch.Draw(Texture, scaledPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void Update(GameTime gameTime)
        {
            bool isGrounded;

            light.Position = Position - Game1.Instance._cameraPosition;
            AttractToPlayer();

            if (shouldUpdatePosition)
            {
                // Rect = new Rectangle((int)(Position.X - (int)Game1.Instance._cameraPosition.X), (int)(Position.Y - (int)Game1.Instance._cameraPosition.Y), Texture.Width, Texture.Height);
                Rect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width / 2, Texture.Height / 2);
                if (CollisionManager.HandleCollision(this, out isGrounded))
                {
                    Position = new Vector2(Position.X, Position.Y - 0.1f);
                    shouldUpdatePosition = false;
                }
                else
                {
                    Position = new Vector2(Position.X, Position.Y + 2f);
                    //shouldUpdatePosition = false;
                }
            }
            Rect = new Rectangle((int)(Position.X - (int)Game1.Instance._cameraPosition.X), (int)(Position.Y - (int)Game1.Instance._cameraPosition.Y), Texture.Width, Texture.Height);
            if (CollisionManager.HandleCollision(Game1.Instance._player, this))
            {
                Game1.Instance._player.Inventory.AddItem(this);
                Game1.Instance._damageMessageList.Add(new DamageMessage($"Collected {this.Name}", 3, Position + new Vector2(0, -20), Game1.Instance));
                Game1.Instance.penumbra.Lights.Remove(light);
                Game1.Instance._items.Remove(this);
            }
        }

        private void AttractToPlayer()
        {
            if (Vector2.Distance(Position, Game1.Instance._player.Pos) < AttractRange)
            {
                var direction = Vector2.Normalize(Game1.Instance._player.Pos - Position);
                Position += direction * AttractSpeed;
                if (Vector2.Distance(Position, Game1.Instance._player.Pos) < 1f)
                {
                    Game1.Instance._items.Add(this);
                    Game1.Instance._items.Remove(this);
                }
            }
        }

        public bool IsVisible()
        {
            int screenWidth = Game1.Instance.GraphicsDevice.Viewport.Width;
            int screenHeight = Game1.Instance.GraphicsDevice.Viewport.Height;
            Rectangle itemBounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Rectangle screenBounds = new Rectangle(0, 0, screenWidth, screenHeight);

            return screenBounds.Intersects(itemBounds);
        }
    }
}