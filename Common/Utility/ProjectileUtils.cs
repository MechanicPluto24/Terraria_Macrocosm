using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Global.GlobalProjectiles;
using Macrocosm.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;


namespace Macrocosm.Common.Utility
{
	public static class ProjectileUtils
	{
		public static void SetTrail<T>(this Projectile projectile) where T : Trail
		{
			if(projectile.TryGetGlobalProjectile(out MacrocosmGlobalProjectile globalProj))
			{
 				globalProj.Trail = Activator.CreateInstance<T>();
				globalProj.Trail.Owner = projectile;
			}
 		}

		public static Trail GetTrail(this Projectile projectile) => projectile.GetGlobalProjectile<MacrocosmGlobalProjectile>().Trail; 

		public static void Explode(this Projectile projectile, float blastRadius)
		{
			projectile.tileCollide = false;
			projectile.timeLeft = 2; 
			projectile.penetrate = -1;
			projectile.alpha = 255;
			
			projectile.position.X += projectile.width / 2;
			projectile.position.Y += projectile.height / 2;
			projectile.width = (int)blastRadius;
			projectile.height = (int)blastRadius;
			projectile.position.X -= projectile.width / 2;
			projectile.position.Y -= projectile.height / 2;

			projectile.netUpdate = true;
		}

		/// <summary>
		/// Draws an animated projectile, leave texture null to draw as entity with the loaded texture
		/// (Only tested for held projectiles)  
		/// </summary>
		/// <param name="proj"> Projectile instance to draw </param>
		/// <param name="lightColor"> Computed environment color </param>
		/// <param name="drawOffset"> Offset to draw from texture center at 0 rotation </param>
		/// <param name="texture"> Leave null to draw as entity with the loaded texture </param>
		public static void DrawAnimated(this Projectile proj, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default, Texture2D texture = null)
		{
			bool drawEntity = false;

			if (texture is null)
			{
				texture = TextureAssets.Projectile[proj.type].Value;
				drawEntity = true;
			}

			Vector2 position = proj.Center - Main.screenPosition;

			int numFrames = Main.projFrames[proj.type];
			Rectangle sourceRect = texture.Frame(1, numFrames, frameY: proj.frame);

			Vector2 origin = new Vector2(texture.Width / 2, texture.Height / numFrames / 2) - new Vector2(drawOffset.X, drawOffset.Y * proj.spriteDirection);

			if (drawEntity)
				Main.EntitySpriteDraw(texture, position, sourceRect, lightColor, proj.rotation, origin, proj.scale, effect, 0);
			else
				Main.spriteBatch.Draw(texture, position, sourceRect, lightColor, proj.rotation, origin, proj.scale, effect, 0f);
		}

		/// <summary>
		/// Draws an animated projectile glowmask
		/// (Only tested for held projectiles)  
		/// </summary>
		public static void DrawAnimatedGlowmask(this Projectile proj, Texture2D glowmask, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default)
			=> proj.DrawAnimated(lightColor, effect, drawOffset + new Vector2(0, -2), glowmask);
	}
}
