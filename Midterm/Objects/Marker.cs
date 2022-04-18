using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Objects
{
    public class Marker
    {

        public enum Orientation
        {
            Right,
            Left
        }
        public Marker(Meter meter, Vector2 center, int width, int height)
        {
            Center = center;

            Boundary = new Rectangle(
                (int) center.X - (width / 2),
                (int) center.Y - (height / 2),
                width,
                height);

            MyMeter = meter;

            Direction = Orientation.Right;
        }
        
        public Rectangle Boundary { get; set; }

        public void setBoundary()
        {
            Boundary = new Rectangle(
                (int) Center.X - (Boundary.Width / 2),
                (int) Center.Y - (Boundary.Height / 2),
                Boundary.Width,
                Boundary.Height);
        }
        
        public Vector2 Center { get; set; }
        
        public Orientation Direction { get; set; }
        
        public int Rate { get; }
        
        public Meter MyMeter { get; set; }

        public void update(GameTime gameTime)
        {
            float movement = MyMeter.Boundary.Width * gameTime.ElapsedGameTime.Milliseconds / 1000;
            Console.WriteLine("Meter Width " + MyMeter.Boundary.Width);
            Console.WriteLine("Elapsed Time " + gameTime.ElapsedGameTime.Milliseconds);
            Console.WriteLine("Movement " + movement);

            
            if (Direction == Orientation.Right)
            {
                if (Boundary.Right + movement > MyMeter.Boundary.Right)
                {
                    Console.WriteLine("My Movement " + movement);
                    Console.WriteLine("Meter Boundary Right" + MyMeter.Boundary.Right);
                    Console.WriteLine("My Boundary Right " + Boundary.Right);
                    float leftMovement = movement - (MyMeter.Boundary.Right - Boundary.Right);
                    Console.WriteLine(leftMovement);
                    Direction = Orientation.Left;
                    Center = new Vector2(
                        MyMeter.Boundary.Right - leftMovement - Boundary.Width / 2,
                        Center.Y);
                    setBoundary();
                }
                else
                {
                    Center = new Vector2(
                        Center.X + movement,
                        Center.Y);
                    setBoundary();
                }
            }
            else if (Direction == Orientation.Left)
            {
                if (Boundary.Left - movement < MyMeter.Boundary.Left)
                {
                    float rightMovement = movement - (Boundary.Left - MyMeter.Boundary.Left);
                    Direction = Orientation.Right;
                    Center = new Vector2(
                        MyMeter.Boundary.Left + rightMovement + Boundary.Width / 2,
                        Center.Y);
                    setBoundary();
                }
                else
                {
                    Center = new Vector2(
                        Center.X - movement,
                        Center.Y);
                    setBoundary();
                }
                
            }
        }
    }
}