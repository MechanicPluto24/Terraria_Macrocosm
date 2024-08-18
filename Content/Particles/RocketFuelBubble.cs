using Macrocosm.Common.Drawing.Particles;

namespace Macrocosm.Content.Particles
{
    public class RocketFuelBubble : Particle
    {
        public override void AI()
        {
            Scale -= 0.001f;

            if (Scale < 0.2f)
                Active = false;
        }
    }
}
