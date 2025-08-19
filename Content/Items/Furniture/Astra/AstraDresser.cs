using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Astra;

public class AstraDresser : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteDresser>(), (int)LuminiteStyle.Astra);
        Item.width = 36;
        Item.height = 22;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AstraBrick, 16)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
