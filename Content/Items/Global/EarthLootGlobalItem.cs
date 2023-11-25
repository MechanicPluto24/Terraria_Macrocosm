using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Global
{
    public class EarthLootGlobalItem : GlobalItem
    {
        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            // if result is copper, iron, silver, gold, tin, lead, tungsten or platinum
            // (no reason to use IDs since they are not even ordered by tier)
            if ((resultType >= 11 && resultType <= 14) || (resultType >= 699 && resultType < 702))
            {
                // 20% chance to override with Macrocosm ores 
                if (Main.rand.NextBool(5))
                {
                    // if overriden, spawn Aluminum or Lithium with 50% chance each
                    if (Main.rand.NextBool())
                        resultType = ModContent.ItemType<AluminumOre>();
                    else
                        resultType = ModContent.ItemType<LithiumOre>();
                }
            }

            // if extractinating desert fossils 
            if (extractType == ItemID.DesertFossil)
            {
                // 4% chance to override result  with (1-5) Oil Shales 
                if (Main.rand.NextBool(25))
                {
                    resultType = ModContent.ItemType<OilShale>();
                    resultStack = Main.rand.Next(1, 6);
                }
            }
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            int aluminumOreType = ModContent.ItemType<AluminumOre>();
            int aluminumBarType = ModContent.ItemType<AluminumBar>();
            int lithiumType = ModContent.ItemType<LithiumOre>();
            int oilShaleType = ModContent.ItemType<OilShale>();
            int siliconType = ModContent.ItemType<Silicon>();

            // chances are based off vanilla loot, denominators multiplied by the number of ore/bar types already in the pool for each crate
            // keep in mind that they are independent from vanilla loot pools i.e. you can get both gold & aluminum bars
            if (ItemID.Sets.IsFishingCrate[item.type])
            {
                if (item.type == ItemID.WoodenCrate)
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumOreType, 7 * 4, 4, 15));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumBarType, 9 * 4, 2, 5));
                }
                else if (item.type == ItemID.WoodenCrateHard)
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumOreType, 14 * 4, 4, 15));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumBarType, 19 * 4, 2, 5));
                }
                else if (item.type == ItemID.IronCrate)
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumOreType, 6 * 4, 12, 21));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumBarType, 5 * 4, 4, 7));
                }
                else if (item.type == ItemID.IronCrateHard)
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumOreType, 12 * 4, 12, 21));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumBarType, 14 * 4, 4, 7));
                }
                else if (item.type == ItemID.GoldenCrate)
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(lithiumType, 5 * 4, 25, 34));
                }
                else if (item.type == ItemID.GoldenCrateHard)
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(lithiumType, 10 * 4, 25, 34));
                }
                else if (ItemID.Sets.IsFishingCrateHardmode[item.type])
                {

                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumBarType, 12 * 4, 10, 20));

                    // this is to avoid getting both lithium & aluminum ores
                    itemLoot.Add(ItemDropRule.SequentialRulesNotScalingWithLuck(14 * 4, new IItemDropRule[]
                    {
                        ItemDropRule.NotScalingWithLuck(aluminumOreType, 1, 30, 49),
                        ItemDropRule.NotScalingWithLuck(lithiumType, 1, 30, 49)
                    }));


                    if (item.type == ItemID.OasisCrateHard)
                    {
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(oilShaleType, 2, 5, 15));
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(siliconType, 2, 10, 20));
                    }
                }
                else
                {
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(aluminumBarType, 12 * 4, 10, 20));

                    // this is to avoid getting both lithium & aluminum ores
                    itemLoot.Add(ItemDropRule.SequentialRulesNotScalingWithLuck(14 * 4, new IItemDropRule[]
                    {
                        ItemDropRule.NotScalingWithLuck(aluminumOreType, 1, 30, 49),
                        ItemDropRule.NotScalingWithLuck(lithiumType, 1, 30, 49)
                    }));

                    if (item.type == ItemID.OasisCrate)
                    {
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(oilShaleType, 2, 5, 15));
                        itemLoot.Add(ItemDropRule.NotScalingWithLuck(siliconType, 2, 10, 20));
                    }
                }
            }
        }
    }
}