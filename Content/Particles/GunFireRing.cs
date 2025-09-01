using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Content.Particles;

public class GunFireRing : Particle
{
    public override int FrameCount => 4;
    public override bool DespawnOnAnimationComplete => true;

    public override void SetDefaults()
    {
        FrameSpeed = 8;
    }

    public override void OnSpawn()
    {
    }

    public override void AI()
    {
        Velocity *= 0.925f;
        Lighting.AddLight(Position, new Color(255, 202, 141).ToVector3());
    }

    public override void OnKill()
    {
    }
}
