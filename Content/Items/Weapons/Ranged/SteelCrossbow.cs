using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Content.Items.Bars;
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
            Item.DefaultToBow(30, 25f, hasAutoReuse: true);
            Item.width = 70;
            Item.height = 16;

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 50);

            Item.damage = 16;
            Item.knockBack = 4.5f;
        }

        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(22f, 7f),
            CenterYOffset = 6f,
            MuzzleOffset = 56f,
            Recoil = (2f, 0f),
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

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SteelBar>(16)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
