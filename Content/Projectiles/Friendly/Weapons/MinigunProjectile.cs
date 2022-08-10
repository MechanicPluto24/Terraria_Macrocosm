using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Utility;
using Terraria.Audio;
using Macrocosm.Sounds;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons
{
	public class MinigunProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Minigun");
			Main.projFrames[Type] = 10;
			ProjectileID.Sets.NeedsUUID[Type] = true;

		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.LastPrism);
			Projectile.friendly = false; 
			Projectile.width = 5; 
			Projectile.height = 5; 
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			//if ((Projectile.rotation < 0 && Projectile.rotation <= MathHelper.PiOver4 && Projectile.rotation >= MathHelper.PiOver4 * 3) || (Projectile.rotation >= 0 &&Projectile.rotation > MathHelper.PiOver4 && Projectile.rotation <= MathHelper.PiOver4 * 3))
			//	hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.height, Projectile.width);
			//else
			//	hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
		}

		public override bool PreAI()
		{

			Player player = Main.player[Projectile.owner];

			Projectile.Center = player.Center;
			Projectile.rotation = Projectile.velocity.ToRotation();// + MathHelper.PiOver2;

			Projectile.spriteDirection = Projectile.direction;

			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;

			// If you do not multiply by projectile.direction, the player's hand will point the wrong direction while facing left.
			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

			// Get the player's current aiming direction as a normalized vector.
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
			if (aim.HasNaNs())
			{
				aim = Vector2.UnitY;
			}

			// Change a portion of the gun's current velocity so that it points to the mouse. This gives smooth movement over time.
			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 1));
			aim *= player.HeldItem.shootSpeed;

			if (aim != Projectile.velocity)
			{
				Projectile.netUpdate = true;
			}
			Projectile.velocity = aim;


			bool stillInUse = player.channel && player.HasAmmo(player.inventory[player.selectedItem]) && !player.noItems && !player.CCed;

			if (!stillInUse)
				Projectile.Kill();

			Projectile.frameCounter++;
			Projectile.ai[0]++;

			if (Projectile.ai[0] < 45)
			{
				if (Projectile.frameCounter >= 4)
				{
					Projectile.frameCounter = 0;
					Projectile.frame++;
					if (Projectile.frame >= Main.projFrames[Type] - 6)
						Projectile.frame = 0;
				}

				SoundEngine.PlaySound(CustomSounds.MinigunWindup with 
				{ 
					Volume = 0.1f,
					SoundLimitBehavior = SoundLimitBehavior.IgnoreNew 
				});

			}
			else
			{

				player.GetModPlayer<MacrocosmPlayer>().ScreenShakeIntensity = 1f;

				if (Projectile.frameCounter >= 4)
				{
					Projectile.frameCounter = 0;
					Projectile.frame += 1;
					if (Projectile.frame >= 10)
						Projectile.frame = 4;
				}

				int projToShoot = 14; //14 = bullet
				float speed = 14f; //the speed of your bullets fired
				int damage = player.GetWeaponDamage(player.inventory[player.selectedItem]); //makes the damage your weapon damage + the ammunition used.
				float knockback = Projectile.knockBack;

				if(stillInUse)
					player.PickAmmo(player.inventory[player.selectedItem], out projToShoot, out speed, out damage, out knockback, out var usedAmmoItemId); //uses ammunition from inventory

				Vector2 offset = Projectile.Center + new Vector2(40, 10 * Projectile.spriteDirection);
				Vector2 rotPoint = MathUtils.RotatingPoint(Projectile.Center, offset, Projectile.rotation);

				int rate = (int)(36 - (Projectile.ai[0] - 45f)) / 5;

				if (Projectile.ai[0] < 70f && Main.rand.NextBool(rate) || Projectile.ai[0] >= 70)
				{
					Projectile.NewProjectile(Projectile.InheritSource(Projectile), rotPoint, Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(8)) * 10f, projToShoot, damage, knockback, Projectile.owner, default, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
				}

				SoundEngine.PlaySound(CustomSounds.MinigunFire with 
				{ 
					Volume = 0.1f,
					SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
				});

			}

			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{

			Vector2 position = (Projectile.position - Main.screenPosition).Floor();
			SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			int numFrames = Main.projFrames[Type];
			Rectangle sourceRect = texture.Frame(1, numFrames, frameY: Projectile.frame);

			Main.EntitySpriteDraw(texture, position, sourceRect, lightColor, Projectile.rotation, new Vector2(texture.Width/2, texture.Height/numFrames/2) + new Vector2(-5,-10 * Projectile.spriteDirection) , 1f, effect, 0);

			return false;
		}

		public override void PostDraw(Color lightColor)
		{

			Vector2 position = (Projectile.position - Main.screenPosition).Floor();
			SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Texture2D texture = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Weapons/MinigunProjectileGlow").Value;
			int numFrames = Main.projFrames[Type];
			Rectangle sourceRect = texture.Frame(1, numFrames, frameY: Projectile.frame);

			Main.spriteBatch.Draw(texture, position, sourceRect, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / numFrames / 2) + new Vector2(-5, -10 * Projectile.spriteDirection), 1f, effect, 0f);
		}

	}
}
