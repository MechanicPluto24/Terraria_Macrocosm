using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon.Sentries;

public class MoonChampionSentryBullet : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 2;

        ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
        ProjectileID.Sets.Explosive[Type] = false;

        ProjectileSets.HitsTiles[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(14);
        AIType = -1;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 270;
        Projectile.light = 0f;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.alpha=0;
    }

    public override bool PreAI()
    {
        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            Projectile.PrepareBombToBlow();

        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity.Y += 0.01f;
            Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Flare, Main.rand.NextFloat(), Main.rand.NextFloat(), 100, default, 1.1f);
            dust.noGravity = true;
            dust.velocity *= 0f;
        Projectile.alpha=0;



        return false;
    }

    public override void PrepareBombToBlow()
    {
        Projectile.tileCollide = false;
        Projectile.alpha = 255;
        Projectile.Resize(128, 128);
        Projectile.knockBack = 8f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.timeLeft = 2;
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.dedServ)
            return;

        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

        // Spawn smoke dusts
        for (int i = 0; i < 30; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.oldVelocity, 1, 1, DustID.Smoke, Main.rand.NextFloat(), Main.rand.NextFloat(), 100, default, 1.5f);
            dust.velocity *= 1.4f;
        }

        //Spawn flare dusts
        for (int i = 0; i < 20; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.oldVelocity, 1, 1, DustID.Flare, Main.rand.NextFloat(), Main.rand.NextFloat(), 100, default, 1.1f);
            dust.noGravity = true;
            dust.velocity *= 7f;

            dust = Dust.NewDustDirect(Projectile.Center + Projectile.oldVelocity, 1, 1, DustID.Flare, Main.rand.NextFloat(), Main.rand.NextFloat(), 100, default, 0.8f);
            dust.velocity *= 3f;
            dust.noGravity = true;
        }

        //Spawn trail dust
        for (int i = 3; i < Projectile.oldPos.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 pos = Projectile.oldPos[i];
                Dust dust = Dust.NewDustDirect(pos, 20, 20, DustID.Torch, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, Scale: 0.12f * (Projectile.oldPos.Length - i));
                dust.noGravity = true;
            }
        }

        var explosion = Particle.Create<TintableExplosion>(p =>
        {
            p.Position = Projectile.Center;
            p.Color = (new Color(195, 115, 62)).WithOpacity(0.4f);
            p.Scale = new(0.9f);
            p.NumberOfInnerReplicas = 9;
            p.ReplicaScalingFactor = 0.5f;
        });

        // Spawn Smoke particles
        for (int i = 0; i < 2; i++)
        {
            Vector2 velocity = Main.rand.NextVector2CircularEdge(2, 2) * (i == 1 ? 0.8f : 0.4f);
            Particle.Create<Smoke>(Projectile.Center, velocity, scale: new(1.2f)).VanillaUpdate = true;
        }
    }

}
