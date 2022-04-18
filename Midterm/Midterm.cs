using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CS5410.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class Midterm : Game
    {
        private static GraphicsDeviceManager m_graphics;
        private IGameState m_currentState;
        private GameStateEnum m_nextStateEnum = GameStateEnum.MainMenu;
        private Dictionary<GameStateEnum, IGameState> m_states;
        private Objects.Controls m_keyboardLayout;

        public Midterm()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // m_graphics.PreferredBackBufferWidth = 1920;
            // m_graphics.PreferredBackBufferHeight = 1080;
            m_graphics.PreferredBackBufferWidth = 1366;
            m_graphics.PreferredBackBufferHeight = 768;

            m_graphics.ApplyChanges();

            // Create all the game states here
            m_states = new Dictionary<GameStateEnum, IGameState>();
            m_states.Add(GameStateEnum.MainMenu, new MainMenuView());
            m_states.Add(GameStateEnum.GamePlay, new GamePlayView());
            m_states.Add(GameStateEnum.HighScores, new HighScoresView());
            m_states.Add(GameStateEnum.About, new CreditsView());

            // We are starting with the main menu
            m_currentState = m_states[GameStateEnum.MainMenu];

            base.Initialize();
        }
        
        

        protected override void LoadContent()
        {
            // Give all game states a chance to load their content
            foreach (var item in m_states)
            {
                item.Value.initialize(this.GraphicsDevice, m_graphics);
                item.Value.loadContent(this.Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            m_nextStateEnum = m_currentState.processInput(gameTime);
            
            // Special case for exiting the game
            if (m_nextStateEnum == GameStateEnum.Exit)
            {
                Exit();
            }
            
            // Initialize session for next state
            else if (m_currentState != m_states[m_nextStateEnum])
            {
                m_states[m_nextStateEnum].initialize(GraphicsDevice, m_graphics);
            }

            m_currentState.update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            m_currentState.render(gameTime);

            m_currentState = m_states[m_nextStateEnum];

            base.Draw(gameTime);
        }
    }
}
