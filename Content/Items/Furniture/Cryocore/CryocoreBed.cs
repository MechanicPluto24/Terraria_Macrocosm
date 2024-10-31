using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cryocore
{
    public class CryocoreBed : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteBed>(), (int)LuminiteStyle.Cryocore);
            Item.width = 36;
            Item.height = 18;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CryocoreBrick, 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
