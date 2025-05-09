using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova
{
    public class HaemonovaClock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteClock>(), (int)LuminiteStyle.Haemonova);
            Item.width = 20;
            Item.height = 40;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HaemonovaBrick>(20)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
