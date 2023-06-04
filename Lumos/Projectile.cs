using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace Lumos
{
    public class Projectile
    {
        private const float Speed = 5f;
        private readonly Texture2D texture;
        public Vector2 Position { get; private set; }
        public float aliveTime = 0f;
        public float maxAliveTime = 2f;
        private readonly Vector2 direction;

        public Light light = new PointLight
        {
            Scale = new Vector2(200f), // Range of the light source (how far the light will travel)
            ShadowType = ShadowType.Solid // Will not lit hulls themselves
        };

        public Projectile(Vector2 position, Vector2 direction, Texture2D texture)
        {
            Position = position;
            this.direction = direction;
            this.texture = texture;
            Game1.Instance.penumbra.Lights.Add(light);
            light.Position = Position;
            light.Color = Color.BlueViolet;
        }

        public void Update(GameTime gametime)
        {
            light.Position = Position;
            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds;
            aliveTime += deltaTime;
            Position += direction * Speed;
            if (aliveTime > maxAliveTime)
            {
                Game1.Instance.penumbra.Lights.Remove(light);
                Game1.Instance._projectiles.Remove(this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }
    }
}