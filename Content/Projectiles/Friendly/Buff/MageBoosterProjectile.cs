using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Buffs.MageBoosters;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Buff;

public class MageBoosterProjectile : ModProjectile
{
    public override string Texture => Macrocosm.EmptyTexPath;

    bool spawned = false;

    public int BoostType
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.scale = 1f;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 3600;
        Projectile.Opacity = 0f;
    }

    public override void AI()
    {
        if (!spawned)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                spawned = true;
                BoostType = Main.rand.Next(0, 3);
            }
        }

        Player player = Main.player[Projectile.owner];
        int dist = 3;
        Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
        float distFactor = Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist);
        float radians = (Projectile.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
        Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);

        Color color = BoostType switch
        {
            0 => new Color(255, 228, 90),
            1 => new Color(209, 90, 255),
            _ => new Color(107, 125, 255),
        };
        Particle.Create<DustParticle>(p =>
        {
            p.DustType = 43;
            p.Position = dustPosition;
            p.Velocity = velocity;
            p.Color = color;
            p.Acceleration = velocity * 1f;
            p.Scale = new(Main.rand.NextFloat(1f, 1.5f));
            p.NoGravity = true;
            p.NormalUpdate = true;
            p.NoLightEmittence = false;
            p.TimeToLive = 300;
        });

        if (Vector2.Distance(player.Center, Projectile.Center) < 200f)
            Projectile.velocity += (player.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 1f;
        else
            Projectile.velocity *= 0.95f;

        if (Vector2.Distance(player.Center, Projectile.Center) < 30f)
        {
            switch (BoostType)
            {
                case 0:
                    if (player.HasBuff(ModContent.BuffType<MageBoosterRegen1>()) || player.HasBuff(ModContent.BuffType<MageBoosterRegen2>()))
                    {
                        player.ClearBuff(ModContent.BuffType<MageBoosterRegen1>());
                        player.AddBuff(ModContent.BuffType<MageBoosterRegen2>(), 900);
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<MageBoosterRegen1>(), 900);
                    }
                    break;
                case 1:
                    if (player.HasBuff(ModContent.BuffType<MageBoosterMana1>()) || player.HasBuff(ModContent.BuffType<MageBoosterMana2>()))
                    {
                        player.ClearBuff(ModContent.BuffType<MageBoosterMana1>());
                        player.AddBuff(ModContent.BuffType<MageBoosterMana2>(), 900);
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<MageBoosterMana1>(), 900);
                    }
                    break;
                case 2:
                    if (player.HasBuff(ModContent.BuffType<MageBoosterDamage1>()) || player.HasBuff(ModContent.BuffType<MageBoosterDamage2>()))
                    {
                        player.ClearBuff(ModContent.BuffType<MageBoosterDamage1>());
                        player.AddBuff(ModContent.BuffType<MageBoosterDamage2>(), 900);
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<MageBoosterDamage1>(), 900);
                    }
                    break;
            }

            Projectile.Kill();
        }

    }
}