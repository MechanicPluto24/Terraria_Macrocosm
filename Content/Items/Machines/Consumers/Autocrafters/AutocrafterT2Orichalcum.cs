using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autocrafters;

public class AutocrafterT2Orichalcum : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autocrafters.AutocrafterT2>(), 1);
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.OrichalcumBar, 15)
            .AddIngredient<AdvancedCircuitBoard>(10)
            .AddIngredient<SteelBar>(5)
            .AddTile<Fabricator>()
            .Register();
    }
}
