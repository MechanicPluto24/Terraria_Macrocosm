using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.StarRoyale
{
    public class StarRoyaleToilet : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteToilet>(), (int)LuminiteStyle.StarRoyale );
            Item.width = 16;
            Item.height = 24;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StarRoyaleBrick, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
