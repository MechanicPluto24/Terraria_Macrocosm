using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles.Crafting;

namespace Macrocosm.Content.Items.Machines.Consumers.Autocrafters;

public class AutocrafterT3 : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autocrafters.AutocrafterT3>());
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.LunarBar, 20)
            .AddIngredient<AdvancedCircuitBoard>(15)
            .AddIngredient<Plastic>(12)
            .AddIngredient<SteelBar>(10)
            .AddTile<Fabricator>()
            .Register();
    }
}
