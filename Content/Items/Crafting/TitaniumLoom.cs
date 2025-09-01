using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Crafting
{
    public class TitaniumLoom : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Crafting.IndustrialLoom>(), tileStyleToPlace: 0);
            Item.width = 50;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddIngredient<SteelBar>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}