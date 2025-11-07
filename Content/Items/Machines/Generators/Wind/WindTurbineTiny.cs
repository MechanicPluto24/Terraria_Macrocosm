using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Generators.Wind;

public class WindTurbineTiny : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Generators.Wind.WindTurbineTiny>());
        Item.width = 28;  
        Item.height = 32;
        Item.value = Item.sellPrice(silver: 6);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<AluminumBar>(6)
        .AddIngredient(ItemID.Wire, 4)
        .AddTile<Tiles.Crafting.Fabricator>()
        .Register();
    }
}
