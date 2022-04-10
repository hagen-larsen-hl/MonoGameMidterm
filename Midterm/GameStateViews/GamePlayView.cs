using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        private bool saving;
        private bool loading;
        private GameStateEnum m_gameState;
        
        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void loadContent(ContentManager contentManager)
        {
        }

        public override void render(GameTime gameTime)
        {
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            return m_gameState;
        }

        public override void update(GameTime gameTime)
        {
            
        }
    }
}
