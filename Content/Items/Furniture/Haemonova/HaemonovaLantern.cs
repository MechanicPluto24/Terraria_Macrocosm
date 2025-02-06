using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova
{
    public class HaemonovaLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteLantern>(), (int)LuminiteStyle.Haemonova);
            Item.width = 14;
            Item.height = 28;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HaemonovaBrick>( 6)
                .AddIngredient<LunarCrystal>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
