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

        public static void HandleCollision(Player player, Rectangle tileRect)
        {
            Rectangle playerBounds = player.Rect;
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(playerBounds, tileRect);

            if (depth != Vector2.Zero)
            {
                // Resolve the collision along the shallow axis (Y axis in this case).
                if (Math.Abs(depth.Y) < Math.Abs(depth.X))
                {
                    // If the player crossed the top of the tile, they are on the ground.
                    if (player.previousBottom <= tileRect.Top)
                    {
                        player.IsOnGround = true;
                    }

                    // Resolve the collision along the Y axis.
                    player.Pos += new Vector2(0, depth.Y) + new Vector2(0, 1);

                    // Perform further collisions with the updated player bounds.
                    playerBounds = player.Rect;
                }
                else // Resolve the collision along the X axis.
                {
                    player.Pos += new Vector2(depth.X, 0);

                    // Perform further collisions with the updated player bounds.
                    playerBounds = player.Rect;
                }
            }
        }
    }
}