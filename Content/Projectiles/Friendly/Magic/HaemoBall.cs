using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class HaemoBall : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.hostile = false;

        Projectile.width = 27;
        Projectile.height = 24;

        Projectile.timeLeft = 600;
        Projectile.penetrate = 1;
        Projectile.tileCollide = true;

        Projectile.aiStyle = ProjAIStyleID.Arrow;
    }

    int particleTime = 10;
    int timer = 0;
    public override void AI()
    {
        Projectile.velocity.Y += 0.2f;
        Projectile.frameCounter++;
        timer++;
        if (timer >= particleTime)
        {
            // spawn particle
            Vector2 velocity = Main.rand.NextVector2Circular(0.2f, 0.2f);
            Dust.NewDust(Projectile.Center, 10, 10, DustID.Blood, velocity.X, velocity.Y);
            particleTime = Main.rand.Next(2, 10);
            timer = 0;
        }
        if (Projectile.frameCounter < 5)
        {
            Projectile.frame = 0;
        }
        else if (Projectile.frameCounter < 10)
        {
            Projectile.frame = 1;
        }
        else if (Projectile.frameCounter < 15)
        {
            Projectile.frame = 2;
        }
        else if (Projectile.frameCounter < 20)
        {
            Projectile.frame = 3;
        }
        else
        {
            Projectile.frameCounter = 0;
        }
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 200; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
            Dust.NewDust(Projectile.Center, 20, 20, DustID.Blood, velocity.X, velocity.Y);
        }
        for (int i = 0;i < 20; i++)
        {
            Vector2 velocityCone = Main.rand.NextVector2Circular(8f, 8f);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocityCone, ModContent.ProjectileType<HaemoTear>(), Projectile.damage / 2, 1);
        }
    }
}
