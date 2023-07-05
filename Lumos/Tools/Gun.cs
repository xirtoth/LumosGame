using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumos.Tools
{
    public class Gun : Tool
    {
        public float ShootDelay { get; set; } = 0.3f;
        private float shootTimer = 0;

        public Gun()
        {
        }

        public override void Update(GameTime gameTime)
        {
            shootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds; // Update the shoot timer
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Use(GameTime gameTime)
        {
            if (shootTimer < 0f)
            {
                Vector2 mousePosition = Mouse.GetState().Position.ToVector2();
                Vector2 playerPosition = Game1.Instance._player.Pos - Game1.Instance._cameraPosition;
                Vector2 direction = Vector2.Normalize(mousePosition - playerPosition);

                Game1.Instance._damageMessageList.Add(new DamageMessage("Shooting gun", 2f, Game1.Instance._player.Pos, Game1.Instance));
                Game1.Instance._projectiles.Add(new Projectile(2f, playerPosition, direction, TileTextures.Apple));
                shootTimer = ShootDelay;
            }
        }
    }
}