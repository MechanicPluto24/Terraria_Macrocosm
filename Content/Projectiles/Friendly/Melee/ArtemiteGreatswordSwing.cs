using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteGreatswordSwing : ModProjectile
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
			Vector2 pos = Projectile.position + new Vector2(65 * Projectile.scale,0).RotatedBy(Projectile.rotation);
			hitbox.X = (int)pos.X;
			hitbox.Y = (int)pos.Y;
		}

		public override void AI()
		{
			float scaleFactor = 1.8f;
			float baseScale = 0.1f;
			float direction = Projectile.ai[0];
			float progress = Projectile.localAI[0] / Projectile.ai[1];
			Player player = Main.player[Projectile.owner];

			Projectile.localAI[0] += 1.3f;

			Projectile.rotation = (float)Math.PI * direction * progress + Projectile.velocity.ToRotation() + direction * (float)Math.PI + player.fullRotation;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);

			Projectile.scale = baseScale + Utility.QuadraticEaseInOut(progress) * scaleFactor;
			Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.item[Main.player[Projectile.owner].selectedItem]);
			Projectile.scale *= 1.5f;

			Rectangle hitbox = Projectile.GetDamageHitbox();
			Vector2 hitboxPos = hitbox.Center.ToVector2() - hitbox.Size()/4;

			for(int i = 0; i < (int)(6 * (Projectile.scale/3f)); i++)
			{
				Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(8, 22), 0).RotatedBy(Projectile.rotation + Main.rand.NextFloat(MathF.PI/2, MathF.PI/2 * 1.5f)) + Main.player[Projectile.owner].velocity;
				Dust dust = Dust.NewDustDirect(hitboxPos, 4, 4, ModContent.DustType<LuminiteDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(2f, 3f));
				dust.noGravity = true;
			}

			if (Projectile.localAI[0] >= Projectile.ai[1] + 1)
 				Projectile.Kill();
 		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Texture2D star = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/Star1").Value;

			Rectangle frame = texture.Frame(1, 4, frameY: 3);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - Main.screenPosition;
			SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			Color color = new Color(130, 220, 199).NewAlpha(1f - progress);// * lightColor.GetLuminance();
	
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color * progressScale, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, Projectile.scale * 0.95f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), color * 0.15f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, Projectile.rotation, origin, Projectile.scale * 0.975f, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (Color.White * progressScale).NewAlpha(0.4f - 0.2f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.95f, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).NewAlpha(0.2f - 0.2f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.75f, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).NewAlpha(0.1f - 0.05f *progressScale), Projectile.rotation, origin, Projectile.scale * 0.55f, effects, 0f);


			var state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			if (progress < 0.95f)
				for(int i = 0; i < 3; i++)
				{
					float scale = Projectile.scale * (0.05f - 0.01f * i);
					float angle = Projectile.rotation + (i * (MathHelper.PiOver4 / 2f));
					Main.EntitySpriteDraw(star, position + Utility.PolarVector(76 * Projectile.scale, angle), null, (new Color(168, 255, 255) * (1f + color.A/255f)).NewAlpha(1f), Projectile.rotation + MathHelper.PiOver4, star.Size() / 2f, scale, SpriteEffects.None);
				}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);
			return false;
		}
	}
}