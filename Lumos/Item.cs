﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public int Count { get; set; } = 1;

        public Item(string name, string description, Texture2D texture, Vector2 position)
        {
            Name = name;
            Description = description;
            Icon = texture;
            Texture = texture;
            Position = position;
            //Rect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position - Game1.Instance._cameraPosition, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            bool isGrounded;

            if (shouldUpdatePosition)
            {
                // Rect = new Rectangle((int)(Position.X - (int)Game1.Instance._cameraPosition.X), (int)(Position.Y - (int)Game1.Instance._cameraPosition.Y), Texture.Width, Texture.Height);
                Rect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
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

                Game1.Instance._items.Remove(this);
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