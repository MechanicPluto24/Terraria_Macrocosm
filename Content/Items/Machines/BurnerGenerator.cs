using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria.ID;

namespace Macrocosm.Content.Items.Machines
{
    public class BurnerGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.BurnerGenerator>());
            Item.width = 52;
            Item.height = 30;
            Item.value =  Item.sellPrice(gold:1);
            Item.mech = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(8)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient<AluminumBar>(6)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }

    }
}