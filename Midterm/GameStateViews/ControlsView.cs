﻿using System;
using System.IO;
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
    public class ControlsView : GameStateView
    {
        private SpriteFont m_font; 
        private bool saving;
        private bool loading;
        private bool awaitKey;
        private bool enterPressed;
        private KeyboardInput m_controlsInputHandler;
        private GameStateEnum m_gameState = GameStateEnum.Help;
        private Objects.Controls m_keyboardLayout = new Objects.Controls();

        private enum Selection
        {
            Up,
            Down,
            Right,
            Left,
            Fire
        }

        private Selection m_currentSelection = Selection.Up;
        
        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            
            m_controlsInputHandler = new KeyboardInput();
            m_controlsInputHandler.registerCommand("up", Keys.Up, true, navigateUp);
            m_controlsInputHandler.registerCommand("down", Keys.Down, true, navigateDown);
            m_controlsInputHandler.registerCommand("enter", Keys.Enter, true, selectOption);
            m_controlsInputHandler.registerCommand("back", Keys.Escape, true, navigateBack);

            enterPressed = true;

            loadLayout();
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/Font");
        }
        
        /// <summary>
        /// Demonstrates how serialize an object to storage
        /// </summary>
        private void saveLayout(Objects.Controls layout)
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    saveLayoutAsync(layout);
                }
            }
        }
        
        private async void saveLayoutAsync(Objects.Controls layout)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("layout.xml", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(Objects.Controls));
                                mySerializer.Serialize(fs, layout);
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
                                }
                            }
                        }
                        else
                        {
                            m_keyboardLayout = new Objects.Controls();
                            m_keyboardLayout.Up = Keys.Up;
                            m_keyboardLayout.Down = Keys.Down;
                            m_keyboardLayout.Left = Keys.Left;
                            m_keyboardLayout.Right = Keys.Right;
                            m_keyboardLayout.Fire = Keys.Space;
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

        public override GameStateEnum processInput(GameTime gameTime)
        {

            m_gameState = GameStateEnum.Help;
            
            if (Keyboard.GetState().GetPressedKeyCount() == 0)
            {
                enterPressed = false;
            }

            if (!awaitKey && !enterPressed)
            {
                m_controlsInputHandler.Update(gameTime);   
            }

            // If we are awaiting a key press and one exists, assign to current selection
            if (awaitKey && !enterPressed && Keyboard.GetState().GetPressedKeyCount() > 0)
            {
                // set to m_currentSelection
                Keys key = Keyboard.GetState().GetPressedKeys()[0];

                if (m_currentSelection == Selection.Up)
                {
                    m_keyboardLayout.Up = key;
                }
                if (m_currentSelection == Selection.Down)
                {
                    m_keyboardLayout.Down = key;
                }
                if (m_currentSelection == Selection.Right)
                {
                    m_keyboardLayout.Right = key;
                }
                if (m_currentSelection == Selection.Left)
                {
                    m_keyboardLayout.Left = key;
                }
                if (m_currentSelection == Selection.Fire)
                {
                    m_keyboardLayout.Fire = key;
                }
                Console.Write("Saving" + m_keyboardLayout.Fire);
                saveLayout(m_keyboardLayout);
                awaitKey = false;
            }
            
            return m_gameState;
        }

        private void navigateUp(GameTime gameTime)
        {
            if (!awaitKey && m_currentSelection != Selection.Up)
            {
                m_currentSelection -= 1;
            }
        }

        private void navigateDown(GameTime gameTime)
        {
            if (!awaitKey && m_currentSelection != Selection.Fire)
            {
                m_currentSelection += 1;
            }
        }
        
        private void selectOption(GameTime gameTime)
        {
            if (!awaitKey)
            {
                awaitKey = true;
                enterPressed = true;
            }
        }

        private void navigateBack(GameTime gameTime)
        {
            m_gameState = GameStateEnum.MainMenu;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            m_spriteBatch.DrawString(m_font, "CONTROLS",
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - m_font.MeasureString("CONTROLS").X / 2, m_graphics.PreferredBackBufferHeight / 8), Color.White);
            
            if (awaitKey)
            {
                m_spriteBatch.DrawString(m_font, "SET NEW CONTROL",
                    new Vector2(m_graphics.PreferredBackBufferWidth / 2 - m_font.MeasureString("SET NEW CONTROL").X / 2, m_graphics.PreferredBackBufferHeight / 2 - m_font.MeasureString("SET NEW CONTROL").Y / 2), Color.White);
            }
            else
            {
                float bottom = (float) (m_graphics.PreferredBackBufferHeight * 0.2);
                bottom = drawMenuItem(m_font, "CUSTOMIZE CONTROLS HERE", bottom, Color.Blue);
                bottom = drawMenuItem(m_font,"Move Up: " + m_keyboardLayout.Up, bottom, m_currentSelection == Selection.Up ? Color.LimeGreen : Color.Green);
                bottom = drawMenuItem(m_font, "Move Down: " + m_keyboardLayout.Down, bottom, m_currentSelection == Selection.Down ? Color.LimeGreen : Color.Green);
                bottom = drawMenuItem(m_font, "Move Right: " + m_keyboardLayout.Right, bottom, m_currentSelection == Selection.Right ? Color.LimeGreen : Color.Green);
                bottom = drawMenuItem(m_font, "Move Left: " + m_keyboardLayout.Left, bottom, m_currentSelection == Selection.Left ? Color.LimeGreen : Color.Green);
                drawMenuItem(m_font, "Fire: " + m_keyboardLayout.Fire, bottom, m_currentSelection == Selection.Fire ? Color.LimeGreen : Color.Green);

            }
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

        public override void update(GameTime gameTime)
        {
        }
    }
}
