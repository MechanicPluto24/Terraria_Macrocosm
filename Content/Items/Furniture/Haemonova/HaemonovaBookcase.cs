using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova
{
    public class HaemonovaBookcase : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteBookcase>(), (int)LuminiteStyle.Haemonova);
            Item.width = 28;
            Item.height = 34;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HaemonovaBrick>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
