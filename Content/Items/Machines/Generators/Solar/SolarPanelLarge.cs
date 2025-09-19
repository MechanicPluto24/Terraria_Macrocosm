using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Generators.Solar;

public class SolarPanelLarge : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Generators.Solar.SolarPanelLarge>());
        Item.width = 44;
        Item.height = 34;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }
    public override void AddRecipes()
    {
        CreateRecipe()
           .AddIngredient<SolarCell>(16)
           .AddIngredient(ItemID.Glass, 12)
           .AddIngredient(ItemID.Wire, 16)
           .AddTile<Tiles.Crafting.Fabricator>()
           .Register();
    }
}