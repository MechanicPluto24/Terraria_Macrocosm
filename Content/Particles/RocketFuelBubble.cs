using Macrocosm.Common.Drawing.Particles;

namespace Macrocosm.Content.Particles;

public class RocketFuelBubble : Particle
{
    public override void SetDefaults()
    {
        ScaleVelocity = new(-0.001f);
    }

    public override void OnSpawn()
    {
    }

    public override void AI()
    {
        if (Scale.X < 0.2f)
            Active = false;
    }
}
