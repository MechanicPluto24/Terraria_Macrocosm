using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class FlamingMeteor : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 35;
			ProjectileID.Sets.TrailingMode[Type] = 3;

			Main.projFrames[Type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;

			Projectile.SetTrail<FlamingMeteorTrail>();
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.OnFire, 360, true);
			target.AddBuff(BuffID.Burning, 90, true);
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.2f;
			if (Projectile.velocity.Y > 24f)
				Projectile.velocity.Y = 24f;

			if (Projectile.velocity != Vector2.Zero)
				Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f, 127, new Color(255, 255, 255), Main.rand.NextFloat(1.2f, 1.8f));
				dust.noGravity = true;
				dust.noLight = true;
			}

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type]; // 6 frames @ 4 ticks/frame
			}
		}

		private SpriteBatchState state;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Type].Value;

			Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
			Vector2 origin = Projectile.Size / 2f + new Vector2(6, 32);

			state.SaveState(Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

			Projectile.GetTrail().Draw(Projectile.Size / 2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition,
				sourceRect, Color.White.WithOpacity(0.1f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
		}
	}
}
