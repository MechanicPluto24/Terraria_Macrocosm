using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech
{
    public class EngineComponentMk2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Cyan;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(20)
                .AddIngredient(ItemID.CobaltBar, 20)
                .AddIngredient(ItemID.TungstenBar, 10)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();

            CreateRecipe()
                .AddIngredient<SteelBar>(20)
                .AddIngredient(ItemID.PalladiumBar, 20)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}