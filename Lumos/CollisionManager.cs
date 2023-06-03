using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public class CollisionManager
    {
        public static bool CheckCollision(Rectangle rectA, Rectangle rectB)
        {
            return (rectA.Intersects(rectB));
        }

        /*   public static void HandleCollision(Player player, Rectangle tileRect)
           {
               if (CheckCollision(player.Rect, tileRect))
               {
                   // Handle vertical collision
                   if (player.velocityY > 0) // Player is falling down
                   {
                       player.velocityY = 0;
                       player.IsOnGround = true;
                       // player.Pos = new Vector2(player.Pos.X, tileRect.Y - player.Rect.Height);
                   }
                   else if (player.velocityY < 0) // Player is jumping up
                   {
                       player.velocityY = 0;
                       player.Pos = new Vector2(player.Pos.X, tileRect.Y + tileRect.Height);
                   }

                   // Handle horizontal collision
                   if (player.velocityX > 0) // Player is moving right
                   {
                       player.velocityX = 0;
                       player.isCollidingRight = true;
                       player.Pos = new Vector2(tileRect.Left - player.Rect.Width, player.Pos.Y);
                   }
                   else if (player.velocityX < 0) // Player is moving left
                   {
                       player.velocityX = 0;
                       player.isCollidingLeft = true;
                       player.Pos = new Vector2(tileRect.Right, player.Pos.Y);
                   }

                   // Check for right side collision
                   if (player.Rect.Right > tileRect.Left && player.velocityX > 0)
                   {
                       player.isCollidingRight = true;
                   }
                   else
                   {
                       player.isCollidingRight = false;
                   }

                   // Check for left side collision
                   if (player.Rect.Left < tileRect.Right && player.velocityX < 0)
                   {
                       player.isCollidingLeft = true;
                   }
                   else
                   {
                       player.isCollidingLeft = false;
                   }
               }
           }*/

        /*  public static void HandleCollision(Player player, Rectangle tileRect)
          {
              if (CheckCollision(player.Rect, tileRect))
              {
                  // Handle vertical collision
                  if (player.velocityY > 0) // Player is falling down
                  {
                      player.velocityY = 0;
                      player.IsOnGround = true;
                      player.Pos = new Vector2(player.Pos.X, tileRect.Top - player.Rect.Height) - new Vector2(0, -1);
                  }
                  else if (player.velocityY < 0) // Player is jumping up
                  {
                      player.velocityY = 0;
                      player.Pos = new Vector2(player.Pos.X, tileRect.Bottom);
                  }

                  // Handle horizontal collision
                  if (player.velocityX > 0) // Player is moving right
                  {
                      player.velocityX = 0;
                      player.isCollidingRight = true;
                      player.Pos = new Vector2(tileRect.Left - player.Rect.Width, player.Pos.Y);
                  }
                  else if (player.velocityX < 0) // Player is moving left
                  {
                      player.velocityX = 0;
                      player.isCollidingLeft = true;
                      player.Pos = new Vector2(tileRect.Right, player.Pos.Y);
                  }

                  // Check for right side collision
                  if (player.Rect.Right > tileRect.Left && player.velocityX > 0)
                  {
                      player.isCollidingRight = true;
                  }
                  else
                  {
                      player.isCollidingRight = false;
                  }

                  // Check for left side collision
                  if (player.Rect.Left < tileRect.Right && player.velocityX < 0)
                  {
                      player.isCollidingLeft = true;
                  }
                  else
                  {
                      player.isCollidingLeft = false;
                  }
              }
          }*/

        /*   public static bool HandleCollision(Rectangle rect)
           {
               int minX = Math.Max(0, (int)(rect.X / 16) - 1);
               int minY = Math.Max(0, (int)(rect.Y / 16) - 1);
               int maxX = Math.Min(Game1.Instance._map.Width - 1, (int)((rect.X + rect.Width) / 16) + 1);
               int maxY = Math.Min(Game1.Instance._map.Height, (int)((rect.Y + rect.Height) / 16) + 1);

               for (int x = minX; x <= maxX; x++)
               {
                   for (int y = minY; y <= maxY; y++)
                   {
                       Rectangle tileRect = new Rectangle(x * 16, y * 16, 16, 16);
                       if (Game1.Instance._map.MapData[x, y] != null && Game1.Instance._map.MapData[x, y].Collision && rect.Intersects(tileRect))
                       {
                           return true;
                       }
                   }
               }
               return false;
           }*/

        public static bool HandleCollision(Rectangle rect, out bool isOnGround)
        {
            int minX = Math.Max(0, (int)(rect.X / 16) - 1);
            int minY = Math.Max(0, (int)(rect.Y / 16) - 1);
            int maxX = Math.Min(Game1.Instance._map.Width - 1, (int)((rect.X + rect.Width) / 16) + 1);
            int maxY = Math.Min(Game1.Instance._map.Height, (int)((rect.Y + rect.Height) / 16) + 1);

            bool collisionDetected = false;
            bool groundDetected = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle tileRect = new Rectangle(x * 16, y * 16, 16, 16);
                    if (Game1.Instance._map.MapData[x, y] != null && Game1.Instance._map.MapData[x, y].Collision && rect.Intersects(tileRect))
                    {
                        collisionDetected = true;

                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(rect, tileRect);

                        if (depth != Vector2.Zero)
                        {
                            if (Math.Abs(depth.Y) < Math.Abs(depth.X))
                            {
                                if (rect.Bottom <= tileRect.Top)
                                {
                                    groundDetected = true;
                                }

                                rect.Y += (int)depth.Y;
                            }
                            else
                            {
                                rect.X += (int)depth.X;
                            }
                        }
                    }
                }
            }

            isOnGround = groundDetected;
            return collisionDetected;
        }

        public static bool HandleCollision(Player player, Enemy enemy)
        {
            Rectangle rect = new Rectangle((int)player.Rect.X - (int)Game1.Instance._cameraPosition.X, (int)player.Rect.Y - (int)Game1.Instance._cameraPosition.Y, player.Rect.Width, player.Rect.Height);
            if (rect.Intersects(enemy.boundingBox))
            {
                return true;
            }
            return false;
        }
    }
}