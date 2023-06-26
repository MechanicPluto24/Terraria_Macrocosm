using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Macrocosm.Content.Trails;
using Terraria.DataStructures;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class DianiteTomeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = 56;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;

			Projectile.SetTrail<DianiteMeteorTrail>();
		}

		public ref float initialTargetPositionX => ref Projectile.ai[0];
		public ref float initialTargetPositionY => ref Projectile.ai[1];

		public Vector2 InitialTargetPosition => new(initialTargetPositionX, initialTargetPositionY);


		bool spawned = false;
		bool rotationClockwise = false;

		public override void AI()
		{
			if (!spawned)
			{
				rotationClockwise = Main.rand.NextBool();
				spawned = true;

				// sync ai array on spawn
				Projectile.netUpdate = true;
			}

			if (rotationClockwise)
				Projectile.rotation += 2f;
			else
				Projectile.rotation -= 2f;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			if(InitialTargetPosition.Y > Projectile.position.Y)
				return false;

			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float count = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) * 10f;

			if (count > 30f)
				count = 30f;

			var state = Main.spriteBatch.SaveState();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			for (int n = 4; n < count - 5; n++)
			{
				Vector2 trailPosition = Projectile.Center - Projectile.velocity * n * 0.2f;
				Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.OrangeRed * (0.8f - (float)n / count), Projectile.rotation + ((float)n/count), TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale * (1f - (float)n / count), SpriteEffects.None, 0f);
			}

			Projectile.GetTrail().Draw();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
		}
	}
}