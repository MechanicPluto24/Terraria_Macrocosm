using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture
{
    public class OilBarrel : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OilBarrel>());
            Item.width = 22;
            Item.height = 26;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
