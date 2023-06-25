using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
                    if (y < 20)
                    {
                        MapData[x, y] = new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false, true);
                    }
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
                    else if (y > 18 && y < 24)
                    {
                        MapData[x, y] = new Tile(MapTiles.dirt, TileTextures.DirtTexture, true, false, true);
                    }
                    else
                    {
                        MapData[x, y] = Tiles.CreateDirt();
                    }
                }
            }

            for (int x = 0; x < Width / 2 + Height / 2; x++)
            {
                // Generate caves using random walk
                // int startX = random.Next(10, Width - 10);
                int startX = random.Next(10, Width);
                int startY = random.Next(25, Height - 10);
                int caveSize = random.Next(10, 100);

                RandomWalk(startX, startY, caveSize, new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false, false));
            }
            for (int x = 0; x < Width / 10 + Height / 10; x++)
            {
                // Generate caves using random walk
                // int startX = random.Next(10, Width - 10);
                int startX = random.Next(10, Width);
                int startY = random.Next(25, Height - 10);
                int caveSize = random.Next(10, 100);

                RandomWalk(startX, startY, caveSize, new Tile(MapTiles.water, TileTextures.WaterTexture, false, false, false));
            }
            for (int x = 0; x < Width / 10 + Height / 10; x++)
            {
                // Generate caves using random walk
                // int startX = random.Next(10, Width - 10);
                int startX = random.Next(10, Width);
                int startY = random.Next(25, Height - 10);
                int caveSize = random.Next(10, 100);
                RandomWalk(startX, startY, caveSize, Tiles.CreateMithril());
            }

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

        /// <summary>
        /// modifies map to have random caves
        /// </summary>
        /// <param name="startX">Start position x</param>
        /// <param name="startY">Start position y</param>
        /// <param name="caveSize">Size of cave</param>
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
            Color waterColor = Color.White;
            Color startColor = Color.White;
            Color endColor = Color.DarkBlue;
            elapsedTime += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            float t = elapsedTime / 0.5f; // Normalize elapsed time between 0 and 1
            if (elapsedTime < 1f)
            {
                waterColor = Color.Lerp(startColor, endColor, t);
            }
            if (elapsedTime > 1f)
            {
                startColor = endColor;
                endColor = startColor;
                elapsedTime = 0f;
                waterColor = Color.Lerp(startColor, endColor, t);
            }

            // Task.Run(() => UpdateTiles());
            Texture2D _texture;
            _texture = new Texture2D(gd, 1, 1);
            _texture.SetData(new Color[] { Color.Red });

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

                        if (MapData[x, y].MapTile == MapTiles.dirt)
                        {
                            if (y > 0)
                            {
                                if (MapData[x, y - 1].MapTile == MapTiles.empty)
                                {
                                    MapData[x, y] = new Tile(MapTiles.grassTop, TileTextures.GrassTop, true, false, true);
                                }
                            }
                            Rectangle tileRect = new Rectangle(
                        (int)tilePosition.X + x * tileWidth,
                        (int)tilePosition.Y + y * tileHeight,
                        tileWidth,
                        tileHeight);

                            /* var testRect = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                             if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                             {
                                 if (player.Rect.Intersects(testRect))
                                 {
                                     spriteBatch.Draw(_texture, new Rectangle((int)tilePosition.X, (int)tilePosition.Y, tileWidth, tileHeight), Color.White);
                                     // CollisionManager.HandleCollision(player, testRect);
                                 }
                             }*/
                        }
                        if (MapData[x, y].IsVisible)
                        {
                            spriteBatch.Draw(MapData[x, y].Texture, tilePosition, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(MapData[x, y].Texture, tilePosition, Color.Black);
                        }
                        /*    else if (MapData[x, y].MapTile == MapTiles.dirtTop)
                            {
                                {
                                    spriteBatch.Draw(MapData[x, y].Texture, tilePosition, Color.Red);
                                }
                            }
                            else if (MapData[x, y].MapTile == MapTiles.water)
                            {
                                spriteBatch.Draw(MapData[x, y].Texture, tilePosition, waterColor);
                            }
                            else if (MapData[x, y].MapTile == MapTiles.grassTop)

                            {
                                spriteBatch.Draw(TileTextures.GrassTop, tilePosition, Color.White);
                            }*/
                        //  spriteBatch.Draw(_texture, player.HorizontalCollisionRect, Color.Blue * 0.5f);
                        //  spriteBatch.Draw(_texture, player.VerticalCollisionRect, Color.Green * 0.5f);

                        // Draw player's rays for debugging
                        /*  foreach (var ray in player.Rays)
                          {
                              spriteBatch.DrawLine(ray.Start, ray.End, Color.Yellow);
                          }*/
                    }
                }
            }
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