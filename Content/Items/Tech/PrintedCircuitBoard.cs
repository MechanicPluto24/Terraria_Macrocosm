using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech
{
    public class PrintedCircuitBoard : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tech.PrintedCircuitBoard>());
            Item.width = 20;
            Item.height = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
              .AddIngredient<Silicon>(6)
              .AddIngredient<Plastic>(1)
              .AddIngredient(ItemID.Wire, 10)
              .AddIngredient(ItemID.SilverBar, 1)
              .AddTile<Tiles.Crafting.Fabricator>()
              .Register();
        }
    }
}