using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entities
{
    public class FlowerEntity : Entity
    {
        public enum GrowState
        {
            Seed,
            Sappling,
            Grown
        }

        private Random rand = new Random();
        private float timeToGrow = 10;
        private float growTime = 0f;

        private int minGrowtime = 1;
        private int maxGrowtime = 5;

        public GrowState _GrowState { get; set; }

        public FlowerEntity(Vector2 position) : base(TileTextures.Mithril, position)
        {
            _GrowState = GrowState.Seed;
            texture = TileTextures.Flower[0];
            timeToGrow = rand.Next(minGrowtime, maxGrowtime);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            growTime += elapsedTime;

            if (growTime > timeToGrow)
            {
                growTime = 0f;
                switch (_GrowState)
                {
                    case GrowState.Seed:
                        _GrowState = GrowState.Sappling;
                        texture = TileTextures.Flower[1];
                        timeToGrow = rand.Next(minGrowtime, maxGrowtime);
                        break;

                    case GrowState.Sappling:
                        _GrowState = GrowState.Grown;
                        texture = TileTextures.Flower[2];
                        timeToGrow = rand.Next(minGrowtime, maxGrowtime);
                        break;

                    case GrowState.Grown:
                        _GrowState = GrowState.Seed;
                        texture = TileTextures.Flower[0];
                        timeToGrow += rand.Next(minGrowtime, maxGrowtime);
                        break;

                    default:
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos, GameTime gametime)
        {
            base.Draw(spriteBatch, cameraPos, gametime);
        }
    }
}