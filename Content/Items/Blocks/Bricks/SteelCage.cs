using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks
{
    public class SteelCage : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.SteelCage>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
            .AddIngredient<SteelBar>()
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}