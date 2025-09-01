using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.MageBoosters
{
    public class MageBoosterDamage2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<GenericDamageClass>() *= 1.1f;

            int dist = 50;
            Vector2 dustPosition = player.Center + new Vector2(dist, 0).RotatedBy((float)(Main.time / 100));
            float distFactor = Vector2.DistanceSquared(player.Center, dustPosition) / (dist * dist);
            float radians = (player.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
            Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
            Particle.Create<DustParticle>(p =>
            {
                p.DustType = 43;
                p.Position = dustPosition;
                p.Velocity = velocity;
                p.Color = new Color(107, 125, 255);
                p.Acceleration = velocity * 1f;
                p.Scale = new(Main.rand.NextFloat(1.8f, 2f));
                p.NoGravity = true;
                p.NormalUpdate = true;
                p.NoLightEmittence = false;
                p.TimeToLive = 300;
            });

        }
    }
}