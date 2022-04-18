using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CS5410.Input;
using CS5410.Particles;
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
        private ParticleEmitter m_emitter;
        private KeyboardInput m_inputHandler = new KeyboardInput();
        private bool gameOver;
        private bool pause;
        private List<int> scores;
        private int gameScore;
        private SpriteFont m_font;
        private ContentManager m_contentManager;
        private ParticleSystem m_particleSystem;
        private Objects.Controls m_keyboardLayout;
        
        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            gameOver = false;
            scores = new List<int>() {0, 0, 0, 0, 0};
            gameScore = 0;
            pause = false;
            m_particleSystem = new ParticleSystem(m_contentManager);
            m_particleSystem.initialize(graphicsDevice, graphics);
            loadScores();
            loadLayout();
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_contentManager = contentManager;
            m_font = m_contentManager.Load<SpriteFont>("Fonts/Font");
        }
        
        private void loadLayout()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    loadLayoutAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }
        
        private async Task loadLayoutAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("layout.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("layout.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(Objects.Controls));
                                    m_keyboardLayout = (Objects.Controls)mySerializer.Deserialize(fs);
                                    m_inputHandler.registerCommand("back", Keys.Escape, true, new InputDeviceHelper.CommandDelegate(navigateBack));
                                    m_inputHandler.registerCommand("space", Keys.Space, true, new InputDeviceHelper.CommandDelegate(unpause));
                                    m_inputHandler.registerCommand("emit", m_keyboardLayout.Particle, true, new InputDeviceHelper.CommandDelegate(createParticleEmitter));
                                }
                            }
                        }
                        else
                        {
                            m_keyboardLayout = new Objects.Controls();
                            m_keyboardLayout.Particle = Keys.E;
                            m_inputHandler.registerCommand("back", Keys.Escape, true, new InputDeviceHelper.CommandDelegate(navigateBack));
                            m_inputHandler.registerCommand("emit", m_keyboardLayout.Particle, true, new InputDeviceHelper.CommandDelegate(createParticleEmitter));
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
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }
                this.loading = false;
            });
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

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            
            if (pause)
            {
                renderPause();
                m_spriteBatch.End();
                return;
            }
            
            m_particleSystem.render(m_spriteBatch);
            
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
        
        private void renderPause()
        {
            float bottom = (float) (m_graphics.PreferredBackBufferHeight * 0.2);
            bottom = drawMenuItem(m_font, "PAUSED", bottom, Color.White);
            bottom = drawMenuItem(m_font, " ", bottom, Color.Aqua);
            bottom = drawMenuItem(m_font, "Press SPACE to resume", bottom, Color.SkyBlue);
            bottom = drawMenuItem(m_font, "Press ESC to return to menu", bottom, Color.DarkRed);
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_gameState = GameStateEnum.GamePlay;
            m_inputHandler.Update(gameTime);
            return m_gameState;
        }

        public override void update(GameTime gameTime)
        {
            if (pause)
            {
                return;
            }
            m_particleSystem.update(gameTime);
        }
        
        private void navigateBack(GameTime gameTime)
        {
            if (pause || gameOver)
            {
                foreach (int score in scores)
                {
                    if (gameScore > score)
                    {
                        scores.Insert(scores.IndexOf(score), gameScore);
                        saveScore(scores);
                        break;
                    }
                }
                m_gameState = GameStateEnum.MainMenu;
            }
            else
            {
                pause = true;
            }
        }

        private void unpause(GameTime gameTime)
        {
            if (pause)
            {
                pause = false;
                return;
            }
        }

        private void createParticleEmitter(GameTime gameTime)
        {
            // Load Objects
            int middleX = m_graphics.GraphicsDevice.Viewport.Width / 2;
            int middleY = m_graphics.GraphicsDevice.Viewport.Height / 2;
            m_particleSystem.addParticleEmitter(new TimeSpan(0, 0, 0, 10), middleX, middleY);
            gameScore += 1;
        }
    }
}
