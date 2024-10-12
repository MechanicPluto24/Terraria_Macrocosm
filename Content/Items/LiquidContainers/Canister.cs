using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Ores;
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
            ItemSets.FuelData[Type] = new FuelData(() => (FuelPotency)(float)FuelPotency.VeryHigh, 240);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = 9999;
            Item.value = 100;
            Item.rare = ItemRarityID.LightRed;
            Item.material = true;
        }

        public override void OnStack(Item source, int numToTransfer)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AluminumBar>(10)
                .AddIngredient<SteelBar>(5)
                .AddIngredient<Silicon>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}