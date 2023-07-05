using Newtonsoft.Json;
using System.IO;

namespace Lumos
{
    public static class SaveData
    {
        public static SaveTile[,] saveTiles;

        public static void WriteMapData(Tile[,] tiles)
        {
            saveTiles = new SaveTile[tiles.GetLength(0), tiles.GetLength(1)];

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    saveTiles[x, y] = new SaveTile(tiles[x, y].IsVisible, tiles[x, y].MapTile);
                }
            }
            var jsondata = JsonConvert.SerializeObject(saveTiles, formatting: Formatting.None);
            File.WriteAllText("mapdata.json", jsondata);
        }

        public static Tile[,] LoadMapData()
        {
            string json = File.ReadAllText("mapdata.json");
            var test = JsonConvert.DeserializeObject<SaveTile[,]>(json);

            Tile[,] loadTiles = new Tile[test.GetLength(0), test.GetLength(1)];
            for (int x = 0; x < test.GetLength(0); x++)
            {
                for (int y = 0; y < test.GetLength(1); y++)
                {
                    switch (test[x, y].MapTiles)
                    {
                        case MapTiles.dirt:
                            loadTiles[x, y] = Tiles.CreateDirt();
                            break;

                        case MapTiles.mithril:
                            loadTiles[x, y] = Tiles.CreateMithril();
                            break;

                        case MapTiles.water:
                            loadTiles[x, y] = Tiles.CreateWater();
                            break;

                        default:
                            loadTiles[x, y] = Tiles.CreateEmpty();
                            break;
                    }
                    loadTiles[x, y].IsVisible = test[x, y].Visible;
                }
            }

            return loadTiles;
        }
    }

    public class SaveTile
    {
        public bool Visible;
        public MapTiles MapTiles;

        public SaveTile(bool visible, MapTiles mapTiles)
        {
            Visible = visible;
            this.MapTiles = mapTiles;
        }
    }
}