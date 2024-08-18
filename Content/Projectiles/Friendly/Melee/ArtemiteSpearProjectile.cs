using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteSpearProjectile : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 50f;
        protected virtual float HoldoutRangeMax => 160f;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 0.95f;

            Projectile.usesOwnerMeleeHitCD = true;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            float holdoutOffset = 0f;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
                holdoutOffset = 45f; // Some offset so the handle doesn't return all the way back to the player
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * (HoldoutRangeMin + holdoutOffset), Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            Projectile.rotation += MathHelper.ToRadians(Projectile.spriteDirection == -1 ? 40f : 135f);

            /*
			if (Projectile.timeLeft == halfDuration)
			{
				Vector2 shootPosition = Projectile.Center;
				Vector2 shootVelocity = Projectile.velocity * 15f;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<ArtemiteSpearProjectileShoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			}
			*/

            // Spawn dust on use
            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(3))
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Scale: 0.6f);

                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteBrightDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Scale: 0.6f);
                    dust.noGravity = true;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.CreateParticle<ArtemiteStar>((p) =>
            {
                p.Position = Projectile.Center;
                p.Velocity = new Vector2(3f, 0).RotatedBy(Projectile.velocity.ToRotation());
                p.Scale = 1.2f;
                p.Rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                p.StarPointCount = 1;
                p.FadeInFactor = 1.2f;
                p.FadeOutFactor = 0.7f;
            }, shouldSync: true
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            float halfDuration = player.itemAnimationMax * 0.5f;
            float animationProgress = Projectile.timeLeft < halfDuration ? Projectile.timeLeft / halfDuration : (player.itemAnimationMax - Projectile.timeLeft) / halfDuration;

            Vector2 effectCenter = Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * (20f + 50f * 1f / (1f - player.GetAttackSpeed(DamageClass.Melee)) * animationProgress);
            Rectangle effectArea = Utils.CenteredRectangle(effectCenter, new Vector2(20f + 30f * animationProgress));
            float effectScale = effectArea.Size().Length() / Projectile.Hitbox.Size().Length();

            float fadeInOutProgress = 1f - (1f - Utils.Remap(animationProgress, 0f, 0.3f, 0f, 1f) * Utils.Remap(animationProgress, 0.3f, 1f, 1f, 0f)) * (1f - Utils.Remap(animationProgress, 0f, 0.3f, 0f, 1f) * Utils.Remap(animationProgress, 0.3f, 1f, 1f, 0f));
            Color effectColor = new Color(130, 220, 199, 0) * fadeInOutProgress;
            float rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            SpriteEffects spriteEffect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0f, Projectile.gfxOffY) - Main.screenPosition, null, effectColor, rotation, texture.Size() / 2f, new Vector2(fadeInOutProgress * effectScale, effectScale) * Projectile.scale * effectScale, spriteEffect);

            for (float progress = 0.4f; progress <= 1f; progress += 0.4f)
                Main.EntitySpriteDraw(texture, Vector2.Lerp(player.MountedCenter, Projectile.Center + new Vector2(0f, Projectile.gfxOffY), progress + 0.2f) - Main.screenPosition + new Vector2(0f, 0f), null, effectColor * 0.75f * progress, rotation, texture.Size() / 2f, new Vector2(fadeInOutProgress * effectScale * fadeInOutProgress, effectScale * 2f * fadeInOutProgress) * Projectile.scale * effectScale, spriteEffect);

            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
            Player player = Main.player[Projectile.owner];
            float animationProgress = Utils.Remap(player.itemAnimation, player.itemAnimationMax, player.itemAnimationMax / 3, 0f, 1f);
            float fadeInOutProgress = 1f - (1f - Utils.Remap(animationProgress, 0f, 0.3f, 0f, 1f) * Utils.Remap(animationProgress, 0.3f, 1f, 1f, 0f)) * (1f - Utils.Remap(animationProgress, 0f, 0.3f, 0f, 1f) * Utils.Remap(animationProgress, 0.3f, 1f, 1f, 0f));
            hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2() + Utility.PolarVector(50 * fadeInOutProgress, Projectile.velocity.ToRotation()), new Vector2(hitbox.Width, hitbox.Height));
        }
    }
}
