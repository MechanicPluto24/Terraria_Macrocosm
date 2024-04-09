using Macrocosm.Content.Items.Materials.Refined;
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
            CreateRecipe(15)
                .AddIngredient<Plastic>(15)
                .AddIngredient(ItemID.Glass, 15)
                .AddTile(TileID.GlassKiln)
                .Register();
        }
    }
}