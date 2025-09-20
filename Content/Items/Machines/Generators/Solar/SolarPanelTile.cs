using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Generators.Solar;

public class SolarPanelTile : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Generators.Solar.SolarPanelTile>());
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(silver: 8);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe(6)
           .AddIngredient<SolarCell>(4)
           .AddIngredient(ItemID.Glass, 3)
           .AddIngredient(ItemID.Wire, 4)
           .AddTile<Tiles.Crafting.Fabricator>()
           .Register();
    }
}