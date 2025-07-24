using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech;

public class Computer : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tech.Computer>());
        Item.width = 32;
        Item.height = 30;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
          .AddIngredient<Plastic>(20)
          .AddIngredient<PrintedCircuitBoard>(5)
          .AddIngredient(ItemID.Glass, 2)
          .AddTile<Tiles.Crafting.Fabricator>()
          .Register();
    }
}