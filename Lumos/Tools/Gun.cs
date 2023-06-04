using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

namespace Lumos.Tools
{
    public class Gun : Tool
    {
        public Gun()
        { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Use()
        {
            Vector2 mousePosition = Mouse.GetState().Position.ToVector2();
            Vector2 playerPosition = Game1.Instance._player.Pos - Game1.Instance._cameraPosition;
            Vector2 direction = Vector2.Normalize(mousePosition - playerPosition);

            Game1.Instance._damageMessageList.Add(new DamageMessage("Shooting gun", 2, playerPosition, Game1.Instance));
            Game1.Instance._projectiles.Add(new Projectile(playerPosition, direction, TileTextures.WaterTexture));
        }
    }
}