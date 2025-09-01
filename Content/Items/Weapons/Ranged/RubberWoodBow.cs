using Macrocosm.Content.Items.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class RubberWoodBow : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToBow(27, 6.6f, hasAutoReuse: false);
            Item.damage = 6;
            Item.knockBack = 0;
            Item.width = 16;
            Item.height = 38;
            Item.value = Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<RubberTreeWood>(10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
