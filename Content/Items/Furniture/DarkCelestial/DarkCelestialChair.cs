using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.DarkCelestial
{
    public class DarkCelestialChair : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteChair>(), (int)LuminiteStyle.DarkCelestial );
            Item.width = 16;
            Item.height = 32;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.DarkCelestialBrick, 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}