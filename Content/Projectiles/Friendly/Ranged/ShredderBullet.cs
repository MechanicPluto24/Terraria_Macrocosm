using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class ShredderBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.scale = 1.2f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 270;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.penetrate = -1;
        }

        public override bool PreAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(new Vector2(0, 0), 4f, 0f, new Color(197, 85, 110), new Color(255, 171, 208, 255));
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Enemies tagged as Slime take guaranteed critical hits. 
            if (NPCSets.Material[target.type] is NPCMaterial.Slime)
            {
                modifiers.SetCrit();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Damages enemies tagged as Organic and Demon three times per hit.
            if (NPCSets.Material[target.type] is NPCMaterial.Organic or NPCMaterial.Demon)
            {
                for (int i = 0; i < 2; i++)
                    target.StrikeNPC(hit);

                target.AddBuff(BuffID.Bleeding, 120);
            }
        }

        //public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
