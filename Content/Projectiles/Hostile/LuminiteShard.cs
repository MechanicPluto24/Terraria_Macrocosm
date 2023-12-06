using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
	public class LuminiteShard : ModProjectile
	{
		public override string Texture => Macrocosm.EmptyTexPath;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public ref float AI_Timer => ref Projectile.ai[0];

		public int TargetPlayer
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public bool Fall
		{
			get => Projectile.ai[2] > 0f;
			set => Projectile.ai[2] = value ? 1f : 0f;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;

			Projectile.tileCollide = false;
		}

		private SpriteBatchState state;
		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.DrawMagicPixelTrail(Vector2.Zero, 5f, 1f, LuminiteSlime.EffectColor, LuminiteSlime.EffectColor.WithOpacity(0f));

			state.SaveState(Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			Main.spriteBatch.DrawStar(Projectile.Center - Main.screenPosition, 1, LuminiteSlime.EffectColor, 0.6f, Projectile.rotation + MathHelper.PiOver2, entity: true);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			bool hasTarget = TargetPlayer >= 0 && TargetPlayer < 255;
			float timeToShoot = 40;
			float baseShootSpeed = 12f;
			float shootDeviation = 0.5f;

			if (Main.netMode != NetmodeID.MultiplayerClient && hasTarget)
			{
				AI_Timer++;

				Player player = Main.player[TargetPlayer];
				if (AI_Timer == timeToShoot && player.active && !player.dead && !player.longInvince)
				{
					float aimAngle = (player.Center - Projectile.Center).ToRotation();
					float shootSpeed = baseShootSpeed + Main.rand.NextFloat(-shootDeviation, shootDeviation);
					Projectile.velocity = Utility.PolarVector(shootSpeed, aimAngle);
					Projectile.netUpdate = true;
				}
				else if (!Fall && AI_Timer > timeToShoot * Main.rand.NextFloat(1.5f, 3f))
				{
					Fall = true;
					Projectile.netUpdate = true;
				}
			}

			if (Fall)
			{
				Projectile.tileCollide = !WorldGen.SolidTile(Projectile.Center.ToTileCoordinates());
				Projectile.velocity.Y += hasTarget ? 0.1f : 0.3f;
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
			}
		}
	}
}
