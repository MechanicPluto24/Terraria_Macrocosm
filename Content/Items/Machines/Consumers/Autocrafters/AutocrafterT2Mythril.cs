using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Tiles.Crafting;
using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autocrafters;

public class AutocrafterT2Mythril : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autocrafters.AutocrafterT2>(), 0);
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MythrilBar, 15)
            .AddIngredient<AdvancedCircuitBoard>(10)
            .AddIngredient<SteelBar>(5)
            .AddTile<Fabricator>()
            .Register();
    }
}
