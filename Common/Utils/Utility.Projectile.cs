using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Content.Projectiles.Global;
using Macrocosm.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using System.Reflection;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
		public static void SetTrail<T>(this Projectile projectile) where T : VertexTrail
		{
			if(projectile.TryGetGlobalProjectile(out MacrocosmProjectile globalProj))
			{
 				globalProj.Trail = Activator.CreateInstance<T>();
				globalProj.Trail.Owner = projectile;
			}
 		}

		public static VertexTrail GetTrail(this Projectile projectile) => projectile.GetGlobalProjectile<MacrocosmProjectile>().Trail;
		public static bool TryGetTrail(this Projectile projectile, out VertexTrail trail)
		{
			trail = projectile.GetGlobalProjectile<MacrocosmProjectile>().Trail;
			return trail is not null;
		}

		public static void Explode(this Projectile projectile, float blastRadius, int timeLeft = 2)
		{
			projectile.tileCollide = false;
			projectile.timeLeft = timeLeft; 
			projectile.penetrate = -1;
			projectile.alpha = 255;

			projectile.velocity = Vector2.Zero;
			
			projectile.position.X += projectile.width / 2;
			projectile.position.Y += projectile.height / 2;
			projectile.width = (int)blastRadius;
			projectile.height = (int)blastRadius;
			projectile.position.X -= projectile.width / 2;
			projectile.position.Y -= projectile.height / 2;

			projectile.netUpdate = true;
		}

		/// <summary>
		/// Hostile projectiles deal 2x the <paramref name="damage"/> in Normal Mode and 4x the <paramref name="damage"/> in Expert Mode.
		/// This helper method remedies that. TODO: check how it behaves in Journey & Master Modes
		/// </summary>
		public static int TrueDamage(int damage)
			=> damage / (Main.expertMode ? 4 : 2);

		public static Rectangle GetDamageHitbox(this Projectile proj)
		{
			MethodInfo dynMethod = proj.GetType().GetMethod("Damage_GetHitbox",
				BindingFlags.NonPublic | BindingFlags.Instance);
			return (Rectangle)dynMethod.Invoke(proj, null);
		}
		
		/// <summary>
		/// Draws an animated projectile, leave texture null to draw as entity with the loaded texture
		/// (Only tested for held projectiles)  
		/// </summary>
		/// <param name="proj"> Projectile instance to draw </param>
		/// <param name="lightColor"> Computed environment color </param>
		/// <param name="drawOffset"> Offset to draw from texture center at 0 rotation </param>
		/// <param name="texture"> Leave null to draw as entity with the loaded texture </param>
		public static void DrawAnimated(this Projectile proj, Color lightColor, SpriteEffects effect, Vector2 drawOffset = default, Texture2D texture = null, Rectangle? frame = null, Effect shader = null)
		{

			if (texture is null)
 				texture = TextureAssets.Projectile[proj.type].Value;

			Vector2 position = proj.Center - Main.screenPosition;

			int numFrames = Main.projFrames[proj.type];
			Rectangle sourceRect = frame ?? texture.Frame(1, numFrames, frameY: proj.frame);

			Vector2 origin = new Vector2(texture.Width / 2, texture.Height / numFrames / 2) - new Vector2(drawOffset.X, drawOffset.Y * proj.spriteDirection);

			SpriteBatchState state = Main.spriteBatch.SaveState();
			if (shader is not null)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(shader, state);
			}

			Main.EntitySpriteDraw(texture, position, sourceRect, lightColor, proj.rotation, origin, proj.scale, effect, 0);

			if (shader is not null)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(state);
			}

		}

		/// <summary>
		/// Draws an animated projectile extra, such as a glowmask
		/// (Only tested for held projectiles)  
		/// </summary>
		public static void DrawAnimatedExtra(this Projectile proj, Texture2D glowmask, Color color, SpriteEffects effect, Vector2 drawOffset = default, Rectangle? frame = null)
			=> proj.DrawAnimated(color, effect, drawOffset + new Vector2(0, -2), glowmask, frame);
	}
}
