using Microsoft.Xna.Framework.Input;

namespace CS5410.Objects
{
    public class Controls
    {
        public Controls() { }

        public Controls(Keys hit, Keys newGame)
        {
            Hit = hit;
            New = newGame;
        }
        
        public Keys Hit { get; set; }
        
        public Keys New { get; set; }
    }
}