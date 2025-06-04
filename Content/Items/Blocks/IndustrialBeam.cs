using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks
{
    public class IndustrialBeam : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            //Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.IndustrialBeam>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<IndustrialPlating>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}