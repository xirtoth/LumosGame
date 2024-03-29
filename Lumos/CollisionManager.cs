﻿using Microsoft.Xna.Framework;
using System;

namespace Lumos
{
    public class CollisionManager
    {
        public static bool CheckCollision(Rectangle rectA, Rectangle rectB)
        {
            return (rectA.Intersects(rectB));
        }

        public static bool HandleCollision(Rectangle rect, out bool isOnGround)
        {
            int minX = Math.Max(0, (int)(rect.X / 16) - 1);
            int minY = Math.Max(0, (int)(rect.Y / 16) - 1);
            int maxX = Math.Min(Game1.Instance._map.Width - 1, (int)((rect.X + rect.Width) / 16) + 1);
            int maxY = Math.Min(Game1.Instance._map.Height - 1, (int)((rect.Y + rect.Height) / 16) + 1);

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

        public static bool HandleCollision(Player player, Item item)
        {
            Rectangle rect = new Rectangle((int)player.Rect.X - (int)Game1.Instance._cameraPosition.X, (int)player.Rect.Y - (int)Game1.Instance._cameraPosition.Y, player.Rect.Width, player.Rect.Height);
            if (rect.Intersects(item.Rect))
            {
                return true;
            }
            return false;
        }

        public static bool HandleCollision(Item item, out bool isOnGround)
        {
            int minX = Math.Max(0, (int)(item.Rect.X / 16) - 1);
            int minY = Math.Max(0, (int)(item.Rect.Y / 16) - 1);
            int maxX = Math.Min(Game1.Instance._map.Width - 1, (int)((item.Rect.X + item.Rect.Width) / 16) + 1);
            int maxY = Math.Min(Game1.Instance._map.Height - 1, (int)((item.Rect.Y + item.Rect.Height) / 16) + 1);

            bool collisionDetected = false;
            bool groundDetected = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle tileRect = new Rectangle(x * 16, y * 16, 16, 16);
                    if (Game1.Instance._map.MapData[x, y] != null && Game1.Instance._map.MapData[x, y].Collision && item.Rect.Intersects(tileRect))
                    {
                        collisionDetected = true;

                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(item.Rect, tileRect);

                        if (depth != Vector2.Zero)
                        {
                            if (Math.Abs(depth.Y) < Math.Abs(depth.X))
                            {
                                if (item.Rect.Bottom <= tileRect.Top)
                                {
                                    groundDetected = true;
                                }

                                item.Rect = new Rectangle((int)item.Rect.X + (int)depth.X, item.Rect.Y, item.Texture.Width, item.Texture.Height);
                            }
                            else
                            {
                                item.Rect = new Rectangle((int)item.Rect.X, (int)item.Rect.Y + (int)depth.Y, item.Texture.Width, item.Texture.Width);
                            }
                        }
                    }
                }
            }

            isOnGround = groundDetected;
            return collisionDetected;
        }

        public static bool HandleCollision(Enemy enemy, out bool isOnGround)
        {
            int minX = Math.Max(0, (int)(enemy.boundingBox.X / 16) - 1);
            int minY = Math.Max(0, (int)(enemy.boundingBox.Y / 16) - 1);
            int maxX = Math.Min(Game1.Instance._map.Width - 1, (int)((enemy.boundingBox.X + enemy.boundingBox.Width) / 16) + 1);
            int maxY = Math.Min(Game1.Instance._map.Height - 1, (int)((enemy.boundingBox.Y + enemy.boundingBox.Height) / 16) + 1);

            bool collisionDetected = false;
            bool groundDetected = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle tileRect = new Rectangle(x * 16, y * 16, 16, 16);
                    if (Game1.Instance._map.MapData[x, y] != null && Game1.Instance._map.MapData[x, y].Collision && enemy.boundingBox.Intersects(tileRect))
                    {
                        collisionDetected = true;

                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(enemy.boundingBox, tileRect);

                        if (depth != Vector2.Zero)
                        {
                            if (Math.Abs(depth.Y) < Math.Abs(depth.X))
                            {
                                if (enemy.boundingBox.Bottom <= tileRect.Top)
                                {
                                    groundDetected = true;
                                }

                                enemy.boundingBox = new Rectangle((int)enemy.boundingBox.X + (int)depth.X, enemy.boundingBox.Y, enemy.boundingBox.Width, enemy.boundingBox.Height);
                            }
                            else
                            {
                                enemy.boundingBox = new Rectangle((int)enemy.boundingBox.X, (int)enemy.boundingBox.Y + (int)depth.Y, enemy.boundingBox.Width, enemy.boundingBox.Width);
                            }
                        }
                    }
                }
            }

            isOnGround = groundDetected;
            return collisionDetected;
        }

        public static bool HandleCollision(Rectangle rect, Enemy enemy)
        {
            // Rectangle rect = new Rectangle((int)proj.Rect.X - (int)Game1.Instance._cameraPosition.X, (int)proj.Rect.Y - (int)Game1.Instance._cameraPosition.Y, proj.Rect.Width, proj.Rect.Height);
            if (rect.Intersects(enemy.boundingBox))
            {
                return true;
            }
            return false;
        }
    }
}