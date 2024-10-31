using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class WindTurbineSmall : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.WindTurbineSmall>());
            Item.width = 20;
            Item.height = 48;
            Item.value = Item.sellPrice(silver: 10);
            Item.mech = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<SteelBar>(10)
               .AddIngredient(ItemID.Wire, 8)
               .AddIngredient<Plastic>(2)
               .AddTile<Tiles.Crafting.Fabricator>()
               .Register();
        }
    }
}