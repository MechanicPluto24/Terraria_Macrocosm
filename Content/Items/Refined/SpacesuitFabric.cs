using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Refined
{
    public class SpacesuitFabric : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;


        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Plastic>(1)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile<Tiles.Crafting.IndustrialLoom>()
                .Register();
        }
    }
}