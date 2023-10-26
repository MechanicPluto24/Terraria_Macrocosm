using Macrocosm.Common.Bases;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class HandheldEngineProjectile : ChargedGunHeldProjectile
	{
		public ref float AI_Overheat => ref Projectile.ai[0];
		public ref float AI_UseCounter => ref Projectile.ai[1];
		public float AI_Windup = 0;

		private const int windupFrames = 1; // inactive frame
		private const int shootFrames = 2;  // number of shooting animaton frames

		private readonly int windupTime = 60;  // ticks till start of shooting 

		private readonly int ManaUseRate = 10;
		private readonly int ManaUseAmount = 5;
		

		public override float CircularHoldoutOffset => 45;

		public override void SetProjectileStaticDefaults()
		{
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
			ComputeOverheat();
			Visuals();

			if(!Main.dedServ && StillInUse)
				PlaySounds();

			AI_UseCounter++;
			AI_Windup++;
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

		private bool OwnerHasMana => OwnerPlayer.CheckMana(ManaUseAmount);

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

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), rotPoint1, Vector2.Zero, ModContent.ProjectileType<HandheldEngineHitbox>(), damage, knockback, OwnerPlayer.whoAmI, Projectile.whoAmI);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), rotPoint2, Vector2.Zero, ModContent.ProjectileType<HandheldEngineHitbox>(), damage, knockback, OwnerPlayer.whoAmI, Projectile.whoAmI);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), rotPoint3, Vector2.Zero, ModContent.ProjectileType<HandheldEngineHitbox>(), damage, knockback, OwnerPlayer.whoAmI, Projectile.whoAmI);

				Vector2 rotPoint4 = Utility.RotatingPoint(Projectile.Center, new Vector2(130, 12 * Projectile.spriteDirection), Projectile.rotation);
				Vector2 rotPoint5 = Utility.RotatingPoint(Projectile.Center, new Vector2(145, 12 * Projectile.spriteDirection), Projectile.rotation);

				Lighting.AddLight(rotPoint1, Color.Lerp(Color.Lerp(new Color(98, 204, 255, 0), new Color(255, 177, 65, 0), AI_Overheat), Color.Lerp(new Color(255, 177, 65), new Color(255, 24, 24), AI_Overheat), 0.25f).ToVector3() * (1.5f - (0.75f * Utility.PositiveSineWave(10))));
				Lighting.AddLight(rotPoint3, Color.Lerp(Color.Lerp(new Color(98, 204, 255, 0), new Color(255, 177, 65, 0), AI_Overheat), Color.Lerp(new Color(255, 177, 65), new Color(255, 24, 24), AI_Overheat), 0.50f).ToVector3() * (1.2f - (0.6f * Utility.PositiveSineWave(10, 1))));
				Lighting.AddLight(rotPoint4, Color.Lerp(Color.Lerp(new Color(98, 204, 255, 0), new Color(255, 177, 65, 0), AI_Overheat), Color.Lerp(new Color(255, 177, 65), new Color(255, 24, 24), AI_Overheat), 0.60f).ToVector3() * (0.8f - (0.4f * Utility.PositiveSineWave(10, 2))));
				Lighting.AddLight(rotPoint5, Color.Lerp(Color.Lerp(new Color(98, 204, 255, 0), new Color(255, 177, 65, 0), AI_Overheat), Color.Lerp(new Color(255, 177, 65), new Color(255, 24, 24), AI_Overheat), 0.75f).ToVector3() * (0.5f - (0.25f * Utility.PositiveSineWave(10, 3))));

				for (int i = 0; i < (int)(10f * MathHelper.Clamp((AI_Windup / windupTime), 0,1)); i++)
				{
					float amp = Main.rand.NextFloat(0, 1f);
					Vector2 position = rotPoint1 + Main.rand.NextVector2Circular(12, 20);
					Vector2 velocity = (Utility.PolarVector(36 * (1 - amp), MathHelper.WrapAngle(Projectile.rotation)) - Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotatedByRandom(MathHelper.PiOver4 * 0.15f) + OwnerPlayer.velocity;

					Particle.CreateParticle<EngineSpark>(p =>
					{
						p.Position = position;
						p.Velocity = velocity;
						p.Scale = Main.rand.NextFloat(1.2f, 1.8f) * (1 - amp);
						p.Rotation = Projectile.rotation;
						p.ColorOnSpawn = Color.White;
						p.ColorOnDespawn = Color.Lerp(new Color(89, 151, 193), new Color(255, 177, 65), AI_Overheat);
					});
				}
			}
		}

		public void Visuals()
		{
			//if(CanShoot)
			//	Lighting.AddLight(Projectile.position + Utility.PolarVector(80f, Projectile.rotation), new Vector3(0.4313f, 0.9764f, 1.0f) * 1.1f);
		}

		public void ComputeOverheat()
		{
			if (!OwnerHasMana)
			{
				if (AI_Overheat < 1f)
					AI_Overheat += 0.002f;
			}
			else
			{
				if (AI_Overheat > 0f)
					AI_Overheat -= 0.004f;

				if (AI_Overheat < 0f)
					AI_Overheat = 0f;
			}
		}

		public override bool? CanHitNPC(NPC target)
		{
			return base.CanHitNPC(target);
		}

		public override bool CanHitPlayer(Player target)
		{
			return base.CanHitPlayer(target);
		}



		private SpriteBatchState state1, state2;

		public override bool PreDraw(ref Color lightColor)
		{
			state1.SaveState(Main.spriteBatch);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state1);

			float windupFactor = 0.1f + 0.9f * Utility.BounceEaseIn(AI_Windup >= windupTime ? 1f : MathHelper.SmoothStep(0.1f, 1f, AI_Windup / windupTime));
			float windupFactor2 = Utility.QuadraticEaseOut(MathHelper.Clamp(-0.4f + 0.6f * (AI_Windup / windupTime), 0, 1));

			VertexStrip strip = new();
			int stripDataCount = 36;
			Vector2[] positions = new Vector2[stripDataCount];
			float[] rotations = new float[stripDataCount];
			Array.Fill(positions, Projectile.Center + (new Vector2(0, 12).RotatedBy(Projectile.rotation) * Projectile.direction) - Main.screenPosition);
			Array.Fill(rotations, Projectile.rotation + MathHelper.Pi);

			for (int i = 0; i < stripDataCount; i++)
 				positions[i] += Utility.PolarVector(4f * i, Projectile.rotation);
 
			var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
							.UseProjectionMatrix(doUse: true)
							.UseSaturation(-2.4f)
							.UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
							.UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail1"))
							.UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"));

			shader.Apply();

			strip.PrepareStrip(positions, rotations, 
				(float progress) => Color.Lerp(
								Color.Lerp(new Color(98, 204, 255, 0), new Color(255, 177, 65, 0), AI_Overheat) * windupFactor,
								Color.Lerp(new Color(255, 177, 65), new Color(255, 24, 24), AI_Overheat) * windupFactor2,
								Utility.CubicEaseIn(progress) * 0.85f + 0.15f * Utility.PositiveSineWave(40)) * (0.05f + 0.95f * windupFactor), 
				(float progress) => MathHelper.Lerp(25, 55 + 30 * Utility.PositiveSineWave(70), progress));
			strip.DrawTrail();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state1);

			Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "ColorGradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Rectangle sourceRect = TextureAssets.Projectile[Type].Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

			effect.Parameters["uSourceRect"].SetValue(new Vector4((float)sourceRect.X, (float)sourceRect.Y, (float)sourceRect.Width, (float)sourceRect.Height));
			effect.Parameters["uImageSize0"].SetValue(TextureAssets.Projectile[Type].Size());

			effect.Parameters["uColorIntensity"].SetValue(new Vector4(AI_Overheat * 0.8f, 0f, 0f, 1f));
			effect.Parameters["uOffset"].SetValue(new Vector2(0.35f,0.5f));
			effect.Parameters["uSize"].SetValue(0.5f * AI_Overheat);
			effect.Parameters["uSDF"].SetValue(1); // square-shaped distance function

			Projectile.DrawAnimated(lightColor, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 12), shader: effect);

			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/HandheldEngineProjectile_Glow").Value;
			Texture2D flame = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/HandheldEngineProjectile_Flame").Value;
			Texture2D warning = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/HandheldEngineProjectile_Warning").Value;
			SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

			int frameY = OwnerHasMana ? 0 : (AI_UseCounter % 24 < 12 ? 1 : 2);
			Rectangle sourceRect = warning.Frame(1, 3, frameY: frameY);

			float alpha = 0.1f + 0.1f * Utility.QuadraticEaseIn(AI_Windup >= windupTime ? 0.9f : MathHelper.SmoothStep(0.1f, 0.9f, AI_Windup / windupTime));

			state2.SaveState(Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.AlphaBlend, state2);

			Projectile.DrawAnimatedExtra(glowmask, Color.Lerp(Color.White, new Color(245, 80, 20), AI_Overheat).WithOpacity(0.9f - 0.9f * AI_Overheat), effects, new Vector2(5, 14));
			Projectile.DrawAnimatedExtra(flame, (Color.Lerp(Color.White, new Color(245, 120, 40), AI_Overheat) * (alpha + 0.9f *  AI_Overheat)).WithOpacity(0f), effects, new Vector2(5, 14));


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state2);

			Projectile.DrawAnimatedExtra(warning, Color.White, effects, new Vector2(5, 14), frame: sourceRect);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state2);
		}

		private SlotId playingSoundId_1 = default;
		private SlotId playingSoundId_2 = default;
		private void PlaySounds()
		{
			if (!StillInUse)
				return;

			if (!playingSoundId_1.IsValid || playingSoundId_1 == default)
			{
				playingSoundId_1 = SoundEngine.PlaySound(SFX.HandheldThrusterFlame with
				{
					Volume = 0.3f,
					IsLooped = true,
					SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
				},
				Projectile.position);
			}
			
			if (!OwnerHasMana && (!playingSoundId_2.IsValid || playingSoundId_2 == default))
			{
				playingSoundId_2 = SoundEngine.PlaySound(SFX.HandheldThrusterOverheat with
				{
					Volume = 0.3f,
					IsLooped = true,
					SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
				},
				Projectile.position);
			}
		}

		public override void OnKill(int timeLeft)
		{
			if (SoundEngine.TryGetActiveSound(playingSoundId_1, out ActiveSound playingSound1))
				playingSound1.Stop();

			if (SoundEngine.TryGetActiveSound(playingSoundId_2, out ActiveSound playingSound2))
				playingSound2.Stop();
		}
	}

	// TODO: replace this with CanHit 
	public class HandheldEngineHitbox : ModProjectile
	{
		public override string Texture => Macrocosm.EmptyTexPath;

		private Projectile Owner => Main.projectile[(int)Projectile.ai[0]];

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 1;
			Projectile.friendly = true;
			Projectile.timeLeft = 2;
			Projectile.tileCollide = false;
		}

		/*
		public bool OrientationVertical
		{
			get => Projectile.ai[1] != 0f;
			set => Projectile.ai[1] = value == true ? 1f : 0f;
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
		*/
	}
	
}
