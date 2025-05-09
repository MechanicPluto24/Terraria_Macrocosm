using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers
{
    public class Canister : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemSets.LiquidContainerData[Type] = LiquidContainerData.CreateEmpty(20);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = 9999;
            Item.value = 100;
            Item.rare = ItemRarityID.LightRed;

        }

        public override void OnStack(Item source, int numToTransfer)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient(ItemID.Glass, 2)
                .AddIngredient<SteelBar>(2)
                .AddIngredient<Plastic>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}