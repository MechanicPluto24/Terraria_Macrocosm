using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
	// TODO: Extend from HeldProjectile
	public abstract class ChargedHeldProjectile : ModProjectile
	{
		public virtual void SetProjectileStaticDefaults() { }

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.NeedsUUID[Type] = true;
			SetProjectileStaticDefaults();
		}

		public virtual void SetProjectileDefaults() { }
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.LastPrism);
			Projectile.friendly = true;
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.tileCollide = false;

			SetProjectileDefaults();
		}

		public override bool? CanDamage() => false;

		protected Player Player => Main.player[Projectile.owner];
		protected virtual bool StillInUse => Player.channel && !Player.noItems && !Player.CCed;

		public virtual float CircularHoldoutOffset { get; set; } = 1f;
		public virtual void ProjectileAI() { }

		public override void AI()
		{
			Aim();
			PlayerVisuals();

			ProjectileAI();

			if (!StillInUse)
				Projectile.Kill();
		}

		protected virtual void PlayerVisuals()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Player.whoAmI != Main.myPlayer)
				return;

			Projectile.Center = Player.MountedCenter + new Vector2(0, Player.gfxOffY);

			Projectile.spriteDirection = Projectile.direction;

			Player.ChangeDir(Projectile.direction);
			Player.heldProj = Projectile.whoAmI;
			//OwnerPlayer.itemTime = 2;
			//OwnerPlayer.itemAnimation = 3;

			Player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

			Projectile.netUpdate = true;
		}

		protected virtual void Aim()
		{
			if (!StillInUse || Player.whoAmI != Main.myPlayer)
				return;

			// Get the player's current aiming direction as a normalized vector.
			Vector2 aim = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitY);

			// Change a portion of the gun's current velocity so that it points to the mouse. This gives smooth movement over time.
			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 1));
			aim *= CircularHoldoutOffset;

			if ((aim != Projectile.velocity || Projectile.velocity != Projectile.oldVelocity) && Main.netMode != NetmodeID.MultiplayerClient)
				Projectile.netUpdate = true;

			if (StillInUse)
				Projectile.velocity = aim;
		}
	}
}
