using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class HandheldEngineProjectile : HeldGunProjectile
	{
		private int windupFrames = 1; // inactive frame
		private int shootFrames = 2;  // number of shooting animaton frames

		private int windupTime = 60;  // ticks till start of shooting 

		private int ManaUseRate = 10;
		private int ManaUseAmount = 5;

		public ref float AI_Windup => ref Projectile.ai[0];
		public ref float AI_UseCounter => ref Projectile.ai[1];

		private float overheatCounter = 0;

		public override float CircularHoldoutOffset => 45;

		public override void SetProjectileStaticDefaults()
		{
			// DisplayName.SetDefault("Experimental Handheld Engine");
			Main.projFrames[Type] = windupFrames + shootFrames;
		}

		public override void SetProjectileDefaults()
		{ 

		}

		private bool CanShoot => true;
		public override void ProjectileAI()
		{
			Main.projFrames[Type] = windupFrames + shootFrames;

			Animate();
			Shoot();
			Visuals();
			PlaySounds();

			AI_UseCounter++;
			AI_Windup++;

			if (!StillInUse)
				Projectile.Kill();
		}

		private void Animate()
		{
			Projectile.frameCounter++;

			if (Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Type])
					Projectile.frame = windupFrames;
			}
		}

		private bool hasMana => OwnerPlayer.CheckMana(ManaUseAmount);

		private void Shoot()
		{
			if (CanShoot)
			{
				Item item = OwnerPlayer.inventory[OwnerPlayer.selectedItem];
				int damage = OwnerPlayer.GetWeaponDamage(item); 
				float knockback = item.knockBack;

				if (StillInUse && AI_UseCounter % ManaUseRate == 0)
					OwnerPlayer.CheckMana(item, ManaUseAmount, true);

				Vector2 rotPoint1 = Utility.RotatingPoint(Projectile.Center, new Vector2(42, 12 * Projectile.spriteDirection), Projectile.rotation);
				Vector2 rotPoint2 = Utility.RotatingPoint(Projectile.Center, new Vector2(72, 12 * Projectile.spriteDirection), Projectile.rotation);
				Vector2 rotPoint3 = Utility.RotatingPoint(Projectile.Center, new Vector2(102, 12 * Projectile.spriteDirection), Projectile.rotation);

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), rotPoint1, Vector2.Zero, ModContent.ProjectileType<StupidShit>(), damage, knockback, OwnerPlayer.whoAmI, Projectile.whoAmI);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), rotPoint2, Vector2.Zero, ModContent.ProjectileType<StupidShit>(), damage, knockback, OwnerPlayer.whoAmI, Projectile.whoAmI);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), rotPoint3, Vector2.Zero, ModContent.ProjectileType<StupidShit>(), damage, knockback, OwnerPlayer.whoAmI, Projectile.whoAmI);

				Vector2 velocity = Utility.PolarVector(8, MathHelper.WrapAngle(Projectile.rotation)) - OwnerPlayer.velocity * 0.2f;
				Vector2 offset = Utility.PolarVector(Projectile.direction == 1 ? 4 : -20, Projectile.rotation) + new Vector2(0,16);

				Dust dust;
				for(int i = 0; i <3; i++)
				{
					dust = Dust.NewDustDirect(rotPoint1 - offset, 24, 24, ModContent.DustType<PlasmaBallDust>(), velocity.X, velocity.Y, Scale: 1f);
					dust.noGravity = true;
					dust.alpha = 255;
				}
			}
		}

		public void Visuals()
		{
			if(CanShoot)
				Lighting.AddLight(Projectile.position + Utility.PolarVector(80f, Projectile.rotation), new Vector3(0.4313f, 0.9764f, 1.0f) * 1.1f);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (!hasMana)
			{
				if (overheatCounter < 1f)
					overheatCounter += 0.002f;
			}
			else 
			{
				if (overheatCounter > 0f)
					overheatCounter -= 0.004f;

				if (overheatCounter < 0f)
					overheatCounter = 0f;
			}

			Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetPath + "OverheatGradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			effect.Parameters["uIntensity"].SetValue(overheatCounter);
			effect.Parameters["uOffset"].SetValue(new Vector2(-0.08f + (-0.2f * (1f - overheatCounter)), 0));

			Projectile.DrawAnimated(lightColor, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 12), shader: effect);
			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/HandheldEngineProjectile_Glow").Value;
			Texture2D flame = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/HandheldEngineProjectile_Flame").Value;
			Texture2D warning = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/HandheldEngineProjectile_Warning").Value;

			Projectile.DrawAnimatedGlowmask(glowmask, Color.White, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 14));

			SpriteBatchState state = Main.spriteBatch.SaveState();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			float alpha = AI_Windup >= windupTime ? 0.9f : MathHelper.SmoothStep(0.1f, 0.9f, AI_Windup / windupTime);
			Projectile.DrawAnimatedGlowmask(flame, Color.White.NewAlpha(alpha), Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 14));

			int frameY = hasMana ? 0 : (AI_UseCounter % 24 < 12 ? 1 : 2);
			Rectangle sourceRect = warning.Frame(1, 3, frameY: frameY);
			Projectile.DrawAnimatedGlowmask(warning, Color.White.NewAlpha(alpha), Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 14), sourceRect);

			Main.spriteBatch.Restore(state);
		}

		private SlotId playingSoundId_1;
		private SlotId playingSoundId_2;
		private void PlaySounds()
		{
			playingSoundId_1 = SoundEngine.PlaySound(SFX.HandheldThrusterFlame with
			{
				Volume = 0.3f,
				IsLooped = true,
				SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
			});

			if (!hasMana)
			{
				playingSoundId_2 = SoundEngine.PlaySound(SFX.HandheldThrusterOverheat with
				{
					Volume = 0.3f,
					IsLooped = true,
					SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
				});
			}
		}

		public override void Kill(int timeLeft)
		{
			if (SoundEngine.TryGetActiveSound(playingSoundId_1, out ActiveSound playingSound1))
				playingSound1.Stop();

			if (!hasMana && SoundEngine.TryGetActiveSound(playingSoundId_2, out ActiveSound playingSound2))
				playingSound2.Stop();
		}
	}

	public class StupidShit : ModProjectile
	{
		public override string Texture => Macrocosm.EmptyTexPath;

		private Projectile Owner => Main.projectile[(int)Projectile.ai[0]];

		public bool OrientationVertical
		{
			get => Projectile.ai[1] != 0f;
			set => Projectile.ai[1] = value == true ? 1f : 0f;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.timeLeft = 2;
			Projectile.tileCollide = false;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			float rotation = Math.Abs(Owner.rotation);

			OrientationVertical = (rotation < 0 && rotation <= MathHelper.PiOver4 && rotation >= MathHelper.PiOver4 * 3 || rotation >= 0 && rotation > MathHelper.PiOver4 && rotation <= MathHelper.PiOver4 * 3);

			if (OrientationVertical)
				hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.height, Projectile.width);
			else
				hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
		}

		public override void AI()
		{
			if (OrientationVertical)
				Projectile.Center += Utility.PolarVector(1000, Math.Abs(Owner.rotation));
		}
	}
	
}
