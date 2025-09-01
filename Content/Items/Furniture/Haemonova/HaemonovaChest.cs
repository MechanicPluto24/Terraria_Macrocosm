using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova;

public class HaemonovaChest : ModItem
{
    public override void SetStaticDefaults()
    {
        
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteChest>(), (int)LuminiteStyle.Haemonova*2);
        Item.width = 32;
        Item.height = 24;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<HaemonovaBrick>(8)
            .AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
