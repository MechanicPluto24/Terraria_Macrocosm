using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture
{
    public class IVPole : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.Chest[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.IVPole>());
            Item.width = 14;
            Item.height = 38;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Plastic>(4)
                .AddIngredient<AluminumBar>(2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
