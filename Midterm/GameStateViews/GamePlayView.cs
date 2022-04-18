using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CS5410.Input;
using CS5410.Objects;
using CS5410.Particles;
using Microsoft.Xna.Framework;
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
        private KeyboardInput m_inputHandler = new KeyboardInput();
        private bool gameOver;
        private List<int> scores;
        private int gameScore;
        private ContentManager m_contentManager;
        private ParticleEmitter m_emitter;
        private Objects.Controls m_keyboardLayout;

        private SpriteFont m_font;
        private Texture2D m_texBlack;
        private Texture2D m_texBlue;
        private Texture2D m_texGreen;
        private Texture2D m_texYellow;
        private Texture2D m_background;

        private Meter m_meter;
        private Region m_region;
        private Marker m_marker;

        private int m_meterBorder;

        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            setData();
            loadScores();

            m_keyboardLayout = new Objects.Controls();
            m_keyboardLayout.Hit = Keys.Space;
            m_keyboardLayout.New = Keys.Enter;
            m_inputHandler.registerCommand("back", Keys.Escape, true, new InputDeviceHelper.CommandDelegate(navigateBack));
            m_inputHandler.registerCommand("hit", m_keyboardLayout.Hit, true, new InputDeviceHelper.CommandDelegate(hit));
            m_inputHandler.registerCommand("new", m_keyboardLayout.New, true, new InputDeviceHelper.CommandDelegate(newGame));
        }

        private void setData()
        {
            gameOver = false;
            loadScores();
            gameScore = 0;
            m_meterBorder = 5;

            m_meter = new Meter(
                new Vector2(
                    m_graphics.PreferredBackBufferWidth / 2,
                    m_graphics.PreferredBackBufferHeight / 2),
                m_graphics.PreferredBackBufferWidth / 2,
                m_graphics.PreferredBackBufferHeight / 8);
                
            m_region = new Region(
                new Vector2(
                    m_graphics.PreferredBackBufferWidth / 2, 
                    m_graphics.PreferredBackBufferHeight / 2), 
                (m_graphics.PreferredBackBufferWidth / 4), 
                (m_graphics.PreferredBackBufferHeight / 8)
            );

            m_marker = new Marker(
                m_meter,
                new Vector2(
                    m_graphics.PreferredBackBufferWidth / 2,
                    m_graphics.PreferredBackBufferHeight / 2),
                (m_graphics.PreferredBackBufferWidth / 256),
                (m_graphics.PreferredBackBufferHeight / 8)
            );
            
            m_emitter = new ParticleEmitter(
                m_contentManager,
                new TimeSpan(0, 0, 0, 0, 200),
                new TimeSpan(0, 0, 0, 0, 10),
                m_marker.Boundary,
                2,
                5,
                new TimeSpan(0, 0, 0, 0, 200),
                new TimeSpan(0, 0, 0, 2));
        }
        
        private void saveScore(List<int> scores)
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    finalizeSaveAsync(scores);
                }
            }
        }
        
        private async void finalizeSaveAsync(List<int> scores)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("scores.xml", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(List<int>));
                                mySerializer.Serialize(fs, scores);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this.saving = false;
            });
        }
        
        private void loadScores()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    loadScoresAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }

        private async Task loadScoresAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("scores.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("scores.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(List<int>));
                                    scores = (List<int>)mySerializer.Deserialize(fs);
                                }
                            }
                        }
                        else
                        {
                            scores = new List<int>() {0, 0, 0, 0, 0};
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this.loading = false;
            });
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_contentManager = contentManager;
            m_font = m_contentManager.Load<SpriteFont>("Fonts/Font");
            m_texBlack = m_contentManager.Load<Texture2D>("Textures/black");
            m_texBlue = m_contentManager.Load<Texture2D>("Textures/blue");
            m_texGreen = m_contentManager.Load<Texture2D>("Textures/green");
            m_texYellow = m_contentManager.Load<Texture2D>("Textures/yellow");
            m_background = m_contentManager.Load<Texture2D>("Textures/background");

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
            if (gameOver)
            {
                renderGameOver();
            }
            else
            {
                renderMeter();
                renderRegion();
                renderMarker();
                m_emitter.draw(m_spriteBatch);
                renderScore();

                // m_particleSystem.render(m_spriteBatch);
            }

            m_spriteBatch.End();
        }

        private void renderGameOver()
        {
            float bottom = drawMenuItem(m_font, "Game Over", m_graphics.PreferredBackBufferHeight / 8, Color.White);
            bottom = drawMenuItem(m_font, "Final Score: " + gameScore, bottom, Color.White);
            bottom = drawMenuItem(m_font, " ", bottom, Color.White);
            bottom = drawMenuItem(m_font, "Pres ESC to Exit", bottom, Color.White);
            bottom = drawMenuItem(m_font, "Or ENTER to Play Again", bottom, Color.White);
        }

        private void renderMeter()
        {
            renderRectWithBorder(
                m_texBlack,
                m_texBlue,
                m_meter.Boundary,
                m_meterBorder,
                Color.Black,
                Color.Blue);
        }
        
        private void renderRegion()
        {
            renderRectWithBorder(
                m_texBlack,
                m_texGreen,
                m_region.Boundary,
                m_meterBorder,
                Color.Black,
                Color.LightGreen);
        }

        private void renderMarker()
        {
            renderRectWithBorder(
                m_texBlack,
                m_texYellow,
                m_marker.Boundary,
                m_meterBorder,
                Color.Black,
                Color.Yellow);
        }

        private void renderRectWithBorder(Texture2D borderTex, Texture2D rectTex, Rectangle rect, int border, Color borderColor, Color rectColor)
        {
            m_spriteBatch.Draw(
                borderTex,
                new Rectangle(
                    rect.Left - border / 2,
                    rect.Top - border / 2,
                    rect.Width + border,
                    rect.Height + border),
                borderColor);
            m_spriteBatch.Draw(
                rectTex,
                rect,
                rectColor);
        }

        private void renderScore()
        {
            float bottom = drawMenuItem(m_font, "Score:", m_graphics.PreferredBackBufferHeight / 8, Color.White);
            bottom = drawMenuItem(m_font, gameScore.ToString(), bottom, Color.White);
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

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_gameState = GameStateEnum.GamePlay;
            m_inputHandler.Update(gameTime);
            return m_gameState;
        }

        public override void update(GameTime gameTime)
        {
            if (!gameOver) {
                m_marker.update(gameTime);
                m_emitter.update(gameTime);
            }
        }
        
        private void navigateBack(GameTime gameTime)
        {
            m_gameState = GameStateEnum.MainMenu;
        }

        private void hit(GameTime gameTime)
        {
            if (m_marker.Boundary.Intersects(m_region.Boundary))
            {
                m_emitter.addParticles(gameTime, m_texYellow, m_marker.Boundary);
                gameScore += (int) (100 * (float) (1 + (0.2 * (m_region.Level - 1))));
                if (m_region.Level < 6)
                {
                    m_region.Level += 1;
                    m_region.setBoundary();
                }
            }
            else
            {
                gameOver = true;
                foreach (int score in scores)
                {
                    if (gameScore > score)
                    {
                        scores.Insert(scores.IndexOf(score), gameScore);
                        saveScore(scores);
                        break;
                    }
                }
            }
        }

        private void newGame(GameTime gameTime)
        {
            setData();
        }
    }
}
