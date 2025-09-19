using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Generators.Solar;

public class SolarPanelSmall : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Generators.Solar.SolarPanelSmall>());
        Item.width = 44;
        Item.height = 34;
        Item.value = Item.sellPrice(silver: 50);
        Item.mech = true;
    }
    public override void AddRecipes()
    {
        CreateRecipe()
           .AddIngredient<SolarCell>(4)
           .AddIngredient(ItemID.Glass, 3)
           .AddIngredient(ItemID.Wire, 4)
           .AddTile<Tiles.Crafting.Fabricator>()
           .Register();
    }
}