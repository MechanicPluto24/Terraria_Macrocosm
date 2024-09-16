using Macrocosm.Common.Drawing.Particles;

namespace Macrocosm.Content.Particles
{
    public class RocketFuelBubble : Particle
    {
        public override void OnSpawn()
        {
            ScaleVelocity = new(-0.001f);
        }

        public override void AI()
        {
            if (Scale.X < 0.2f)
                Active = false;
        }
    }
}
