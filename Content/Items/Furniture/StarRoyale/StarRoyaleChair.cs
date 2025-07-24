using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.StarRoyale;

public class StarRoyaleChair : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteChair>(), (int)LuminiteStyle.StarRoyale);
        Item.width = 16;
        Item.height = 32;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient(ItemID.StarRoyaleBrick, 5)
        .AddTile(TileID.MythrilAnvil)
        .Register();
    }
}