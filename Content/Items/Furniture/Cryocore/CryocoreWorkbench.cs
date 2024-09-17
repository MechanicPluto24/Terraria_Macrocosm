using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Macrocosm.Common.Enums;

namespace Macrocosm.Content.Items.Furniture.Cryocore
{
    public class CryocoreWorkbench : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteWorkbench>(),(int)LuminiteStyle.Cryocore);
            Item.width = 28;
            Item.height = 16;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CryocoreBrick, 10)
                .Register();
        }
    }
}
