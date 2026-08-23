using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Ores;

public class QuartzFragment : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.QuartzBlock>());
        Item.value = 1000;
        Item.rare = ModContent.RarityType<MoonRarity1>();
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Items.Walls.QuartzWall>(4)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
