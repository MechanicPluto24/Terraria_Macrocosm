using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.StarRoyale
{
    public class StarRoyaleTable : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteTable>(), (int)LuminiteStyle.StarRoyale);
            Item.width = 32;
            Item.height = 24;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(TileID.StarRoyaleBrick, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
