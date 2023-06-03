using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public static class TileTextures
    {
        public static Texture2D DirtTexture { get; set; }
        public static Texture2D WaterTexture { get; set; }

        public static Texture2D DirtTopTexture { get; set; }

        public static Texture2D GrassTop { get; set; }

        public static Texture2D EmptyTexture { get; set; }

        public static SpriteFont MyFont { get; set; }

        public static Texture2D[] Enemy1Animated { get; set; }

        public static Texture2D[] Enemy1Walk { get; set; }

        public static Texture2D[] PlayerIdle { get; set; }

        public static Texture2D[] PlayerWalk { get; set; }

        public static void LoadContent(ContentManager gm)
        {
            Enemy1Animated = new Texture2D[4];
            Enemy1Walk = new Texture2D[4];
            PlayerIdle = new Texture2D[2];
            PlayerWalk = new Texture2D[4];
            PlayerWalk[0] = gm.Load<Texture2D>("player_02");
            PlayerWalk[1] = gm.Load<Texture2D>("player_03");
            PlayerWalk[2] = gm.Load<Texture2D>("player_04");
            PlayerWalk[3] = gm.Load<Texture2D>("player_05");
            PlayerIdle[0] = gm.Load<Texture2D>("player_00");
            PlayerIdle[1] = gm.Load<Texture2D>("player_01");
            Enemy1Animated[0] = gm.Load<Texture2D>("slime-idle-0");
            Enemy1Animated[1] = gm.Load<Texture2D>("slime-idle-1");
            Enemy1Animated[2] = gm.Load<Texture2D>("slime-idle-2");
            Enemy1Animated[3] = gm.Load<Texture2D>("slime-idle-3");
            Enemy1Walk[0] = gm.Load<Texture2D>("slime-move-0");
            Enemy1Walk[1] = gm.Load<Texture2D>("slime-move-1");
            Enemy1Walk[2] = gm.Load<Texture2D>("slime-move-2");
            Enemy1Walk[3] = gm.Load<Texture2D>("slime-move-3");
            TileTextures.WaterTexture = gm.Load<Texture2D>("water");
            TileTextures.DirtTexture = gm.Load<Texture2D>("dirt");
            TileTextures.DirtTopTexture = gm.Load<Texture2D>("dirttop");
            TileTextures.EmptyTexture = gm.Load<Texture2D>("empty");
            TileTextures.GrassTop = gm.Load<Texture2D>("grasstop");
            TileTextures.MyFont = gm.Load<SpriteFont>("MyFont");
        }
    }
}