using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks
{
    public class LexanGlass : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.LexanGlass>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient<Plastic>(2)
                .AddIngredient(ItemID.Glass, 1)
                .AddTile(TileID.GlassKiln)
                .Register();
        }
    }
}