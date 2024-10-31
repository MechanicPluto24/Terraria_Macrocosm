using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Crafting
{
    public class Fabricator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Crafting.Fabricator>());
            Item.width = 38;
            Item.height = 36;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddIngredient<SteelBar>(10)
                .AddIngredient<PrintedCircuitBoard>(2)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddIngredient<SteelBar>(10)
                .AddIngredient<PrintedCircuitBoard>(2)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}