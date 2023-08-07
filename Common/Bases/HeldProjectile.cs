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
        public virtual void SetDefaultsHeldProjectile() { }
        public virtual bool CanUseItemHeldProjectile(Player player) => true;

        public virtual float? ProjectileScale => null;

        /// <summary>
        /// Use SetDefaultsHeldProjectile instead.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public sealed override void SetDefaults()
        {
            Item.damage = 99;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<T>();
            Item.knockBack = 5;
            Item.value = 10000;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            if (GetInstance<T>().KillMode == HeldProjectile.HeldProjectileKillMode.Manual)
            {
                Item.channel = true;
            }

            SetDefaultsHeldProjectile();
		}

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<T>(), damage, knockback, player.whoAmI, type);
            projectile.scale = ProjectileScale ?? Item.scale;
            return false;
        }

        /// <summary>
        /// Use CanUseItemHeldProjectile instead.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public sealed override bool CanUseItem(Player player) => Main.projectile.FirstOrDefault(
            projectile => projectile.ModProjectile is T && projectile.owner == player.whoAmI && projectile.active
        ) is null && CanUseItemHeldProjectile(player);
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
        /// <summary>
        /// The projectile type the item would have shot based on ammo.
        /// </summary>
        protected short ShootProjectileType => (short)Projectile.ai[0];

        private bool shouldDie = false;
        protected EntitySource_ItemUse_WithAmmo Source { get; private set; }
        protected Item Item => Source.Item;
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
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;

            ResetDefaults();
        }

        public sealed override void OnSpawn(IEntitySource source)
        {
            if (
                source is EntitySource_ItemUse_WithAmmo itemSource
                )
            {
                Source = itemSource;
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
                Projectile.timeLeft = Projectile.extraUpdates;
            }

            if (Player.HeldItem != Item || (Player.ItemAnimationEndingOrEnded && KillMode == HeldProjectileKillMode.OnAnimationEnd))
            {
                shouldDie = true;
            }

            // This might fix the issue with changing the aim while the projectile is alive, but creates a stutter for some reason
            //Player.ChangeDir(MathF.Abs(Projectile.rotation) <= MathHelper.PiOver2 ? 1 : -1);

			Player.heldProj = Projectile.whoAmI;

            return true;
        }

        public sealed override bool ShouldUpdatePosition() => false;

        /// <summary>
        /// Kills the <see cref="HeldProjectile"/> properly.
        /// </summary>
        public void UnAlive()
        {
            shouldDie = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public virtual void Draw(Color lightColor) { }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            Draw(lightColor);
            return false;
        }
    }
}
