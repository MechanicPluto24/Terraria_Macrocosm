using Macrocosm.Content.Items.Materials;
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
           .AddIngredient<SolarCell>(6)
           .AddIngredient(ItemID.Glass, 2)
           .AddIngredient(ItemID.Wire, 6)
           .AddTile<Tiles.Crafting.Fabricator>()
           .Register();
    }
}
