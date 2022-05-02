using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Particles
{
    public class Particle
    {
        public Particle(int name, Vector2 position, Vector2 direction, float speed, TimeSpan lifetime, Texture2D tex)
        {
            this.name = name;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.lifetime = lifetime;
            texture = tex;

            this.rotation = 0;
        }

        public int name;
        public Vector2 position;
        public float rotation;
        public Vector2 direction;
        public float speed;
        public TimeSpan lifetime;
        public Texture2D texture;
    }
}