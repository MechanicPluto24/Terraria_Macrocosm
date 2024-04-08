using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Blocks
{
    public class RegolithBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.RegolithBrick>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Regolith>(2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}