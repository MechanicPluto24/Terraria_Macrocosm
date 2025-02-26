using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.NPCs.Enemies.Pollution;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class TrashnadoProjectile : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }

        private bool spawned;
        private Trashnado.TrashEntity trashEntity;
        public override void AI()
        {
            if (!spawned)
            {
                trashEntity = Trashnado.TrashEntityRandomPool.Get();
                spawned = true;
            }

            Projectile.velocity.Y += MacrocosmSubworld.GetGravityMultiplier() / 4;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                var particle = Particle.Create<TextureDustParticle>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Velocity = Main.rand.NextVector2Circular(2f, 2f);
                    p.Scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f));
                    p.ScaleVelocity = new Vector2(Main.rand.NextFloat(-0.1f, -0.01f));
                    p.Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    p.SourceTexture = trashEntity.Texture;
                    p.TimeToLive = 45;
                    p.Scale = new(1.6f);
                });
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(trashEntity.Texture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Lerp(lightColor, Color.White, 1f - Projectile.alpha / 255f)), Projectile.rotation, trashEntity.Texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
