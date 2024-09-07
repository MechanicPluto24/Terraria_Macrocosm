using Macrocosm.Common.Drawing.Particles;
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

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteSwordShoot : ModProjectile
    {
        public override string Texture => Macrocosm.TexturesPath + "Swing";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        /*
        public ref float Timer => ref Projectile.localAI[0];
        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];
        public ref float Scale => ref Projectile.ai[2];
        */

        public override void AI()
        {
            Projectile.direction = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.2f * Projectile.direction;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return null;
        }

        public override void CutTiles()
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.CreateParticle<ArtemiteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = 1f;
                p.Rotation = MathHelper.PiOver4;
            }, shouldSync: true
            );
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Particle.CreateParticle<ArtemiteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = 1f;
                p.Rotation = MathHelper.PiOver4;
            }, shouldSync: true
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale * 1.2f;
            SpriteEffects spriteEffects = ((!(Projectile.velocity.X >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.
            float progress = 0.5f;
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

            Color color = new Color(130, 220, 199) * 1.4f;
            Color backDarkColor = color;
            Color middleMediumColor = color;
            Color frontLightColor = color ;

            // Back part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), backDarkColor , Projectile.rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -1f * (1f - progress), origin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), middleMediumColor  * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), frontLightColor * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = Projectile.rotation + Projectile.spriteDirection * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.spriteDirection;
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 6f) * scale;
                Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0)  * (i / 9f), middleMediumColor, progress, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            return false;
        }
    }
}