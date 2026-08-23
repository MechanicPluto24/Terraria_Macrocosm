using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Oil;

public class Pumpjack : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Oil.Pumpjack>());
        Item.width = 44;
        Item.height = 40;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    // TBD
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<SteelBar>(6)
            .AddIngredient<PrintedCircuitBoard>(5)
            .AddIngredient<Motor>(2)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
    }
}
