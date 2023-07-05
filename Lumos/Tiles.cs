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
            return new Tile(MapTiles.dirt, TileTextures.DirtTexture, false, true, false);
        }

        public static Tile CreateEmpty()
        {
            return new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false, false);
        }

        public static Tile CreateWater()
        {
            return new Tile(MapTiles.water, TileTextures.WaterTexture, true, true, false);
        }
    }
}