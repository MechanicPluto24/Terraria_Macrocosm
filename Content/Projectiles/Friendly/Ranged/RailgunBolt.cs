using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class RailgunBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 270;
            Projectile.friendly = true;

            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.extraUpdates = 3;
        }

        private bool killed;
        public override void AI()
        {
            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(0f - Projectile.velocity.Y, 0f - Projectile.velocity.X) - MathHelper.PiOver2;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            }

            if (killed)
            {
                Projectile.Opacity -= 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FalseKill();
            return false;
        }

        private void FalseKill()
        {
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.velocity = default;
            Projectile.damage = 0;
            Projectile.extraUpdates = 0;
            killed = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 127);
        public override bool PreDraw(ref Color lightColor)
        {
            int trailCount = 12;
            float distanceMult = 0.4f;
            for (int n = 0; n < trailCount; n++)
            {
                Vector2 trailPosition = Projectile.Center - new Vector2(10, 0).RotatedBy(Projectile.velocity.ToRotation()) * n * distanceMult + Main.rand.NextVector2Circular(5, 5);
                Color glowColor = (GetAlpha(lightColor) ?? default) * (((float)trailCount - n) / trailCount) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, glowColor, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
