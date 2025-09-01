using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Luminite
{
    public class LuminiteToilet : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteToilet>(), (int)LuminiteStyle.Luminite);
            Item.width = 16;
            Item.height = 24;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBrick, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
