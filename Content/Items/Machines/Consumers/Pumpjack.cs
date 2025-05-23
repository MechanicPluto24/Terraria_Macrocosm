using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.LiquidContainers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers
{
    public class Pumpjack : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Pumpjack>());
            Item.width = 44;
            Item.height = 40;
            Item.value = Item.sellPrice(gold: 1);
            Item.mech = true;
        }

        // TBD
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.CopperBar, 12)
                .AddIngredient<AluminumBar>(6)
                .AddIngredient<Canister>(4)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();

            CreateRecipe()
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.TinBar, 12)
                .AddIngredient<AluminumBar>(6)
                .AddIngredient<Canister>(4)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}