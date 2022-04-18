using Microsoft.Xna.Framework.Input;

namespace CS5410.Objects
{
    public class Controls
    {
        public Controls() { }

        public Controls(Keys particle)
        {
            Particle = particle;
        }
        
        public Keys Particle { get; set; }
    }
}