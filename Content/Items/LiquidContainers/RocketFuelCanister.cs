using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Liquids;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers
{
    public class RocketFuelCanister : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemSets.LiquidContainerData[Type] = new LiquidContainerData(LiquidType.RocketFuel, 20, ModContent.ItemType<Canister>());
            ItemSets.FuelData[Type] = new FuelData(() => (FuelPotency)((float)FuelPotency.VeryHigh), 240);
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
    }
}