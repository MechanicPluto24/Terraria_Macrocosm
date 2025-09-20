using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks;

public class ProtolithBrick : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.ProtolithBrick>());
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Protolith>(2)
            .AddTile(TileID.Furnaces)
            .Register();

        CreateRecipe()
            .AddIngredient<ProtolithBrickWall>(4)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}