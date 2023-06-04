using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lumos
{
    public class DamageMessage
    {
        public string Message { get; set; }
        public float Duration { get; set; }
        public Vector2 Position { get; set; }

        public float PingPongValue { get; set; }

        private Color Color { get; set; }
        private float elapsedTime;
        private float destroyTimer;
        private float movedAmount;
        private Random rand = new Random();

        public DamageMessage(string message, float duration, Vector2 position, Game1 game)
        {
            Message = message;
            Duration = duration;
            Position = position;
            int red = rand.Next(256);
            int green = rand.Next(256);
            int blue = rand.Next(256);

            // Create a random color using the generated values
            Color randomColor = new Color(red, green, blue);
            Color = randomColor;
            game._damageMessageList.Add(this);
        }

        public void Update(GameTime gameTime, Game1 game)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += deltaTime;
            destroyTimer += deltaTime;
            if (elapsedTime >= 0.1f && movedAmount < 30f)
            {
                Position += new Vector2(0, -2f);
                movedAmount += 2f;
                elapsedTime = 0f;
            }
            if (destroyTimer > Duration)
            {
                game._damageMessageList.Remove(this);
            }
        }

        public void Draw(Game1 game)
        {
            //PingPongValue = 0f;
            float speed = 0.02f; // Adjust the speed of the ping-pong effect

            // Update the ping-pong value
            PingPongValue += speed;

            // Calculate the scaled and shifted value within the range 0.8 to 1.2
            float scale = MathHelper.Lerp(0.4f, 1.2f, (float)Math.Sin(PingPongValue));
            game._spriteBatch.DrawString(game._myFont, Message, Position - game._cameraPosition, Color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}