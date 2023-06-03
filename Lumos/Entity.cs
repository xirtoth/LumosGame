using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public abstract class Entity
    {
        protected Texture2D texture;
        protected Vector2 position;

        public Rectangle Bounds => new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

        public Entity(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public virtual void Update(GameTime gameTime, Vector2 cameraPos, Player player, Game1 game)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.Draw(texture, position - cameraPos, Color.White);
        }
    }
}