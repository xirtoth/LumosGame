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
        public Rectangle boundingBox { get; set; } = Rectangle.Empty;

        public Enemy(Texture2D texture, Vector2 position) : base(texture, position)
        {
            health = 100;
            maxHealth = 100;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public override void Update(GameTime gameTime, Vector2 cameraPos)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += deltaTime;
            if (elapsedTime > 0.2f)
            {
                position += new Vector2(0, -0.2f);
                elapsedTime = 0;
            }
            boundingBox = new Rectangle((int)(position.X - cameraPos.X), (int)(position.Y - cameraPos.Y), texture.Width, texture.Height);
            base.Update(gameTime, cameraPos);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            base.Draw(spriteBatch, cameraPos);
            DrawHealthText(spriteBatch, cameraPos);
        }

        public void DrawHealthText(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.DrawString(TileTextures.MyFont, $"{health}/{maxHealth}", position - cameraPos + new Vector2(-20, -texture.Height), Color.Red);
        }

        public void TakeDamage(int amount, Game1 game)
        {
            if (health > 0)
            {
                health -= amount;
            }
            game._damageMessageList.Add(new DamageMessage(amount.ToString(), 2f, position, game));
        }
    }
}