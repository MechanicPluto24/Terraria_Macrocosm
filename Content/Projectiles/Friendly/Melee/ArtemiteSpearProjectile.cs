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
			Projectile.width = 42;
			Projectile.height = 42;
			Projectile.scale = 0.95f;
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

			if (Projectile.timeLeft == halfDuration)
			{
				Vector2 shootPosition = Projectile.Center;
				Vector2 shootVelocity = Projectile.velocity * 15f;
				//Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<ArtemiteSpearProjectileShoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			}

			// Spawn dust on use
			if (!Main.dedServ)
			{
				if (Main.rand.NextBool(3))
				{
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Scale: 0.6f);
				}
			}

			return false; // Don't execute vanilla AI.
		}

		// Clean this up lol
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];  
            float a = Utils.Remap(player.itemAnimation, player.itemAnimationMax, player.itemAnimationMax / 3, 0f, 1f);
            float b = 20f;
			float c = 50f;
			float d = 20f;
			float e = 30f;
            c *= 1f / (1f - player.GetAttackSpeed(DamageClass.Melee));
            float num7 = b + c * a;
            float num8 = d + e * a;
            float f = Projectile.velocity.ToRotation();
            Vector2 center = Projectile.Center + f.ToRotationVector2() * num7;
            Vector2 vector2 = Projectile.Center + new Vector2(0f, Projectile.gfxOffY);
            SpriteEffects dir = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                dir = SpriteEffects.FlipHorizontally;
            Rectangle extensionBox = Utils.CenteredRectangle(center, new Vector2(num8, num8));
            Vector2 value2 = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            float num3 = extensionBox.Size().Length() / Projectile.Hitbox.Size().Length();
            float num4 = Utils.Remap(player.itemAnimation, player.itemAnimationMax, (float)player.itemAnimationMax / 3f, 0f, 1f);
            float num5 = Utils.Remap(num4, 0f, 0.3f, 0f, 1f) * Utils.Remap(num4, 0.3f, 1f, 1f, 0f);
            num5 = 1f - (1f - num5) * (1f - num5);
            Vector2 vector3 = Projectile.Center + new Vector2(0f, Projectile.gfxOffY);
            Vector2.Lerp(value2, vector3, 1.5f);
            Texture2D value3 = TextureAssets.Extra[98].Value;
            Vector2 origin = value3.Size() / 2f;
			Color color = new Color(130, 220, 199,0);

            float num6 = (Projectile.velocity.ToRotation() + MathHelper.PiOver2);

            Main.EntitySpriteDraw(value3, Vector2.Lerp(vector3, vector2, 0.5f) - Main.screenPosition, null, color * num5, num6, origin, new Vector2(num5 * num3, num3) * Projectile.scale * num3, dir);
            Main.EntitySpriteDraw(value3, Vector2.Lerp(vector3, vector2, 1f) - Main.screenPosition, null, color * num5, num6, origin, new Vector2(num5 * num3, num3 * 1.5f) * Projectile.scale * num3, dir);
            Main.EntitySpriteDraw(value3, Vector2.Lerp(value2, vector2, num4 * 1.5f - 0.5f) - Main.screenPosition + new Vector2(0f, 2f), null, color * num5, num6, origin, new Vector2(num5 * num3 * 1f * num5, num3 * 2f * num5) * Projectile.scale * num3, dir);
            for (float x = 0.4f; x <= 1f; x += 0.1f)
            {
                Vector2 vector4 = Vector2.Lerp(value2, vector3, x + 0.2f);
                Main.EntitySpriteDraw(value3, vector4 - Main.screenPosition + new Vector2(0f, 2f), null, color * num5 * 0.75f * x, num6, origin, new Vector2(num5 * num3 * 1f * num5, num3 * 2f * num5) * Projectile.scale * num3, dir);
            }

            return base.PreDraw(ref lightColor);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
        }


    }
}
