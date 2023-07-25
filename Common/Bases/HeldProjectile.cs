using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Common.Bases
{
    internal abstract class HeldProjectileItem<T> : ModItem where T: HeldProjectile
    {
        protected virtual void ResetDefaults() { }
        public sealed override void SetDefaults()
        {
            Item.damage = 99;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<T>();
            Item.knockBack = 5;
            Item.value = 10000;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = true;

            if (GetInstance<T>().KillMode == HeldProjectile.HeldProjectileKillMode.Manual)
            {
                Item.channel = true;
                Item.autoReuse = false;
            }

            ResetDefaults();
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ProjectileType<T>(), damage, knockback, player.whoAmI, type);
            return false;
        }
    }

    internal abstract class HeldProjectile : ModProjectile
    {
        public enum HeldProjectileKillMode
        {
            OnAnimationEnd,
            Manual
        }
        public abstract HeldProjectileKillMode KillMode { get; }
        public Player Player => Main.player[Projectile.owner];
        protected int Damage => Projectile.damage;
        protected short ShootProjectile => (short)Projectile.ai[0];
        protected Vector2 DirectionToMouse 
        { 
            get
            {
                return new Vector2(Projectile.ai[1], Projectile.ai[2]);
            }
            private set 
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }
        private bool shouldDie = false;
        protected Item Item { get; private set; }
        private bool shouldRunOnSpawn = true;

        protected virtual void ResetDefaults() { }
        protected virtual void OnSpawn() { }

        public sealed override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 999;

            ResetDefaults();
        }

        public sealed override void OnSpawn(IEntitySource source)
        {
            if (
                source is EntitySource_ItemUse_WithAmmo itemSource
                )
            {
                Item = itemSource.Item;
            }
            else
            {
                Projectile.active = false;
            }
        }

        public sealed override bool PreAI()
        {
            if (shouldRunOnSpawn)
            {
                OnSpawn();
                shouldRunOnSpawn = false;
            }

            if (!shouldDie)
            {
                Projectile.timeLeft = Projectile.extraUpdates + 2;
            }

            if (Player.HeldItem != Item || (Player.ItemAnimationEndingOrEnded && KillMode == HeldProjectileKillMode.OnAnimationEnd))
            {
                shouldDie = true;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                DirectionToMouse = Player.RotatedRelativePoint(Player.MountedCenter).DirectionTo(Main.MouseWorld);
            }

            Player.heldProj = Projectile.whoAmI;

            return true;
        }

        /// <summary>
        /// Kills the <see cref="HeldProjectile"/> properly.
        /// </summary>
        public void UnAlive()
        {
            shouldDie = true;
        }

        /// <summary>
        /// If this is set to <c>null</c> there will be no collision.
        /// </summary>
        public virtual (Vector2 startPosition, Vector2 endPosition, float width)? LineCollision => null;
        public sealed override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (LineCollision is null)
            {
                return false;
            }

            float _ = 0;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(), 
                LineCollision.Value.startPosition, 
                LineCollision.Value.endPosition,
                LineCollision.Value.width,
                ref _
            );
        }

        public virtual void Draw(Color lightColor) { }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            Draw(lightColor);
            return false;
        }
    }
}
