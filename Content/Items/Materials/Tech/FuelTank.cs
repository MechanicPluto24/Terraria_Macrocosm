using Macrocosm.Content.Items.Materials.Bars;
using Macrocosm.Content.Items.Materials.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Tech
{
    public class FuelTank : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.LightRed;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AluminumBar>(10)
                .AddIngredient<SteelBar>(5)
                .AddIngredient<Silicon>(8)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}