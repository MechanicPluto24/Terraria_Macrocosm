using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks;

public class DianiteBrick : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.DianiteBrick>());
    }

    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<DianiteOre>(1)
            .AddIngredient<Protolith>(4)
            .AddTile(TileID.Furnaces)
            .Register();
    }
}
