using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.DarkCelestial
{
    public class DarkCelestialPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminitePlatform>(), (int)LuminiteStyle.DarkCelestial);
            Item.width = 24;
            Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.DarkCelestialBrick)
                .Register();
        }
    }
}