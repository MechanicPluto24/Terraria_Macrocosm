using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Astra;

public class AstraDoor : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteDoorClosed>(), (int)LuminiteStyle.Astra);
        Item.width = 16;
        Item.height = 16;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AstraBrick, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
