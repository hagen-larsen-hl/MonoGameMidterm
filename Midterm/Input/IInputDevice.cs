using Microsoft.Xna.Framework;

namespace CS5410.Input
{
    /// <summary>
    /// Abstract base class that defines how input is presented to game code.
    /// </summary>
    public interface IInputDevice
    {
        void Update(GameTime gameTime);
    }

    public class InputDeviceHelper
    {
        public delegate void CommandDelegate(GameTime gameTime);
    }
}