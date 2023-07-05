using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public enum Raintype
    {
        Water,
        Snow,
        Fire,
    }

    public class RainSystem
    {
        private readonly List<RainDrop> raindrops;
        private Random rand;
        public bool isActive;

        public RainSystem()
        {
            rand = new Random();
            raindrops = new List<RainDrop>();
            isActive = false;
        }

        private RainDrop CreateRandomRaindrop(Raintype type)
        {
            RainDrop raindrop = new RainDrop();
            raindrop.Position = new Vector2(rand.Next(Game1.Instance.GraphicsDevice.Viewport.Width), rand.Next(Game1.Instance.GraphicsDevice.Viewport.Height));
            // raindrop.Velocity = new Vector2(rand.Next(1, 400), rand.Next(100, 400));
            switch (type)
            {
                case Raintype.Water:
                    raindrop.Velocity = new Vector2(0, rand.Next(400, 800));
                    break;

                case Raintype.Snow:
                    raindrop.Velocity = new Vector2(rand.Next(-20, 20), rand.Next(50, 100));
                    break;

                default:
                    break;
            }
            //  raindrop.Velocity = new Vector2(rand.Next(-20, 20), rand.Next(50, 100));
            raindrop.Size = rand.Next(1, 4);
            raindrop.Transparency = rand.Next(50, 200) / 255f;
            raindrop.Raintype = type;
            return raindrop;
        }

        public void AddRaindrops(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (i % 2 == 0)
                {
                    raindrops.Add(CreateRandomRaindrop(Raintype.Snow));
                }
                else
                {
                    raindrops.Add(CreateRandomRaindrop(Raintype.Water));
                }
            }
        }

        public void EmptyRaindrops()
        {
            raindrops.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                foreach (var drop in raindrops)
                {
                    if (drop != null)
                    {
                        switch (drop.Raintype)
                        {
                            case (Raintype.Snow):
                                drop.Position += drop.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                drop.Velocity = new Vector2(rand.Next(-30, 30), rand.Next(100, 150));
                                if (drop.Position.Y >= Game1.Instance.GraphicsDevice.Viewport.Height)
                                {
                                    drop.Position = new Vector2(rand.Next(Game1.Instance.GraphicsDevice.Viewport.Width), -20);
                                    drop.Size = rand.Next(1, 4);
                                    drop.Transparency = rand.Next(50, 200) / 255f;
                                }
                                if (drop.Position.X >= Game1.Instance.GraphicsDevice.Viewport.Width)
                                {
                                    drop.Position = new Vector2(rand.Next(0, Game1.Instance.GraphicsDevice.Viewport.Width) + 100, drop.Position.Y);
                                }
                                break;

                            case (Raintype.Water):
                                drop.Position += drop.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                //drop.Velocity = new Vector2(0, rand.Next(100, 400));
                                if (drop.Position.Y >= Game1.Instance.GraphicsDevice.Viewport.Height)
                                {
                                    drop.Position = new Vector2(rand.Next(Game1.Instance.GraphicsDevice.Viewport.Width), -20);
                                    drop.Size = rand.Next(1, 4);
                                    drop.Transparency = rand.Next(50, 200) / 255f;
                                }
                                if (drop.Position.X >= Game1.Instance.GraphicsDevice.Viewport.Width)
                                {
                                    drop.Position = new Vector2(rand.Next(0, Game1.Instance.GraphicsDevice.Viewport.Width) + 100, drop.Position.Y);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                foreach (var drop in raindrops)
                {
                    //  Rectangle dropRect = new Rectangle((int)drop.Position.X, (int)drop.Position.Y, (int)drop.Size, (int)drop.Size);
                    Color dropColor = Color.FromNonPremultiplied(255, 255, 255, (byte)(drop.Transparency * 255));
                    // Color dropColor = Color.FromNonPremultiplied(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255), (byte)(drop.Transparency * 255));
                    // sb.DrawRectangle(dropRect, dropColor);

                    //sb.DrawLine(drop.Position, Vector2.Zero, dropColor, thickness: 10);

                    switch (drop.Raintype)
                    {
                        case (Raintype.Snow):
                            sb.DrawPoint(drop.Position.X, drop.Position.Y, dropColor, size: 4);

                            break;

                        case (Raintype.Water):
                            Rectangle dropRect = new Rectangle((int)drop.Position.X, (int)drop.Position.Y, (int)drop.Size, (int)drop.Size);
                            sb.DrawRectangle(dropRect, dropColor);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}