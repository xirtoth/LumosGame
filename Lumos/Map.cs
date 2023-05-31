using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
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
        grassTop
    }

    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private float elapsedTime = 0f;

        public Texture2D Texture { get; set; }

        public MapTiles[,] MapData;

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
            MapData = new MapTiles[Width, Height];
        }

        public void GenerateMap()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (y < 20)
                    {
                        MapData[x, y] = MapTiles.empty;
                    }
                    if (y < 20 && y < 19)
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            MapData[x, y] = MapTiles.dirtTop;
                        }
                    }
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
                    else
                    {
                        MapData[x, y] = MapTiles.dirt;
                    }
                }
            }

            for (int x = 0; x < 10; x++)
            {
                // Generate caves using random walk
                // int startX = random.Next(10, Width - 10);
                int startX = random.Next(10, Width);
                int startY = random.Next(25, Height - 10);
                int caveSize = random.Next(10, 100);

                RandomWalk(startX, startY, caveSize);
            }

            int waterCount = 0;
            while (waterCount < 100)
            {
                int x = random.Next(Width);
                int y = random.Next(Height);

                if (MapData[x, y] == MapTiles.empty)
                {
                    MapData[x, y] = MapTiles.water;
                    waterCount++;
                }
            }

            //Task.Run(UpdateTiles);
        }

        private void RandomWalk(int startX, int startY, int caveSize)
        {
            int currentX = startX;
            int currentY = startY;

            while (caveSize > 0)
            {
                MapData[currentX, currentY] = MapTiles.empty;

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

        public void Update()
        {
            if (!isUpdatingTiles)
            {
                isUpdatingTiles = true;
                Task.Run(() => UpdateTiles());
            }
        }

        /*
         *
         *      public void Update(Player player)
              {
                  Rectangle playerRect = player.Rect;

                  for (int x = 0; x < Width; x++)
                  {
                      for (int y = 0; y < Height; y++)
                      {
                          Rectangle tileRect = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);

                          if (playerRect.Intersects(tileRect))
                          {
                              MapData[x, y] = MapTiles.water;
                              // Handle collision, prevent player from passing through the dirt tile
                              // For example, you can stop the player from moving in that direction or reset their position
                              // Here, we'll reset the player's position to their previous position
                              // player.Pos = player.PreviousPos;
                          }
                      }
                  }
              }*/

        public void DrawMap(SpriteBatch spriteBatch, Player player, Vector2 cameraPos, Viewport viewport, GraphicsDevice gd, GameTime gameTime)
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
                        if (MapData[x, y] == MapTiles.dirtTop)
                        {
                            if (y < Height)
                            {
                                if (MapData[x, y + 1] != MapTiles.dirt)
                                {
                                    MapData[x, y] = MapTiles.empty;
                                }
                            }
                        }

                        if (MapData[x, y] == MapTiles.dirt)
                        {
                            if (y > 0)
                            {
                                if (MapData[x, y - 1] == MapTiles.empty)
                                {
                                    MapData[x, y] = MapTiles.grassTop;
                                }
                            }
                            Rectangle tileRect = new Rectangle(
                        (int)tilePosition.X + x * tileWidth,
                        (int)tilePosition.Y + y * tileHeight,
                        tileWidth,
                        tileHeight);
                            spriteBatch.Draw(TileTextures.DirtTexture, tilePosition, Color.White);
                            var testRect = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                            if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                            {
                                if (player.Rect.Intersects(testRect))
                                {
                                    spriteBatch.Draw(_texture, new Rectangle((int)tilePosition.X, (int)tilePosition.Y, tileWidth, tileHeight), Color.White);
                                    // CollisionManager.HandleCollision(player, testRect);
                                }
                            }
                        }
                        else if (MapData[x, y] == MapTiles.dirtTop)
                        {
                            {
                                spriteBatch.Draw(TileTextures.DirtTopTexture, tilePosition, Color.Red);
                            }
                        }
                        else if (MapData[x, y] == MapTiles.water)
                        {
                            spriteBatch.Draw(TileTextures.WaterTexture, tilePosition, waterColor);
                        }
                        else if (MapData[x, y] == MapTiles.grassTop)

                        {
                            spriteBatch.Draw(TileTextures.GrassTop, tilePosition, Color.White);
                        }
                        spriteBatch.Draw(_texture, player.HorizontalCollisionRect, Color.Blue * 0.5f);
                        spriteBatch.Draw(_texture, player.VerticalCollisionRect, Color.Green * 0.5f);

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
            HashSet<(int, int)> newlyCreatedWaterTiles = new HashSet<(int, int)>(); // Track newly created water tiles

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (MapData[x, y] == MapTiles.water)
                    {
                        // Apply water physics here
                        if (y + 1 < Height && MapData[x, y + 1] == MapTiles.empty)
                        {
                            newlyCreatedWaterTiles.Add((x, y + 1)); // Add newly created water tile to the set
                        }
                        else if (y + 1 < Height && MapData[x, y + 1] == MapTiles.water)
                        {
                            if (x + 1 < Width && MapData[x + 1, y] == MapTiles.empty && MapData[x + 1, y + 1] != MapTiles.water)
                            {
                                newlyCreatedWaterTiles.Add((x + 1, y)); // Add newly created water tile to the set
                            }
                            if (x - 1 >= 0 && MapData[x - 1, y] == MapTiles.empty && MapData[x - 1, y + 1] != MapTiles.water)
                            {
                                newlyCreatedWaterTiles.Add((x - 1, y)); // Add newly created water tile to the set
                            }
                        }
                    }

                    if (MapData[x, y] == MapTiles.dirtTop)
                    {
                        if (y - 1 >= 0 && MapData[x, y - 1] == MapTiles.empty)
                        {
                            MapData[x, y - 1] = MapTiles.dirtTop;
                        }
                    }
                }
            }

            await Task.Delay(500); // Delay for 0.5 seconds

            foreach ((int x, int y) in newlyCreatedWaterTiles)
            {
                MapData[x, y] = MapTiles.water; // Update the map with newly created water tiles
            }

            isUpdatingTiles = false; // Update is complete

            await Task.Delay(1);
        }
    }
}