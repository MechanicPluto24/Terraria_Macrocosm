using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases
{
    // TODO: Extend from HeldProjectile
    public abstract class ChargedGunHeldProjectile : ModProjectile
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

            SetProjectileDefaults();
        }

        protected Player OwnerPlayer => Main.player[Projectile.owner];
        protected bool StillInUse => OwnerPlayer.channel && !OwnerPlayer.noItems && !OwnerPlayer.CCed;

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

            if (OwnerPlayer.whoAmI != Main.myPlayer)
                return;

            Projectile.Center = OwnerPlayer.MountedCenter + new Vector2(0, OwnerPlayer.gfxOffY);

            Projectile.spriteDirection = Projectile.direction;

            OwnerPlayer.ChangeDir(Projectile.direction);
            OwnerPlayer.heldProj = Projectile.whoAmI;
            //OwnerPlayer.itemTime = 2;
            //OwnerPlayer.itemAnimation = 3;

            OwnerPlayer.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            Projectile.netUpdate = true;
        }

        protected virtual void Aim()
        {
            if (!StillInUse || OwnerPlayer.whoAmI != Main.myPlayer)
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
