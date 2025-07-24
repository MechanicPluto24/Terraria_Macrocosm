using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Special;

public class LaunchPadMarker : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Special.LaunchPadMarker>());

        Item.width = 16;
        Item.height = 16;
        Item.value = 500;
    }

    public override bool? UseItem(Player player)
    {
        return null;
    }

    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient(ItemID.Glass, 1)
            .AddIngredient(ItemID.Wire, 2)
            .AddIngredient<AluminumBar>(2)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
    }

}
