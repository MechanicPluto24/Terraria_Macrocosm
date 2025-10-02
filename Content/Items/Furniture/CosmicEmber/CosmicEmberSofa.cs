using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.CosmicEmber;

public class CosmicEmberSofa : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteSofa>(), (int)LuminiteStyle.CosmicEmber);
        Item.width = 34;
        Item.height = 24;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CosmicEmberBrick, 5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
