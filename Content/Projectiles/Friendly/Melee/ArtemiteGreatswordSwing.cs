using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	public class ArtemiteGreatswordSwing : ModProjectile
	{
		public override string Texture => Macrocosm.TexturesPath + "Swing";

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

		protected Vector2 PositionAdjustment => new Vector2(55 * Projectile.scale, 0).RotatedBy(Projectile.rotation);

		public ref float SwingDirection => ref Projectile.ai[0];
		public ref float TargetSwingRotation => ref Projectile.ai[1];
		public ref float SwingRotation => ref Projectile.ai[2];

        bool spawned = false;
        float defScale;
    
        public override void AI()
		{
            if (!spawned)
            {
                defScale = Projectile.scale;
                spawned = true;
            }

			float progress = SwingRotation / TargetSwingRotation;
			Player player = Main.player[Projectile.owner];
			Item item = player.CurrentItem();

			float speed = 0.6f * player.GetTotalAttackSpeed(DamageClass.Melee);
			SwingRotation += speed;

            float angle = MathHelper.Pi;
            Projectile.rotation = angle * SwingDirection * progress + Projectile.velocity.ToRotation() + SwingDirection * angle + player.fullRotation;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + PositionAdjustment;

			Projectile.scale = 2f * player.GetAdjustedItemScale(item) * defScale;

			Vector2 hitboxPos = Projectile.Center - PositionAdjustment + Utility.PolarVector(175, Projectile.rotation);

            for (int i = 0; i < 2; i++)
            {
                if (progress < 0.15f)
                    break;

                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(1, 20 * speed * progress), 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
                Dust dust = Dust.NewDustDirect(Vector2.Lerp(hitboxPos, player.Center, Main.rand.NextFloat()), 1, 1, ModContent.DustType<ArtemiteBrightDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(1.2f, 2f));
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.CreateParticle<ArtemiteStar>(target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2), -Vector2.UnitY * 0.4f, 1f, 0f, shouldSync: true);
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Player player = Main.player[Projectile.owner];

			Rectangle frame = texture.Frame(1, 4, frameY: 3);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - PositionAdjustment - Main.screenPosition;
			SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

			float progress = SwingRotation / TargetSwingRotation;
			float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			Color color = new Color(130, 220, 199).WithOpacity(1f - progress) * 1.2f; 

			float scale = Projectile.scale;
			float rotation = Projectile.rotation;
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color * progressScale, rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, scale * 0.95f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), color * 0.15f, rotation, origin, scale, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, rotation, origin, scale, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, rotation, origin, scale * 0.975f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.4f - 0.39f * (1f - progressScale)), rotation, origin, scale * 0.95f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.5f - 0.49f * (1f - progressScale)), rotation, origin, scale * 0.75f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.05f - 0.04f * (1f - progressScale)), rotation, origin, scale * 0.55f, effects, 0f);
            int iteration = 0;
            for (float f = 0f; f < 16f; f += 0.04f)
            {
                float starProgress = f / 16f;

                float angle = Projectile.rotation + SwingDirection * (f - 2f) * ((float)Math.PI * -2f) * 0.025f + MathHelper.Pi * 0.17f * SwingDirection;
                Vector2 drawpos = position + angle.ToRotationVector2() * ((float)frame.Width * 0.5f - 8f) * Projectile.scale;
                Utility.DrawSwingEffectStar(1f, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0) * progressScale, color * progressScale, progress, 0f, 0.5f, 1f, 1f, angle, new Vector2(0.1f + 0.2f * starProgress, 1.1f) * (1f - starProgress) * progress, Vector2.One * 0.7f);

                if (iteration++ == 0)
                    Utility.DrawSwingEffectStar(1f, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0), color, progress, 0f, 0.5f, 1f, 1f, angle, new Vector2(0.2f, 1.2f) * (1f - starProgress), new Vector2(10f, 20f));
            }

            return false;
		}
    }

	public class ArtemiteGreatswordSwing2 : ArtemiteGreatswordSwing
	{
        bool spawned = false;
        float defScale;

        public override void AI()
        {
            if (!spawned)
            {
                defScale = Projectile.scale;
                spawned = true;
            }

            float progress = SwingRotation / TargetSwingRotation;
            Player player = Main.player[Projectile.owner];
            Item item = player.CurrentItem();

            float speed = 0.6f * player.GetTotalAttackSpeed(DamageClass.Melee);
            SwingRotation += speed;

            float angle = MathHelper.Pi - MathHelper.Pi/16;
            Projectile.rotation = angle * SwingDirection * progress + Projectile.velocity.ToRotation() + SwingDirection * angle + player.fullRotation;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + PositionAdjustment;

            Projectile.scale = 2.6f * player.GetAdjustedItemScale(item) * defScale;

            Vector2 hitboxPos = Projectile.Center - PositionAdjustment + Utility.PolarVector(175, Projectile.rotation);

            for (int i = 0; i < 2; i++)
            {
                if (progress < 0.15f)
                    break;

                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(1, 20 * speed * progress), 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
                Dust dust = Dust.NewDustDirect(Vector2.Lerp(hitboxPos, player.Center, Main.rand.NextFloat()), 1, 1, ModContent.DustType<ArtemiteBrightDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(1.2f, 2f));
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Player player = Main.player[Projectile.owner];

            Rectangle frame = texture.Frame(1, 4, frameY: 3);
            Vector2 origin = frame.Size() / 2f;

            Vector2 position = Projectile.Center - PositionAdjustment - Main.screenPosition;
            SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

            float progress = SwingRotation / TargetSwingRotation;
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

            float opacity = (1f - Projectile.alpha / 255f);
            Color color = new Color(81, 180, 114).WithOpacity(1f - progress) * opacity; 

            float scale = Projectile.scale;
            float rotation = Projectile.rotation;
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color * progressScale, rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, scale * 0.95f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), color * 0.15f, rotation, origin, scale, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, rotation, origin, scale, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, rotation, origin, scale * 0.975f, effects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.4f - 0.39f * (1f - progressScale)), rotation, origin, scale * 0.95f, effects, 0f);
            //Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.5f - 0.49f * (1f - progressScale)), rotation, origin, scale * 0.75f, effects, 0f);
            //Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.05f - 0.04f * (1f - progressScale)), rotation, origin, scale * 0.55f, effects, 0f);

            int iteration = 0;
            for (float f = 0f; f < 16f * (0.5f + opacity); f += 0.02f)
            {
                float starProgress = f / 16f;

                float angle = rotation + SwingDirection * (f - 2f) * (MathHelper.Pi * -2.2f) * 0.025f + MathHelper.Pi * 0.17f * SwingDirection;
                Vector2 drawpos = position + angle.ToRotationVector2() * ((float)frame.Width * 0.5f - 7f) * scale;
                Utility.DrawSwingEffectStar(1f, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0) * progressScale * opacity, color * progressScale, progress, 0f, 0.5f, 1f, 1f, angle, new Vector2(0.1f + 0.2f * starProgress, 1f) * (1f - starProgress) * progress, Vector2.One * 1.5f);

                if (iteration++ == 0)
                    Utility.DrawSwingEffectStar(1f, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0) * opacity, color, progress, 0f, 0.5f, 1f, 1f, angle, new Vector2(0.2f, 1.2f) * (1f - starProgress), new Vector2(10f, 20f));
            }

            return false;
        }
    }
}