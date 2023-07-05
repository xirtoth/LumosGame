using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Penumbra;
using System.Collections.Generic;

namespace Lumos
{
    public class Projectile
    {
        private const float Speed = 2.4f;
        private readonly Texture2D texture;
        public Vector2 Position { get; private set; }
        public float aliveTime = 0f;
        public float MaxAliveTime = 2f;
        private readonly Vector2 direction;

        public Light light = new PointLight
        {
            Scale = new Vector2(20f),
            ShadowType = ShadowType.Solid
        };

        public Projectile(float maxAliveTime, Vector2 position, Vector2 direction, Texture2D texture)
        {
            Position = position;
            this.direction = direction;

            this.direction *= Speed;
            this.texture = texture;
            Game1.Instance.penumbra.Lights.Add(light);
            light.Position = Position;
            light.Color = Color.BlueViolet;
            MaxAliveTime = maxAliveTime;
        }

        public void Update(GameTime gametime)
        {
            light.Position = Position;
            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds;
            aliveTime += deltaTime;
            Position += direction * Speed;
            if (aliveTime > MaxAliveTime)
            {
                Game1.Instance.penumbra.Lights.Remove(light);

                Game1.Instance._projectiles.Remove(this);
            }
            Rectangle collisionRect = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            List<Enemy> enemiesCopy = new List<Enemy>(Game1.Instance._enemies);
            foreach (Enemy e in enemiesCopy)
            {
                if (CollisionManager.HandleCollision(collisionRect, e))
                {
                    e.TakeDamage(100, Game1.Instance);
                    Game1.Instance.penumbra.Lights.Remove(light);

                    Game1.Instance._projectiles.Remove(this);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color dropColor = Color.FromNonPremultiplied(255, 255, 255, (byte)(0.2 * 255));
            spriteBatch.Draw(texture, Position, Color.White);
            spriteBatch.DrawLine(Game1.Instance._player.Pos - Game1.Instance._cameraPosition, Position, dropColor);
        }
    }
}