using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Content.Items.Bars;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class SteelRifle : GunHeldProjectileItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 16;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item11;
        }

        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(24f, 0f),
            CenterYOffset = 9f,
            MuzzleOffset = 45f,
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
            position -= new Vector2(4 * player.direction, 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SteelBar>(16)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
