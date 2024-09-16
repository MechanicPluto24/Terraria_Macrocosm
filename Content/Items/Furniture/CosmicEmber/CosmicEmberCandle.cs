using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.CosmicEmber
{
    public class CosmicEmberCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteCandle>(), (int)LuminiteStyle.CosmicEmber );
            Item.width = 16;
            Item.height = 16;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CosmicEmberBrick, 4)
                .AddIngredient(ItemID.Torch, 1) // Luminite Crystal
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
