using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public class Tiles
    {
        public static Tile CreateMithril()
        {
            return new Tile(MapTiles.mithril, TileTextures.Mithril, true, false, false);
        }

        public static Tile CreateDirt()
        {
            return new Tile(MapTiles.mithril, TileTextures.DirtTexture, true, true, false);
        }

        // Add more factory methods or constructors for other tile types
    }
}