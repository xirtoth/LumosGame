using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumos
{
    public abstract class Tool
    {
        public int Damage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Tool()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Use(GameTime gametime)
        {
        }
    }
}