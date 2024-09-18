using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.CosmicEmber
{
    public class CosmicEmberWorkbench : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteWorkbench>(), (int)LuminiteStyle.CosmicEmber);
            Item.width = 28;
            Item.height = 16;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CosmicEmberBrick, 10)
                .Register();
        }
    }
}
