using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cryocore;

public class CryocoreBookcase : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteBookcase>(), (int)LuminiteStyle.Cryocore);
        Item.width = 28;
        Item.height = 34;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CryocoreBrick, 20)
            .AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
