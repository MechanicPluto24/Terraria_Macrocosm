using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autosmelters;

public class AutosmelterT2Titanium : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autosmelters.AutosmelterT2>(), 1);
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.TitaniumBar, 15)
            .AddIngredient<AdvancedCircuitBoard>(8)
            .AddIngredient<SteelBar>(5)
            .AddTile<Fabricator>()
            .Register();
    }
}

