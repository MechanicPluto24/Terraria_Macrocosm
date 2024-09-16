using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = false;

            SetProjectileDefaults();
        }

        protected bool spawned;
        protected int itemUseTime;

        protected Player Player => Main.player[Projectile.owner];
        protected virtual bool StillInUse => Player.channel && !Player.noItems && !Player.CCed;

        public virtual float CircularHoldoutOffset { get; set; } = 1f;

        public virtual bool ShouldUpdateAimRotation => true;
        public virtual Player.CompositeArmStretchAmount? CompositeArmStretchAmount => null;

        public override bool? CanDamage() => false;
        public virtual void ProjectileAI() { }
        public virtual void ProjectileOnSpawn() { }

        public override void AI()
        {
            AI_OnSpawn();
            UpdateItemTime();
            Aim();
            PlayerVisuals();

            ProjectileAI();

            if (!StillInUse)
                Projectile.Kill();
        }

        private void AI_OnSpawn()
        {
            if (!spawned)
            {
                ProjectileOnSpawn();
                itemUseTime = Player.CurrentItem().useTime;
                spawned = true;
            }
        }

        private void UpdateItemTime()
        {
            if (itemUseTime > 0)
                itemUseTime--;
        }

        protected virtual void PlayerVisuals()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (Player.whoAmI != Main.myPlayer)
                return;

            Projectile.Center = Player.MountedCenter + new Vector2(0, Player.gfxOffY);

            Projectile.spriteDirection = Projectile.direction;

            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;
            //OwnerPlayer.itemTime = 2;
            //OwnerPlayer.itemAnimation = 3;

            float armRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            if (CompositeArmStretchAmount.HasValue)
                Player.SetCompositeArmFront(true, CompositeArmStretchAmount.Value, armRotation);
            else
                Player.itemRotation = armRotation;

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

            if (StillInUse && ShouldUpdateAimRotation)
                Projectile.velocity = aim;
        }
    }
}
