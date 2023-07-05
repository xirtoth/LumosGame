using Microsoft.Xna.Framework;

namespace Lumos
{
    public class RainDrop
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Size { get; set; }
        public float Transparency { get; set; }

        public Raintype Raintype { get; set; }
    }
}