using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseCrate")]
    public class IndustrialCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ItemID.Sets.IsFishingCrate[Type] = true;
            //ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialCrate>());
            Item.width = 32;
            Item.height = 32;
            Item.value = 500;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Drop a special weapon/accessory etc. specific to this crate's theme (i.e. Sky Crate dropping Fledgling Wings or Starfury)
            int[] themedDrops = [
                ModContent.ItemType<Copernicus>(),
                ModContent.ItemType<HandheldEngine>(),
                ModContent.ItemType<ClawWrench>(),
                //ModContent.ItemType<SomeSummonWeapon>(),
            ];
            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(10, themedDrops));

            // Drop coins
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 4, 5, 13));

            IItemDropRule[] oreTypes = [
                ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 1, 30, 50),
                ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 1, 30, 50),
                ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 1, 30, 50),
                ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 1, 30, 50),
                ItemDropRule.Common(ItemID.LunarOre, 1, 30, 50),
            ];
            itemLoot.Add(new OneFromRulesRule(7, oreTypes));

            // Drop pre-hm bars (except copper/tin), with the addition of one from ExampleMod
            IItemDropRule[] oreBars = [
                ItemDropRule.Common(ModContent.ItemType<ArtemiteBar>(), 1, 10, 21),
                ItemDropRule.Common(ModContent.ItemType<SeleniteBar>(), 1, 10, 21),
                ItemDropRule.Common(ModContent.ItemType<ChandriumBar>(), 1, 10, 21),
                ItemDropRule.Common(ModContent.ItemType<DianiteBar>(), 1, 10, 21),
                ItemDropRule.Common(ItemID.LunarBar, 1, 10, 21),
            ];
            itemLoot.Add(new OneFromRulesRule(4, oreBars));

            // drugs, anyone?
            /*
            IItemDropRule[] drugs = [
            ];
            itemLoot.Add(new OneFromRulesRule(4, drugs));
            */

            // Drop (pre-hm) resource potion
            IItemDropRule[] resourcePotions = [
                ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 5, 10),
                ItemDropRule.Common(ItemID.SuperManaPotion, 1, 5, 10),
            ];
            itemLoot.Add(new OneFromRulesRule(2, resourcePotions));
        }
    }
}
