using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Tiles.Furniture.MoonBase;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.MoonBase
{
    public class MoonBaseDoor : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MoonBaseDoorClosed>());
            Item.width = 12;
            Item.height = 32;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<MoonBasePlating>(5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}