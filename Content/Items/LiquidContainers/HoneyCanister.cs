using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Liquids;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers;

public class HoneyCanister : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
        ItemSets.LiquidContainerData[Type] = new LiquidContainerData(LiquidID.Honey, 20, ModContent.ItemType<Canister>());
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 44;
        Item.maxStack = 9999;
        Item.value = 100;
        Item.rare = ItemRarityID.LightRed;
    }
}