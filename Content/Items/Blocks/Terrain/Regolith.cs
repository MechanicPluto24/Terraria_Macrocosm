using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Terrain;

public class Regolith : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.Regolith>());
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<RegolithWall>(4)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}