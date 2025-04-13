using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Regolith
{
    public class RegolithPiano : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Regolith.RegolithPiano>());
            Item.width = 38;
            Item.height = 24;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RegolithBrick>(15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ItemID.Book, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
