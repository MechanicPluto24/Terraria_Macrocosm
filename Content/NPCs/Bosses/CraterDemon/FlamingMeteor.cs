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
			Projectile.alpha = 255;

			Projectile.SetTrail<FlamingMeteorTrail>();
		}

        private float flashTimer;
        private float maxFlashTimer = 5;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.OnFire, 360, true);
			target.AddBuff(BuffID.Burning, 90, true);
		}

		private bool spawned;
		private Vector2 spawnPosition;
		public override void AI()
		{
			if (!spawned)
			{
				spawnPosition = Projectile.position;
				spawned = true;
            }

			Projectile.velocity.Y += 0.2f;
			if (Projectile.velocity.Y > 24f)
				Projectile.velocity.Y = 24f;

			if (Projectile.velocity != Vector2.Zero)
				Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

			Lighting.AddLight(Projectile.Center, new Color(242, 142, 35).ToVector3());

			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 127, new Color(255, 255, 255), Main.rand.NextFloat(1.4f, 2.2f));
				dust.noGravity = true;
				dust.noLight = true;
			}

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type]; // 6 frames @ 4 ticks/frame
			}

			Projectile.alpha -= 15;
            flashTimer++;
        }

        private SpriteBatchState state;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Type].Value;

			Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
			Vector2 origin = Projectile.Size / 2f + new Vector2(6, 32);

			state.SaveState(Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);


            if (flashTimer < maxFlashTimer)
            {
                Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Flare2").Value;
                float progress = flashTimer / maxFlashTimer;
                float scale = Projectile.scale * progress * 0.85f;
                Vector2 position = spawnPosition + Projectile.Size / 2f;
                float opacity = 1f;
                Main.spriteBatch.Draw(flare, position - Main.screenPosition, null, new Color(242, 142, 35).WithOpacity(opacity), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
			else
			{
                Projectile.GetTrail().Draw(Projectile.Size / 2f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, Color.White.WithOpacity(0.2f) * (1f - Projectile.alpha / 255f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
		}
	}
}
