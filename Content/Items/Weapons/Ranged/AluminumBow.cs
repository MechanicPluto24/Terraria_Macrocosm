using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class AluminumBow : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToBow(28, 6.6f, hasAutoReuse: false);
            Item.damage = 8;
            Item.width = 16;
            Item.height = 32;
            Item.value = Item.sellPrice(silver: 2, copper: 60);
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<AluminumBar>(7)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
