using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Particles
{
    public class ParticleSystem
    {
        private ContentManager m_contentManager;
        private List<ParticleEmitter> m_emitters;

        public ParticleSystem(ContentManager contentManager)
        {
            m_contentManager = contentManager;
        }

        public void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_emitters = new List<ParticleEmitter>();
        }

        public void update(GameTime gameTime)
        {
            foreach (ParticleEmitter emitter in m_emitters)
            {
                emitter.update(gameTime);
            }
        }

        public void render(SpriteBatch spriteBatch)
        {
            foreach (ParticleEmitter emitter in m_emitters)
            {
                emitter.draw(spriteBatch);
            }
        }

        public void addParticleEmitter(TimeSpan lifeRemaining, int middleX, int middleY)
        {
            // m_emitters.Add(
                // new ParticleEmitter(
                    // m_contentManager,
                    // lifeRemaining,
                    // new TimeSpan(0, 0, 0, 0, 1),
                    // middleX, middleY,
                    // 20,
                    // 10,
                    // new TimeSpan(0, 0, 0, 0, 500),
                    /// new TimeSpan(0, 0, 0, 0, 3000))
                // );
            
            m_emitters.Add(
                new ParticleEmitter(
                    m_contentManager,
                    lifeRemaining,
                    new TimeSpan(0, 0, 0, 0, 10),
                    middleX, middleY,
                    10,
                    10,
                    new TimeSpan(0, 0, 0, 3),
                    new TimeSpan(0, 0, 0, 0, 3000))
            );
        }
    }
}
