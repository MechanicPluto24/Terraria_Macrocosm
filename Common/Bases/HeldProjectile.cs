using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Common.Bases
{
    public abstract class HeldProjectileItem<T> : ModItem where T : HeldProjectile
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
            Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<T>(), damage, knockback, player.whoAmI);
            projectile.scale = ProjectileScale ?? Item.scale;

            if (projectile.ModProjectile is HeldProjectile heldProjectile)
            {
                heldProjectile.ShootProjectileType = type;
            }

            // This needs to be called once more, in order to properly sync the ExtraAI,
            // without using the ai[] array, including the item used which can only be synced there.
            if (Main.netMode != NetmodeID.SinglePlayer && player.whoAmI == Main.myPlayer)
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);

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

    public abstract class HeldProjectile : ModProjectile
    {
        public enum HeldProjectileKillMode
        {
            OnAnimationEnd,
            Manual
        }

        public abstract HeldProjectileKillMode KillMode { get; }
        public Player Player => Main.player[Projectile.owner];
        protected int Damage => Projectile.damage;

        /// <summary> The projectile type the item would have shot based on ammo. </summary>
        public int ShootProjectileType { get; set; }

        protected Item item = new();

        private bool shouldDie;

        /// <summary> The item used to spawn this. </summary>
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
                item = itemSource.Item;
                Projectile.netUpdate = true;
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

            if (Projectile.owner == Main.myPlayer &&
                ((Player.HeldItem.type != item.type && Player.HeldItem.type != Main.mouseItem.type) ||
                (Player.ItemAnimationEndingOrEnded && KillMode == HeldProjectileKillMode.OnAnimationEnd)) ||
                (Player.itemTime <= 1 && KillMode == HeldProjectileKillMode.OnAnimationEnd))
            {
                shouldDie = true;
            }

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

        public virtual void NetSend(BinaryWriter writer) { }

        public virtual void NetReceive(BinaryReader reader) { }

        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            // TODO: this could be a job for the NetSyncAttribute
            writer.Write((short)ShootProjectileType);
            writer.Write(Projectile.scale);
            ItemIO.Send(item, writer);

            NetSend(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader reader)
        {
            ShootProjectileType = reader.ReadInt16();
            Projectile.scale = reader.ReadSingle();
            ItemIO.Receive(item, reader);

            NetReceive(reader);
        }
    }
}
