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

	public class test : ModProjectile
	{
		public override string Texture => "Macrocosm/Assets/Textures/Swing";

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/Swing").Value;

			ProjectileID.Sets.TrailCacheLength[Type] = 12;
			ProjectileID.Sets.TrailingMode[Type] = 3;

			Projectile.rotation = Projectile.velocity.ToRotation();

			Rectangle frame = texture.Frame(1, 4, frameY: 0);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - Main.screenPosition;
			Color color = new Color(130, 220, 199);// * lightColor.GetLuminance();

			SpriteEffects effects = Projectile.velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			//Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color, Projectile.rotation, origin, Projectile.scale * 0.95f, effects, 0f);
			
			for(int i = 0; i < Projectile.oldPos.Length; i++)
			{
				float progress = (float)i/ Projectile.oldPos.Length;
				Main.EntitySpriteDraw(texture, Projectile.oldPos[i] - Main.screenPosition, texture.Frame(1, 4, frameY: 1), color * (0.4f - progress), Projectile.rotation - 0.1f * Projectile.direction, origin, Projectile.scale * 0.8f, effects, 0f);
				Main.EntitySpriteDraw(texture, Projectile.oldPos[i] - Main.screenPosition, texture.Frame(1, 4, frameY: 2), color * (0.3f - progress), Projectile.rotation - 0.1f * Projectile.direction, origin, Projectile.scale * 1f, effects, 0f);
			}


			return false;
		}
	}


	public class ArtemiteSwordSwing : ModProjectile
	{
		public override string Texture => "Macrocosm/Assets/Textures/Swing";

		public override void SetDefaults()
		{
			Projectile.width = 110;
			Projectile.height = 110;
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

		private Vector2 positionAdjustment => new Vector2(55 * Projectile.scale, 0).RotatedBy(Projectile.rotation);

		bool shot = false;
		public override void AI()
		{
			float scaleFactor = 0.5f;
			float baseScale = 0.4f;
			float direction = Projectile.ai[0];
			float progress = Projectile.localAI[0] / Projectile.ai[1];
			Player player = Main.player[Projectile.owner];
			Item item = player.HeldItem;
			float speed = player.GetTotalAttackSpeed(DamageClass.Melee);

			Projectile.localAI[0] += 16f * speed;

			Projectile.rotation = (float)Math.PI * direction * progress + Projectile.velocity.ToRotation() + direction * (float)Math.PI + player.fullRotation;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionAdjustment;

			Projectile.scale = baseScale + Utility.CubicEaseInOut(progress) * scaleFactor;
			Projectile.scale *= player.GetAdjustedItemScale(item);
			Projectile.scale *= 1.5f;

			Vector2 hitboxPos = Projectile.Center - positionAdjustment + Utility.PolarVector(175, Projectile.rotation);

			for (int i = 0; i < (int)(1 * (Projectile.scale < 0.5f ? 0 : 1)); i++)
			{
				//Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(1, 10 * speed), 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
				//Dust dust = Dust.NewDustDirect(hitboxPos, 1, 1, ModContent.DustType<ArtemiteBrightDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(2f, 3f));
				//dust.noGravity = true;

				Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(4, 6), 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
				Dust dust = Dust.NewDustDirect(Vector2.Lerp(Projectile.position, player.Center, 0.5f), Projectile.width / 2, Projectile.height / 2, ModContent.DustType<ArtemiteDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(0.6f, 1f)); ;
				dust.noGravity = true;
			}

			if (progress >= 0.55f && !shot && Projectile.owner == Main.myPlayer)
			{
				shot = true;
				float aimAngle = (Main.MouseWorld - player.Center).ToRotation() + 0.12f * Projectile.direction;
				Vector2 velocity = Utility.PolarVector(25, aimAngle);
				Terraria.Projectile.NewProjectile(null, Projectile.Center, velocity, ModContent.ProjectileType<test>(), Projectile.damage, 0f);
			}

			if (Projectile.localAI[0] >= Projectile.ai[1] + 1)
			{
				Projectile.Kill();
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Texture2D star = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/Star1").Value;

			Rectangle frame = texture.Frame(1, 4, frameY: 3);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - positionAdjustment - Main.screenPosition;
			SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			Color color = new Color(130, 220, 199).NewAlpha(1f-progress) * (0.2f * progress);// * lightColor.GetLuminance();

			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, Projectile.scale * 0.95f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), color, Projectile.rotation, origin, Projectile.scale, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color, Projectile.rotation, origin, Projectile.scale, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color, Projectile.rotation, origin, Projectile.scale * 0.975f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), color, Projectile.rotation, origin, Projectile.scale * 0.95f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), color, Projectile.rotation, origin, Projectile.scale * 0.75f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), color, Projectile.rotation, origin, Projectile.scale * 0.55f, effects, 0f);

			return false;
		}
	}
}