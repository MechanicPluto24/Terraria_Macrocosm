using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteGreatswordSwing : ModProjectile
	{
		public override string Texture => "Macrocosm/Assets/Textures/Swing";

		public override void SetDefaults()
		{
			Projectile.width = 160;
			Projectile.height = 160;
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
			Vector2 pos = Projectile.position + new Vector2(80 * Projectile.scale,0).RotatedBy(Projectile.rotation);
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

			Projectile.localAI[0] += 1.4f;

			Projectile.rotation = (float)Math.PI * direction * progress + Projectile.velocity.ToRotation() + direction * (float)Math.PI + player.fullRotation;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);

			Projectile.scale = baseScale + progress * scaleFactor;
			Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.item[Main.player[Projectile.owner].selectedItem]);
			Projectile.scale *= 1.5f;

			if (Projectile.localAI[0] >= Projectile.ai[1])
 				Projectile.Kill();
 		}

		public override bool PreDraw(ref Color lightColor)
		{
			Asset<Texture2D> val = TextureAssets.Projectile[Type];

			Rectangle frame = val.Frame(1, 4, frameY: 3);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - Main.screenPosition;
			SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			Color color = new Color(130, 220, 199).NewAlpha(1f - progress) * lightColor.GetLuminance();
	
			Main.EntitySpriteDraw(val.Value, position, val.Frame(1, 4, frameY: 0), color * progressScale, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, Projectile.scale * 0.95f, effects, 0f);
			Main.EntitySpriteDraw(val.Value, position, val.Frame(1, 4, frameY: 1), color * 0.15f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
			Main.EntitySpriteDraw(val.Value, position, val.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
		    Main.EntitySpriteDraw(val.Value, position, val.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, Projectile.rotation, origin, Projectile.scale * 0.975f, effects, 0f);
		    Main.EntitySpriteDraw(val.Value, position, val.Frame(1, 4, frameY: 3), color * progressScale, Projectile.rotation, origin, Projectile.scale * 0.95f, effects, 0f);
			
			return false;
		}
	}
}