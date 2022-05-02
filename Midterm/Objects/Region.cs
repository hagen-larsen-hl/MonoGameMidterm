using Microsoft.Xna.Framework;

namespace CS5410.Objects
{
    public class Region
    {
        public Region(Vector2 center, int width, int height)
        {
            Center = center;

            Boundary = new Rectangle(
                (int) center.X - (width / 2),
                (int) center.Y - (height / 2),
                width,
                height);

            Level = 1;
        }
        
        public Rectangle Boundary { get; set; }
        
        public void setBoundary()
        {
            Boundary = new Rectangle(
                (int) (Center.X - ((Boundary.Width - (Boundary.Width / (Level * 0.8))) / 2)),
                (int) Center.Y - (Boundary.Height / 2),
                (int) (Boundary.Width - (Boundary.Width / (Level * 0.8))),
                Boundary.Height);
        }
        public Vector2 Center { get; set; }
        
        public int Level { get; set; }
    }
}