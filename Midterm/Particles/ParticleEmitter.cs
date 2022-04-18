using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Particles
{
    public class ParticleEmitter
    {

        public Dictionary<int, Particle> m_particles = new Dictionary<int, Particle>();
        private Texture2D m_texSnowflake;
        private CustomRandom m_random = new CustomRandom();

        private TimeSpan m_rate;
        private int m_sourceX;
        private int m_sourceY;
        private int m_sarticleSize;
        private int m_speed;
        private TimeSpan m_lifetime;
        private TimeSpan m_switchover;
        public TimeSpan m_lifeRemaining;
        private TimeSpan m_accumulated = TimeSpan.Zero;

        public Vector2 Gravity { get; set; }

        public ParticleEmitter(ContentManager content, TimeSpan lifeRemaining, TimeSpan rate, int sourceX, int sourceY, int size, int speed, TimeSpan lifetime, TimeSpan wwitchover)
        {
            m_lifeRemaining = lifeRemaining;
            m_rate = rate;
            m_sourceX = sourceX;
            m_sourceY = sourceY;
            m_sarticleSize = size;
            m_speed = speed;
            m_lifetime = lifetime;
            m_switchover = wwitchover;

            m_texSnowflake = content.Load<Texture2D>("Textures/snowflake");

            this.Gravity = new Vector2(0, 0);
        }

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public void update(GameTime gameTime)
        {
            m_lifeRemaining -= gameTime.ElapsedGameTime;
            //
            // Generate particles at the specified rate
            if (m_lifeRemaining > TimeSpan.Zero)
            {
                addParticles(gameTime);
            }
            
            //
            // For any existing particles, update them, if we find ones that have expired, add them
            // to the remove list.
            updateParticles(gameTime);
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, m_sarticleSize, m_sarticleSize);
            foreach (Particle p in m_particles.Values)
            {
                Texture2D texDraw;
                if (p.lifetime < m_switchover)
                {
                    texDraw = m_texSnowflake;
                }
                else
                {
                    texDraw = m_texSnowflake;
                }

                r.X = (int)p.position.X;
                r.Y = (int)p.position.Y;
                spriteBatch.Draw(
                    texDraw,
                    r,
                    null,
                    Color.White,
                    p.rotation,
                    new Vector2(texDraw.Width / 2, texDraw.Height / 2),
                    SpriteEffects.None,
                    0);
            }
        }

        private void addParticles(GameTime gameTime)
        {
            m_accumulated += gameTime.ElapsedGameTime;
            while (m_accumulated > m_rate)
            {
                m_accumulated -= m_rate;

                // Particle p = new Particle(
                    // m_random.Next(),
                    // new Vector2(m_sourceX, m_sourceY),
                    // m_random.nextCircleVector(),
                    // (float)m_random.nextGaussian(m_speed, 1),
                    // m_lifetime);

                Particle p = new Particle(
                    m_random.Next(),
                    new Vector2(m_random.Next(0, 1000), 0),
                    new Vector2((float)0, (float)1),
                    m_random.Next(2, 10),
                    new TimeSpan(0, 0, 0, 2)
                );

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }
        }

        private void updateParticles(GameTime gameTime)
        {
            List<int> removeMe = new List<int>();
            foreach (Particle p in m_particles.Values)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(p.name);
                }
                //
                // Update its position
                p.position += (p.direction * p.speed);
                //
                // Have it rotate proportional to its speed
                p.rotation += p.speed / 50.0f;
                //
                // Apply some gravity
                p.direction += this.Gravity;
            }

            //
            // Remove any expired particles
            foreach (int Key in removeMe)
            {
                m_particles.Remove(Key);
            }
        }
    }
}
