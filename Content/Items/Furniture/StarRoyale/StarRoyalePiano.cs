using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.StarRoyale
{
    public class StarRoyalePiano : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminitePiano>(), (int)LuminiteStyle.StarRoyale);
            Item.width = 30;
            Item.height = 26;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(TileID.StarRoyaleBrick, 15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ItemID.Book, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
