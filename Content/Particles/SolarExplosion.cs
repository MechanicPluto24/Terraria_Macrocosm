using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Content.Particles;

public class SolarExplosion : Particle
{
    public override int FrameCount => 5;
    public override bool DespawnOnAnimationComplete => true;

    public override void SetDefaults()
    {
        FrameSpeed = 3;
        Color = new Color(255, 164, 57);
    }

    public override void OnSpawn()
    {
    }

    public override void AI()
    {
        Lighting.AddLight(Center, Color.ToVector3());
    }

    public override void OnKill()
    {
    }
}
