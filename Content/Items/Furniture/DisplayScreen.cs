using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tech;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture;

public class DisplayScreen : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.DisplayScreen>());
        Item.width = 36;
        Item.height = 24;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Plastic>(5)
            .AddIngredient<AluminumBar>(2)
            .AddIngredient<PrintedCircuitBoard>(1)
            .AddIngredient(ItemID.Glass, 10)
            .AddTile<Fabricator>()
            .Register();
    }
}