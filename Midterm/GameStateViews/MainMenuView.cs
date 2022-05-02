﻿using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using centipede.Content.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class MainMenuView : GameStateView
    {
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private KeyboardInput m_inputHandler;
        private Texture2D m_background;

        private enum MenuState
        {
            NewGame,
            HighScores,
            Credits,
            Quit
        }

        private MenuState m_currentSelection = MenuState.NewGame;
        private GameStateEnum m_currentView = GameStateEnum.MainMenu;
        
        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);

            m_inputHandler = new KeyboardInput();
            m_inputHandler.registerCommand("up", Keys.Up, true, navigateUp);
            m_inputHandler.registerCommand("down", Keys.Down, true, navigateDown);
            m_inputHandler.registerCommand("enter", Keys.Enter, true, selectMenuOption);
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/Font");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/Font");
            m_background = contentManager.Load<Texture2D>("Textures/background");
        }
        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_currentView = GameStateEnum.MainMenu;
            m_inputHandler.Update(gameTime);
            return m_currentView;
        }

        private void navigateUp(GameTime gameTime)
        {
            if (m_currentSelection != MenuState.NewGame)
            {
                m_currentSelection -= 1;
            }
        }

        private void navigateDown(GameTime gameTime)
        {
            if (m_currentSelection != MenuState.Quit)
            {
                m_currentSelection += 1;
            }
        }

        private void selectMenuOption(GameTime gameTime)
        {
            if (m_currentSelection == MenuState.NewGame)
            {
                m_currentView = GameStateEnum.GamePlay;
            }
            if (m_currentSelection == MenuState.HighScores)
            {
                m_currentView = GameStateEnum.HighScores;
            }
            if (m_currentSelection == MenuState.Credits)
            {
                m_currentView = GameStateEnum.About;
            }
            if (m_currentSelection == MenuState.Quit)
            {
                m_currentView = GameStateEnum.Exit;
            }
        }
        
        public override void update(GameTime gameTime)
        {
        }
        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            
            m_spriteBatch.Draw(
                m_background,
                new Rectangle(
                    0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight),
                null,
                Color.White,
                0,
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

            // I split the first one's parameters on separate lines to help you see them better
            float bottom = drawMenuItem(
                m_currentSelection == MenuState.NewGame ? m_fontMenuSelect : m_fontMenu, 
                "New Game",
                200, 
                m_currentSelection == MenuState.NewGame ? Color.White : Color.Green);
            bottom = drawMenuItem(m_currentSelection == MenuState.HighScores ? m_fontMenuSelect : m_fontMenu, "High Scores", bottom, m_currentSelection == MenuState.HighScores ? Color.White : Color.Green);
            bottom = drawMenuItem(m_currentSelection == MenuState.Credits ? m_fontMenuSelect : m_fontMenu, "Credits", bottom, m_currentSelection == MenuState.Credits ? Color.White : Color.Green);
            drawMenuItem(m_currentSelection == MenuState.Quit ? m_fontMenuSelect : m_fontMenu, "Quit", bottom, m_currentSelection == MenuState.Quit ? Color.White : Color.Green);

            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color);

            return y + stringSize.Y;
        }
    }
}