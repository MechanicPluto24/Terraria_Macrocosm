using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Common.CrossMod;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class NebulaRemnantProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            MoRHelper.AddElementToProjectile(Type, MoRHelper.Arcane);
            MoRHelper.AddElementToProjectile(Type, MoRHelper.Celestial);
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9f;
            Projectile.rotation = 0f;

            if (Projectile.timeLeft < 50)
                Projectile.Opacity -= 0.02f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, (float)(Projectile.scale * ((Math.Sin(Main.time * 3f) * 0.2f) + 1.2f)), Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

            return false;
        }

    }
}
