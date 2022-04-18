using Microsoft.Xna.Framework;

namespace CS5410.Objects
{
    public class Meter
    {
        public Meter(Vector2 center, int width, int height)
        {
            Center = center;

            Boundary = new Rectangle(
                (int) center.X - width / 2,
                (int) center.Y - height / 2,
                width,
                height
                );
        }

        public Rectangle Boundary { get; set; }
        
        public Vector2 Center { get; set; }
    }
}