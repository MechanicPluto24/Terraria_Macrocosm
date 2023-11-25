using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteSwordSwing : ModProjectile
    {
        public override string Texture => Macrocosm.TextureAssetsPath + "Swing";

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
        }

        private Vector2 PositionAdjustment => new Vector2(55 * Projectile.scale, 0).RotatedBy(Projectile.rotation);

        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float TargetSwingRotation => ref Projectile.ai[1];
        public ref float SwingRotation => ref Projectile.ai[2];


        public override void AI()
        {
            float scaleFactor = 1.8f;
            float baseScale = 0f;
            float progress = SwingRotation / TargetSwingRotation;
            Player player = Main.player[Projectile.owner];
            Item item = player.HeldItem;
            float speed = player.GetTotalAttackSpeed(DamageClass.Melee);

            SwingRotation += 1.75f * speed;

            Projectile.rotation = (float)Math.PI * SwingDirection * progress + Projectile.velocity.ToRotation() + SwingDirection * (float)Math.PI + player.fullRotation;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + PositionAdjustment;

            Projectile.scale = baseScale + MathHelper.SmoothStep(0, 1, progress) * scaleFactor;
            Projectile.scale *= player.GetAdjustedItemScale(item);
            Projectile.scale *= 1.5f;

            Vector2 hitboxPos = Projectile.Center - PositionAdjustment + Utility.PolarVector(200, Projectile.rotation);

            for (int i = 0; i < (int)(10 * (progress < 0.3f ? 0 : progress)); i++)
            {
                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(1, 20 * speed * progress), 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
                Dust dust = Dust.NewDustDirect(hitboxPos, 1, 1, ModContent.DustType<ArtemiteBrightDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(2f, 3f));
                dust.noGravity = true;

                if (Main.rand.NextBool(4))
                {
                    dustVelocity = new Vector2(Main.rand.NextFloat(4, 6), 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
                    dust = Dust.NewDustDirect(Vector2.Lerp(Projectile.position, player.Center, 0.5f), Projectile.width / 2, Projectile.height / 2, ModContent.DustType<ArtemiteDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(0.6f, 1f)); ;
                    dust.noGravity = true;
                }
            }

            if (SwingRotation >= TargetSwingRotation + 1)
                Projectile.Kill();
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            //Texture2D star = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Star1").Value;
            Texture2D star = TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value;
            Player player = Main.player[Projectile.owner];

            Rectangle frame = texture.Frame(1, 4, frameY: 3);
            Vector2 origin = frame.Size() / 2f;

            Vector2 position = Projectile.Center - PositionAdjustment - Main.screenPosition;
            SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

            float progress = SwingRotation / TargetSwingRotation;
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

            Color color = new Color(130, 220, 199).WithOpacity(1f - progress);// * lightColor.GetLuminance();

            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color * progressScale, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, Projectile.scale * 0.95f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), color * 0.15f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, Projectile.rotation, origin, Projectile.scale * 0.975f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (Color.White * (0.2f - 0.2f * progressScale)).WithOpacity(0.4f - 0.4f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.95f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.2f - 0.2f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.75f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.1f - 0.05f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.55f, effects, 0f);


            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            float rotation = Projectile.rotation + (MathHelper.PiOver4 / 2f * player.direction);
            Main.EntitySpriteDraw(star, player.Center + Utility.PolarVector(100, rotation) - Main.screenPosition, null, new Color(168, 215, 215) * Projectile.scale * (0.4f + 0.6f * progress) * 0.6f, Projectile.rotation + MathHelper.PiOver4, star.Size() / 2f, 1.2f, SpriteEffects.None);
            Main.EntitySpriteDraw(star, player.Center + Utility.PolarVector(100, rotation) - Main.screenPosition, null, new Color(168, 215, 215) * Projectile.scale * (0.4f + 0.6f * progress) * 0.6f, Projectile.rotation - MathHelper.PiOver4, star.Size() / 2f, 1.2f, SpriteEffects.None);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            return false;
        }
    }
}