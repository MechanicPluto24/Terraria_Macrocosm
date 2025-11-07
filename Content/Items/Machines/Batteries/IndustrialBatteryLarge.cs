using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Batteries;

public class IndustrialBatteryLarge : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Batteries.IndustrialBatteryLarge>());
        Item.width = 42;  
        Item.height = 36;  
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<Battery>(16)
        .AddIngredient(ItemID.CobaltBar, 10)
        .AddTile<Tiles.Crafting.Fabricator>()
        .Register();

        CreateRecipe()
        .AddIngredient<Battery>(16)
        .AddIngredient(ItemID.PalladiumBar, 10)
        .AddTile<Tiles.Crafting.Fabricator>()
        .Register();
    }
}
