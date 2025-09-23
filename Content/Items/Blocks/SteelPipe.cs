using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks;

public class SteelPipe : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.SteelPipe>());
    }

    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<Bars.SteelBar>(1)
            .AddTile(TileID.Anvils)
            .Register();
    }
}