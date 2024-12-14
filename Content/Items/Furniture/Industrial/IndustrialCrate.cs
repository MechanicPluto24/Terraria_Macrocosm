using Macrocosm.Content.Items.Accessories;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Consumables.Throwable;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Tools;
using Macrocosm.Content.Items.Torches;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
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
            // 1/20 chance to get an Moon Base Outpost drop
            int[] themedDrops = [
                ModContent.ItemType<ClawWrench>(),
                ModContent.ItemType<StopSign>(),
                ModContent.ItemType<WaveGunRed>(),
                ModContent.ItemType<Copernicus>(),
                ModContent.ItemType<HummingbirdDroneRemote>(),
                ModContent.ItemType<OsmiumBoots>(),
                ModContent.ItemType<StalwartTowerShield>(),
                ModContent.ItemType<Sledgehammer>(),
                ModContent.ItemType<LaserSight>()
            ];
            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(20, themedDrops));

            // Drop coins

            IItemDropRule[] oreTypes = [
                ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 1, 10, 15),
                ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 1, 10, 15),
                ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 1, 10, 15),
                ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 1, 10, 15),
                ItemDropRule.Common(ItemID.LunarOre, 1, 30, 50),
            ];
            itemLoot.Add(new OneFromRulesRule(7, oreTypes));

            IItemDropRule[] oreBars = [
                ItemDropRule.Common(ModContent.ItemType<SeleniteBar>(), 1, 3, 6),
                ItemDropRule.Common(ModContent.ItemType<ArtemiteBar>(), 1, 3, 6),
                ItemDropRule.Common(ModContent.ItemType<ChandriumBar>(), 1, 3, 6),
                ItemDropRule.Common(ModContent.ItemType<DianiteBar>(), 1, 3, 6),
                ItemDropRule.Common(ItemID.LunarBar, 1, 10, 21),
            ];
            itemLoot.Add(new OneFromRulesRule(4, oreBars));

            // drugs, anyone?
            /*
            IItemDropRule[] drugs = [
            ];
            itemLoot.Add(new OneFromRulesRule(4, drugs));
            */

            IItemDropRule[] common = [
                ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 5, 10),
                ItemDropRule.Common(ItemID.SuperManaPotion, 1, 5, 10),
                ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 1, 5, 10),
                ItemDropRule.Common(ModContent.ItemType<LunarCrystal>(), 1, 5, 10),
                ItemDropRule.Common(ModContent.ItemType<LuminiteTorch>(), 1, 5, 10),
            ];
            itemLoot.Add(new OneFromRulesRule(1, common));
        }
    }
}
