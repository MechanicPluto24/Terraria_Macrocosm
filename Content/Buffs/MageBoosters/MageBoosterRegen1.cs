using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.MageBoosters;

public class MageBoosterRegen1 : ComplexBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = false;
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void UpdateLifeRegen(Player player)
    {
        player.lifeRegen += 5;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        int dist = 50;
        Vector2 dustPosition = player.Center + new Vector2(dist, 0).RotatedBy((float)(Main.time / 100 + 2.094f));
        float distFactor = Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist);
        float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
        Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
        Particle.Create<DustParticle>(p =>
        {

            p.DustType = 43;
            p.Position = dustPosition;
            p.Velocity = velocity;
            p.Color = new Color(255, 228, 90);
            p.Acceleration = velocity * 1f;
            p.Scale = new(Main.rand.NextFloat(0.8f, 1f));
            p.NoGravity = true;
            p.NormalUpdate = true;
            p.NoLightEmittence = false;
            p.TimeToLive = 300;
        });

    }
}