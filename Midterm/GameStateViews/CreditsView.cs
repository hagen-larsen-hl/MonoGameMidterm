using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class CreditsView : GameStateView
    {
        private SpriteFont m_font;
        private Texture2D m_background;
        
        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/Font");
            m_background = contentManager.Load<Texture2D>("Textures/background");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.About;
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
            
            float bottom = (float) (m_graphics.PreferredBackBufferHeight * 0.1);
            bottom = drawMenuItem(m_font, "CREDITS", bottom, Color.White);
            bottom = drawMenuItem(m_font," ", bottom, Color.Green);
            bottom = drawMenuItem(m_font,"DEVELOPED BY:", bottom, Color.Green);
            bottom = drawMenuItem(m_font, "Hagen Larsen", bottom, Color.White);
            
            
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
