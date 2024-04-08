using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Blocks
{
    public class IrradiatedBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.IrradiatedBrick>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IrradiatedRock>(2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}