using Macrocosm.Common.Utils;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public abstract class HeldGunProjectile : ModProjectile
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
			Projectile.friendly = false;
			Projectile.width = 1;
			Projectile.height = 1;

			SetProjectileDefaults();
		}


		protected Player OwnerPlayer => Main.player[Projectile.owner];
		protected bool StillInUse => OwnerPlayer.channel && OwnerPlayer.HasAmmo(OwnerPlayer.inventory[OwnerPlayer.selectedItem]) && !OwnerPlayer.noItems && !OwnerPlayer.CCed;


		public virtual float CircularHoldoutOffset { get; set; } = 1f;
		public virtual void ProjectileAI() { }

		public override void AI()
		{
			PlayerVisuals();
			Aim();

			ProjectileAI();
		}

		protected virtual void PlayerVisuals()
		{
			Projectile.Center = OwnerPlayer.Center;
			Projectile.rotation = Projectile.velocity.ToRotation();

			Projectile.spriteDirection = Projectile.direction;

			OwnerPlayer.ChangeDir(Projectile.direction);
			OwnerPlayer.heldProj = Projectile.whoAmI;
			OwnerPlayer.itemTime = 2;
			OwnerPlayer.itemAnimation = 2;

			OwnerPlayer.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

			Projectile.netUpdate = true;
		}

		protected virtual void Aim()
		{
			if(OwnerPlayer.whoAmI != Main.myPlayer)
				return;

			// Get the player's current aiming direction as a normalized vector.
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - Projectile.Center);

			if (aim.HasNaNs())
				aim = Vector2.UnitY;

			// Change a portion of the gun's current velocity so that it points to the mouse. This gives smooth movement over time.
			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 1));
			aim *= CircularHoldoutOffset;

			if (aim != Projectile.velocity && Main.netMode != NetmodeID.MultiplayerClient)
				Projectile.netUpdate = true;

			Projectile.velocity = aim;

			if(Projectile.velocity != Projectile.oldVelocity)
				Projectile.netUpdate = true;
		}
	}
}
