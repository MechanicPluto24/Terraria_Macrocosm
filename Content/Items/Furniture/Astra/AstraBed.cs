using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Astra;

public class AstraBed : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteBed>(), (int)LuminiteStyle.Astra);
        Item.width = 36;
        Item.height = 18;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AstraBrick, 15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
