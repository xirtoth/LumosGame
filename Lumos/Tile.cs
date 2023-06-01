﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public class Tile
    {
        public MapTiles MapTile { get; set; }
        public Texture2D Texture { get; set; }

        public bool Animated { get; set; } = false;
        public bool Collision { get; set; }

        public Tile(MapTiles maptile, Texture2D texture, bool collision, bool animated)
        {
            MapTile = maptile;
            Texture = texture;
            Collision = collision;
            animated = false;
        }

        public void Update()
        {
            if (Animated)
            {
            }
        }
    }
}