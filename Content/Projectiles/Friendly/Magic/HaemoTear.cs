using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;
public class HaemoTear : ModProjectile
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

        Projectile.width = 12;
        Projectile.height = 13;

        Projectile.timeLeft = 600;
        Projectile.penetrate = 2;
        Projectile.tileCollide = true;

        Projectile.aiStyle = ProjAIStyleID.Arrow;
    }

    int particleTime = 5;
    int timer = 0;
    public override void AI()
    {
        Projectile.frameCounter++;
        timer++;
        if (timer >= particleTime)
        {
            // spawn particle
            Vector2 velocity = Main.rand.NextVector2Circular(0.2f, 0.2f);
            Dust.NewDust(Projectile.Center, 10, 10, DustID.Blood, velocity.X, velocity.Y);
            particleTime = Main.rand.Next(1, 3);
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
        for (int i = 0; i < 40; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f);
            Dust.NewDust(Projectile.Center, 20, 20, DustID.Blood, velocity.X, velocity.Y);
        }
    }
}
