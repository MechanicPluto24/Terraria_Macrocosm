using Macrocosm.Common.Bases.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class SteelCrossbow : GunHeldProjectileItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 16;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
        }

        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(22f, 7f),
            CenterYOffset = 6f,
            MuzzleOffset = 56f,
            Recoil = (2f, 0.1f),
            RecoilDiminish = 0.9f
        };

        public override bool? UseItem(Player player)
        {
            return true;
        }

        public override void UpdateInventory(Player player)
        {
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }
    }
}
