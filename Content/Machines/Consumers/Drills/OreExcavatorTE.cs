using Macrocosm.Common.Loot;
using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Loot.DropRules;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Items.Blocks.Sands;
using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Drills
{
    public class OreExcavatorTE : BaseDrillTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<OreExcavator>();
        protected override float ExcavateRate => 60;
        public override int InventorySize => 50;

        public override void MachineUpdate()
        {
            MinPower = 1f;
            MaxPower = 25f;
            base.MachineUpdate();
        }

        protected override void PopulateItemLoot(LootTable loot)
        {
            switch (MacrocosmSubworld.SanitizeID(MacrocosmSubworld.CurrentID))
            {
                case nameof(Moon):

                    loot.Add(new TECommonDrop(this, ModContent.ItemType<Regolith>(), 20, minAmt: 10, maxAmt: 100));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<Cynthalith>(), 20, minAmt: 10, maxAmt: 100));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<Protolith>(), 20, minAmt: 15, maxAmt: 150));

                    loot.Add(new TECommonDrop(this, ModContent.ItemType<QuartzFragment>(), 25));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<SeleniteOre>(), 10));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<ArtemiteOre>(), 10));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<ChandriumOre>(), 10));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<DianiteOre>(), 10));
                    loot.Add(new TECommonDrop(this, ItemID.LunarOre, 8));
                    loot.Add(new TECommonDrop(this, ModContent.ItemType<NickelOre>(), 16));

                    break;

                default:

                    var purity = new SceneDataConditions.IsPurity(scene);
                    var corruption = new SceneDataConditions.IsCorruption(scene);
                    var crimson = new SceneDataConditions.IsCrimson(scene);
                    var corruptionEvilOre = new ConditionsChain.All(corruption, Condition.DownedEaterOfWorlds.ToDropCondition(default));
                    var crimsonEvilOre = new ConditionsChain.All(crimson, Condition.DownedBrainOfCthulhu.ToDropCondition(default));
                    var hallow = new SceneDataConditions.IsHallow(scene);
                    var noEvil = new SceneDataConditions.IsSpreadableBiome(scene).Not();
                    var desert = new SceneDataConditions.IsDesert(scene);
                    var snow = new SceneDataConditions.IsSnow(scene);
                    var beach = new TilePositionConditions.IsBeach(Position.ToPoint());
                    var glowshroom = new SceneDataConditions.IsGlowingMushroom(scene);
                    var jungle = new SceneDataConditions.IsJungle(scene);
                    var isUnderworld = new SceneDataConditions.IsUnderworld(scene);
                    var notUnderworld = isUnderworld.Not();
                    var hellstone = new ConditionsChain.All(isUnderworld, Condition.DownedEowOrBoc.ToDropCondition(default));
                    var hardmode = new Conditions.IsHardmode();

                    // Common loot
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<Coal>(), 6, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CopperOre, 6, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.TinOre, 6, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<AluminumOre>(), 7, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.IronOre, 8, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.LeadOre, 8, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.SilverOre, 10, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.TungstenOre, 10, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<LithiumOre>(), 11, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<Silicon>(), 11, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.GoldOre, 12, notUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PlatinumOre, 12, notUnderworld));

                    // Common Hardmode loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CobaltOre, 12, hardmode));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PalladiumOre, 12, hardmode));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.MythrilOre, 16, hardmode));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.OrichalcumOre, 16, hardmode));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.AdamantiteOre, 20, hardmode));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.TitaniumOre, 20, hardmode));

                    // Purity loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.DirtBlock, 20, minAmt: 10, maxAmt: 100, condition: purity));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.StoneBlock, 20, minAmt: 10, maxAmt: 100, condition: purity));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.SiltBlock, 25, minAmt: 10, maxAmt: 50, condition: new ConditionsChain.All(snow.Not(), desert.Not())));

                    // Corruption loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.EbonstoneBlock, 20, minAmt: 10, maxAmt: 100, condition: corruption));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.DemoniteOre, 14, condition: corruptionEvilOre));

                    // Crimson loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CrimstoneBlock, 20, minAmt: 10, maxAmt: 100, condition: crimson));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CrimtaneOre, 14, condition: crimsonEvilOre));

                    // Hallow loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PearlstoneBlock, 20, minAmt: 10, maxAmt: 100, condition: hallow));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CrystalShard, 25, minAmt: 2, maxAmt: 5, condition: hallow));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.QueenSlimeCrystal, 50, condition: hallow));

                    // Common desert loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.DesertFossil, 25, minAmt: 2, maxAmt: 10, condition: desert));
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<OilShale>(), 50, minAmt: 1, maxAmt: 5, condition: desert));

                    // Pure desert loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.SandBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, noEvil)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.HardenedSand, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, noEvil)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.Sandstone, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, noEvil)));

                    // Corrupt desert loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.EbonsandBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, corruption)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptHardenedSand, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, corruption)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptSandstone, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, corruption)));

                    // Crimson desert loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsandBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, crimson)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonHardenedSand, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, crimson)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonSandstone, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, crimson)));

                    // Hallowed desert loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PearlsandBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, hallow)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.HallowHardenedSand, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, hallow)));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.HallowSandstone, 22, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, hallow)));

                    // Pure Ocean biome loot
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaSand>(), 25, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, noEvil)));

                    // Corrupt Ocean biome loot
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaEbonsand>(), 25, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, corruption)));

                    // Crimson Ocean biome loot
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaCrimsand>(), 25, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, crimson)));

                    // Hallow Ocean biome loot
                    loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaPearlsand>(), 25, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, hallow)));

                    // Pure snow biome loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.IceBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, noEvil)));

                    // Common snow biome loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.SnowBlock, 20, minAmt: 10, maxAmt: 100, condition: snow));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.SlushBlock, 25, minAmt: 10, maxAmt: 50, condition: snow));

                    // Corrupt snow biome loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PurpleIceBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, corruption)));

                    // Crimson snow biome loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.RedIceBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, crimson)));

                    // Hallowed snow biome loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.PinkIceBlock, 20, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, hallow)));

                    // Glowing mushroom loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 20, minAmt: 10, maxAmt: 100, condition: glowshroom));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.MushroomGrassSeeds, 25, minAmt: 5, maxAmt: 15, condition: glowshroom));

                    // Jungle loot
                    loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 20, minAmt: 10, maxAmt: 100, condition: jungle));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.ChlorophyteOre, 12, condition: new ConditionsChain.All(jungle, new Conditions.DownedPlantera())));

                    // Underworld loot 
                    loot.Add(new TEDropWithConditionRule(this, ItemID.AshBlock, 20, minAmt: 10, maxAmt: 100, condition: isUnderworld));
                    loot.Add(new TEDropWithConditionRule(this, ItemID.Hellstone, 10, hellstone));

                    break;
            }
        }

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            basePosition.X -= 12;
            base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
        }
    }
}
