using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cryocore;

public class CryocorePlatform : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 200;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminitePlatform>(), (int)LuminiteStyle.Cryocore);
        Item.width = 24;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient(ItemID.CryocoreBrick)
            .Register();
    }
}