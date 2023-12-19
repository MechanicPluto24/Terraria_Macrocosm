using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    public class PhantasmalBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = spawnTimeLeft;
            CooldownSlot = 1;
        }

        int spawnTimeLeft = 480;
        bool spawned = false;
        public override void AI()
        {
            if (!spawned)
            {
                // TODO: sound
                spawned = true;
            }

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X) - MathHelper.PiOver2;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            }


            if (Projectile.timeLeft > spawnTimeLeft * 0.2f)
                Projectile.alpha -= (byte)(Projectile.velocity.Length() * 0.7f);
            else
                Projectile.alpha += (byte)(Projectile.velocity.Length() * 0.5f);

            Projectile.alpha = (byte)MathHelper.Clamp(Projectile.alpha, 0, 255);

            if (++Projectile.frameCounter >= 9)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.frame = 0; 
            }

            if(Main.rand.NextFloat(0.1f, 1f) > Projectile.alpha / 255f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - new Vector2(25,0).RotatedBy(Projectile.velocity.ToRotation()), ModContent.DustType<XaocGreenDust>(), Projectile.velocity , Alpha: 100, Scale: 0.7f);
                dust.noLight = true;
                dust.noGravity = true;
            }         
        }

        public override Color? GetAlpha(Color lightColor) => Color.White.WithOpacity(0.5f) * (1f - Projectile.alpha / 255f);

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
