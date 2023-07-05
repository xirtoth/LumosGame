using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lumos
{
    public enum MapTiles
    {
        dirt,
        dirtTop,
        water,
        empty,
        grassTop,
        test,
        mithril
    }

    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private float elapsedTime = 0f;

        public Texture2D Texture { get; set; }

        public Tile[,] MapData;

        private int tileWidth = 16, tileHeight = 16;
        private Random random = new Random();
        private bool isUpdatingTiles = false;
        private ManualResetEvent threadSync = new ManualResetEvent(true);

        public Map()
        { }

        public Map(int width, int height, Texture2D tex)
        {
            Width = width;
            Height = height;
            Texture = tex;
            MapData = new Tile[Width, Height];
        }

        public void GenerateMap()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    /*     if (y < 20 && y > 18)
                         {
                             if (random.Next(0, 2) == 1)
                             {
                                 MapData[x, y] = new Tile(MapTiles.dirt, TileTextures.DirtTexture, true, false);
                             }
                         }*/
                    /* else
                     {
                         var rand = random.Next(0, 2);
                         if (rand == 0)
                         {
                             MapData[x, y] = MapTiles.dirt;
                         }
                         else
                         {
                             MapData[x, y] = MapTiles.empty;
                         }
                     }*/
                    /* else if (y > 18 && y < 24)
                     {
                         MapData[x, y] = new Tile(MapTiles.dirt, TileTextures.DirtTexture, false, false, true);
                     }
                     else
                     {
                         MapData[x, y] = Tiles.CreateDirt();
                     } */
                    MapData[x, y] = Tiles.CreateDirt();
                }
            }

            /*  for (int x = 0; x < Width / 2 + Height / 2; x++)
              {
                  // Generate caves using random walk
                  // int startX = random.Next(10, Width - 10);
                  int startX = random.Next(10, Width);
                  int startY = random.Next(25, Height - 10);
                  int caveSize = random.Next(10, 100);

                  RandomWalk(startX, startY, caveSize, new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false, false));
              } */
            /*  for (int x = 0; x < 10000; x++)
              {
                  // Generate caves using random walk
                  // int startX = random.Next(10, Width - 10);
                  int startX = random.Next(10, Width);
                  int startY = random.Next(25, Height - 10);
                  int caveSize = random.Next(10, 100);

                  RandomWalk(startX, startY, caveSize, new Tile(MapTiles.water, TileTextures.WaterTexture, false, false, false));
              }*/
            int numWaterAreas = Game1.Instance._map.Width / 10; // Number of circular water areas to create
            int minRadius = 5; // Minimum radius of each water area
            int maxRadius = 12; // Maximum radius of each water area

            for (int i = 0; i < numWaterAreas; i++)
            {
                CreateWaterArea(minRadius, maxRadius);
            }

            //CreateRoads(10);

            /*    for (int x = 0; x < 10000; x++)
                {
                    // Generate caves using random walk
                    // int startX = random.Next(10, Width - 10);
                    int startX = random.Next(10, Width);
                    int startY = random.Next(25, Height - 10);
                    int caveSize = random.Next(10, 200);
                    RandomWalk(startX, startY, caveSize, Tiles.CreateMithril());
                } */

            /*int waterCount = 0;
            while (waterCount < 100)
            {
                int x = random.Next(Width);
                int y = random.Next(Height);

                if (MapData[x, y] == new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false))
                {
                    MapData[x, y] = new Tile(MapTiles.water, TileTextures.WaterTexture, false, false);
                    waterCount++;
                }
            }*/

            //Task.Run(UpdateTiles);
        }

        public void CreateRoads(int amount)
        {
            threadSync.Reset(); // Reset the event to block the new thread initially

            Thread thread = new Thread(() =>
            {
                for (int i = 0; i < 1; i++)
                {
                    // Choose random starting and ending points for the road
                    //Tile startTile = GetRandomNonWaterTile();
                    Tile startTile = MapData[(int)Game1.Instance._player.Pos.X / 16, (int)Game1.Instance._player.Pos.Y / 16];
                    Tile endTile = GetRandomNonWaterTile(startTile, 40);

                    // Find a path between the starting and ending points, avoiding water
                    List<Tile> roadTiles = FindPath(startTile, endTile);

                    // Mark the road tiles as road tiles (e.g., create instances of RoadTile class)
                    foreach (var tile in roadTiles)
                    {
                        int x = GetTileIndex(tile).Item1;
                        int y = GetTileIndex(tile).Item2;

                        if (IsValidTile(x, y))
                        {
                            MapData[x, y] = Tiles.CreateMithril();
                        }
                        else
                        {
                            // Handle out-of-bounds tiles accordingly (e.g., skip or terminate the path)
                        }
                    }
                }

                threadSync.Set(); // Signal that the thread has finished
            });

            thread.Start();
        }

        public void WaitForRoadCreation()
        {
            threadSync.WaitOne(); // Wait until the thread has finished
        }

        /// <summary>
        /// modifies map to have random caves
        /// </summary>
        /// <param name="startX">Start position x</param>
        /// <param name="startY">Start position y</param>
        /// <param name="caveSize">Size of cave</param>
        private void CreateWaterArea(int minRadius, int maxRadius)
        {
            int centerX = random.Next(maxRadius, Width - maxRadius); // Random center X within valid range
            int centerY = random.Next(maxRadius, Height - maxRadius); // Random center Y within valid range
            int radiusX = random.Next(minRadius, maxRadius + 1); // Random X-axis radius
            int radiusY = random.Next(minRadius, maxRadius + 1); // Random Y-axis radius

            // Apply additional randomness to the radii
            float radiusVariation = 0.9f; // Adjust the variation factor to control the level of randomness

            radiusX = ApplyRadiusVariation(radiusX, radiusVariation);
            radiusY = ApplyRadiusVariation(radiusY, radiusVariation);

            for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
                {
                    if (IsWithinEllipticalArea(x, y, centerX, centerY, radiusX, radiusY) && IsWithinMapBounds(x, y))
                    {
                        MapData[x, y] = new Tile(MapTiles.water, TileTextures.WaterTexture, false, false, false);
                    }
                }
            }
        }

        private void GenerateWater(int centerX, int centerY, int radiusX, int radiusY)
        {
            int startX = Math.Max(centerX - radiusX, 0);
            int startY = Math.Max(centerY - radiusY, 0);
            int endX = Math.Min(centerX + radiusX, Width - 1);
            int endY = Math.Min(centerY + radiusY, Height - 1);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (IsWithinEllipticalArea(x, y, centerX, centerY, radiusX, radiusY))
                    {
                        MapData[x, y] = Tiles.CreateWater();
                    }
                }
            }
        }

        private int ApplyRadiusVariation(int radius, float variationFactor)
        {
            float variation = radius * variationFactor;
            return radius + random.Next((int)-variation, (int)variation + 1);
        }

        private bool IsWithinEllipticalArea(int x, int y, int centerX, int centerY, int radiusX, int radiusY)
        {
            float radiusXVariation = 0.3f; // Adjust the variation factor to control the level of X-axis radius randomness
            float radiusYVariation = 0.3f; // Adjust the variation factor to control the level of Y-axis radius randomness

            // Calculate the adjusted radii with random variations
            int adjustedRadiusX = ApplyRadiusVariation(radiusX, radiusXVariation);
            int adjustedRadiusY = ApplyRadiusVariation(radiusY, radiusYVariation);

            // Calculate the squared distances from the current position to the center
            float dx = (x - centerX) / (float)adjustedRadiusX;
            float dy = (y - centerY) / (float)adjustedRadiusY;

            return (dx * dx) + (dy * dy) <= 1.0f;
        }

        private bool IsWithinCircularArea(int x, int y, int centerX, int centerY, int radius)
        {
            int deltaX = x - centerX;
            int deltaY = y - centerY;

            return (deltaX * deltaX) + (deltaY * deltaY) <= (radius * radius);
        }

        private bool IsWithinMapBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private void RandomWalk(int startX, int startY, int caveSize, Tile tile)
        {
            int currentX = startX;
            int currentY = startY;

            while (caveSize > 0)
            {
                if (MapData[currentX, currentY].MapTile == MapTiles.dirt)
                {
                    MapData[currentX, currentY] = tile;
                }

                // Move in a random direction
                int direction = random.Next(4);
                switch (direction)
                {
                    case 0: // Up
                        currentY--;
                        break;

                    case 1: // Down
                        currentY++;
                        break;

                    case 2: // Left
                        currentX--;
                        break;

                    case 3: // Right
                        currentX++;
                        break;
                }

                // Check if the new position is within the map bounds
                if (currentX < 1 || currentX >= Width - 1 || currentY < 1 || currentY >= Height - 1)
                {
                    break; // Stop if we reach the edge of the map
                }

                caveSize--;
            }
        }

        public void Update(Player player)
        {
            /*   if (!isUpdatingTiles)
               {
                   isUpdatingTiles = true;
                   Task.Run(() => UpdateTiles());
               }*/
            /*   int minX = Math.Max(0, (int)(player.Pos.X / tileWidth) - 1);
               int minY = Math.Max(0, (int)(player.Pos.Y / tileHeight) - 1);
               int maxX = Math.Min(Width - 1, (int)((player.Pos.X + player.Rect.Width) / tileWidth) + 1);
               int maxY = Math.Min(Height - 1, (int)((player.Pos.Y + player.Rect.Height) / tileHeight) + 1);
               Rectangle playerRect = player.Rect;

               for (int x = minX; x <= maxX; x++)
               {
                   for (int y = minY; y <= maxY; y++)
                   {
                       Rectangle tileRect = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);

                       if (playerRect.Intersects(tileRect))
                       {
                           // Handle collision with the tile
                           if (MapData[x, y].Collision == true)
                           {
                               // Example: Prevent player from passing through the dirt tile
                               // Reset the player's position to their previous position
                               player.Pos = player.PreviousPos;
                           }
                           else if (MapData[x, y].MapTile == MapTiles.water)
                           {
                               // Example: Handle collision with water tile
                               // Do something when player collides with water
                           }
                           // Add more collision handling for other tile types if needed
                       }
                   }
               } */
        }

        public void DrawMap(SpriteBatch spriteBatch, Player player, Microsoft.Xna.Framework.Vector2 cameraPos, Viewport viewport, GraphicsDevice gd, GameTime gameTime)
        {
            //Color waterColor = Color.White;
            //Color startColor = Color.White;
            //Color endColor = Color.DarkBlue;
            // elapsedTime += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            int startX = (int)(cameraPos.X / tileWidth) - 1;
            int startY = (int)(cameraPos.Y / tileHeight) - 1;
            int endX = (int)((cameraPos.X + viewport.Width) / tileWidth) + 1;
            int endY = (int)((cameraPos.Y + viewport.Height) / tileHeight) + 1;
            int minX = Math.Max(startX, (int)(player.Pos.X / tileWidth) - 1);
            int minY = Math.Max(startY, (int)(player.Pos.Y / tileHeight) - 1);
            int maxX = Math.Min(endX, (int)((player.Pos.X + player.Rect.Width) / tileWidth) + 1);
            int maxY = Math.Min(endY, (int)((player.Pos.Y + player.Rect.Height) / tileHeight) + 1);
            Vector2 initialDrawPosition = new Vector2(startX * tileWidth - cameraPos.X, startY * tileHeight - cameraPos.Y);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    // Calculate the position to draw the tile based on camera position
                    // Vector2 tilePosition = initialDrawPosition + new Vector2((x - startX) * tileWidth, (y - startY) * tileHeight);
                    Vector2 tilePosition = initialDrawPosition + new Vector2((x - startX) * tileWidth, (y - startY) * tileHeight);

                    // Check if the tile position is within the map bounds
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                    {
                        if (MapData[x, y].MapTile == MapTiles.dirtTop)
                        {
                            if (y < Height)
                            {
                                if (MapData[x, y + 1].MapTile != MapTiles.dirt)
                                {
                                    MapData[x, y] = new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false, false);
                                }
                            }
                        }

                        if (MapData[x, y].IsVisible)
                        {
                            spriteBatch.Draw(MapData[x, y].Texture, tilePosition, Color.White);
                        }
                        /*   else
                           {
                               //  spriteBatch.Draw(MapData[x, y].Texture, tilePosition, Color.Black);
                               //  spriteBatch.Draw(TileTextures.EmptyTexture, tilePosition, Color.White);
                           } */
                    }
                }
            }
        }

        public void SaveMapToFiles()
        {
            SaveData.WriteMapData(MapData);
        }

        public void LoadMapFromFile(string filePath)
        {
            MapData = SaveData.LoadMapData();
        }

        public List<Tile> FindPath(Tile startTile, Tile endTile)
        {
            int startX = -1;
            int startY = -1;
            int endX = -1;
            int endY = -1;

            // Find the indices of the start and end tiles in the MapData array
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (MapData[x, y] == startTile)
                    {
                        startX = x;
                        startY = y;
                    }
                    else if (MapData[x, y] == endTile)
                    {
                        endX = x;
                        endY = y;
                    }
                }
            }

            // Perform A* pathfinding
            List<Tile> path = new List<Tile>();

            // Create open and closed lists
            List<Tile> openList = new List<Tile>();
            List<Tile> closedList = new List<Tile>();

            // Add the start tile to the open list
            openList.Add(startTile);

            // Create dictionaries to store the cost and parent of each tile
            Dictionary<Tile, int> gCosts = new Dictionary<Tile, int>();
            Dictionary<Tile, int> hCosts = new Dictionary<Tile, int>();
            Dictionary<Tile, Tile> parents = new Dictionary<Tile, Tile>();

            // Initialize the costs for the start tile
            gCosts[startTile] = 0;
            hCosts[startTile] = CalculateHeuristic(startTile, endTile);

            while (openList.Count > 0)
            {
                // Get the tile with the lowest fCost from the open list
                Tile currentTile = openList.OrderBy(t => gCosts[t] + hCosts[t]).First();

                // If the current tile is the end tile, we have found the path
                if (currentTile == endTile)
                {
                    // Reconstruct the path
                    Tile pathTile = currentTile;
                    while (pathTile != startTile)
                    {
                        path.Insert(0, pathTile);
                        pathTile = parents[pathTile];
                    }
                    path.Insert(0, startTile);
                    break;
                }

                // Move the current tile from the open list to the closed list
                openList.Remove(currentTile);
                closedList.Add(currentTile);

                // Get the neighboring tiles
                List<Tile> neighbors = GetNeighbors(currentTile);

                foreach (Tile neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    int tentativeGCost = gCosts[currentTile] + CalculateMovementCost(currentTile, neighbor);

                    if (!openList.Contains(neighbor) || tentativeGCost < gCosts[neighbor])
                    {
                        parents[neighbor] = currentTile;
                        gCosts[neighbor] = tentativeGCost;
                        hCosts[neighbor] = CalculateHeuristic(neighbor, endTile);

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            return path;
        }

        private static List<Tile> ReconstructPath(Dictionary<Tile, Tile> tileParents, Tile currentTile)
        {
            var path = new List<Tile> { currentTile };

            while (tileParents.ContainsKey(currentTile))
            {
                currentTile = tileParents[currentTile];
                path.Add(currentTile);
            }

            path.Reverse();
            return path;
        }

        private int CalculateHeuristic(Tile tile1, Tile tile2)
        {
            // Calculate the Manhattan distance as the heuristic
            int dx = Math.Abs(GetTileIndex(tile1).Item1 - GetTileIndex(tile2).Item1);
            int dy = Math.Abs(GetTileIndex(tile1).Item2 - GetTileIndex(tile2).Item2);
            return dx + dy;
        }

        private Tuple<int, int> GetTileIndex(Tile tile)
        {
            int width = MapData.GetLength(0);
            int height = MapData.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (MapData[x, y] == tile)
                    {
                        return Tuple.Create(x, y);
                    }
                }
            }

            // Tile not found, return invalid index
            return Tuple.Create(-1, -1);
        }

        private int CalculateMovementCost(Tile currentTile, Tile neighborTile)
        {
            // Calculate the movement cost from the current tile to the neighbor tile
            // In this case, assume a constant cost of 1 for simplicity
            return 1;
        }

        private List<Tile> GetNeighbors(Tile tile)
        {
            var neighbors = new List<Tile>();

            int width = MapData.GetLength(0);
            int height = MapData.GetLength(1);

            int tileX = -1;
            int tileY = -1;

            // Find the coordinates of the given tile in the map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (MapData[x, y] == tile)
                    {
                        tileX = x;
                        tileY = y;
                        break;
                    }
                }

                if (tileX != -1 && tileY != -1)
                    break;
            }

            if (tileX == -1 || tileY == -1)
            {
                // Tile not found in the map
                return neighbors;
            }

            // Get the neighboring tiles of the given tile, including diagonal neighbors
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int neighborX = tileX + xOffset;
                    int neighborY = tileY + yOffset;

                    if (IsValidTile(neighborX, neighborY) && (xOffset != 0 || yOffset != 0))
                    {
                        neighbors.Add(MapData[neighborX, neighborY]);
                    }
                }
            }

            return neighbors;
        }

        private bool IsValidTile(int x, int y)
        {
            int width = MapData.GetLength(0);
            int height = MapData.GetLength(1);

            // Check if the coordinates are within the valid range of the map
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                // Additional conditions for diagonal movement
                // Ensure that both adjacent tiles in the diagonal direction are also valid
                if (x - 1 >= 0 && y - 1 >= 0 && MapData[x - 1, y - 1].MapTile == MapTiles.water)
                    return false;

                if (x + 1 < width && y - 1 >= 0 && MapData[x + 1, y - 1].MapTile == MapTiles.water)
                    return false;

                if (x - 1 >= 0 && y + 1 < height && MapData[x - 1, y + 1].MapTile == MapTiles.water)
                    return false;

                if (x + 1 < width && y + 1 < height && MapData[x + 1, y + 1].MapTile == MapTiles.water)
                    return false;

                // Tile is valid if it is not water
                return MapData[x, y].MapTile != MapTiles.water;
            }

            // Coordinates are outside the valid range
            return false;
        }

        private Tile GetRandomNonWaterTile()
        {
            // Get a random tile that is not water
            while (true)
            {
                int x = random.Next(Width);
                int y = random.Next(Height);
                if (MapData[x, y].MapTile != MapTiles.water)
                {
                    return MapData[x, y];
                }
            }
        }

        private Tile GetRandomNonWaterTile(Tile startTile, int maxRange)
        {
            // Get a random tile that is not water and within the specified range of the start tile

            int startX = GetTileIndex(startTile).Item1;
            int startY = GetTileIndex(startTile).Item2;

            for (int i = 0; i < maxRange; i++)
            {
                int xOffset = random.Next(-maxRange, maxRange + 1);
                int yOffset = random.Next(-maxRange, maxRange + 1);

                int x = startX + xOffset;
                int y = startY + yOffset;

                if (IsValidTile(x, y) && MapData[x, y].MapTile != MapTiles.water)
                {
                    return MapData[x, y];
                }
            }

            // If no valid tile is found within the range, return null or handle accordingly
            return null;
        }

        public async Task UpdateTiles()
        {
            HashSet<(int, int)> newlyCreatedWaterTiles = new HashSet<(int, int)>();

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (MapData[x, y].MapTile == MapTiles.water)
                    {
                        if (y + 1 < Height && MapData[x, y + 1].MapTile == MapTiles.empty)
                        {
                            newlyCreatedWaterTiles.Add((x, y + 1));
                        }
                        else if (y + 1 < Height && MapData[x, y + 1].MapTile == MapTiles.water)
                        {
                            if (x + 1 < Width && MapData[x + 1, y].MapTile == MapTiles.empty && MapData[x + 1, y + 1].MapTile != MapTiles.water)
                            {
                                newlyCreatedWaterTiles.Add((x + 1, y));
                            }
                            if (x - 1 >= 0 && MapData[x - 1, y].MapTile == MapTiles.empty && MapData[x - 1, y + 1].MapTile != MapTiles.water)
                            {
                                newlyCreatedWaterTiles.Add((x - 1, y));
                            }
                        }
                    }

                    if (MapData[x, y].MapTile == MapTiles.dirtTop)
                    {
                        if (y - 1 >= 0 && MapData[x, y - 1].MapTile == MapTiles.empty)
                        {
                            MapData[x, y - 1] = new Tile(MapTiles.dirtTop, TileTextures.DirtTexture, false, false);
                        }
                    }
                }
            }

            await Task.Delay(500);

            foreach ((int x, int y) in newlyCreatedWaterTiles)
            {
                MapData[x, y] = new Tile(MapTiles.water, TileTextures.WaterTexture, false, false);
            }

            isUpdatingTiles = false;

            await Task.Delay(50);
        }
    }
}