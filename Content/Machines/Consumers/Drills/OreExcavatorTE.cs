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

namespace Macrocosm.Content.Machines.Consumers.Drills;

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
                break;

            case nameof(Earth):
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
                var hellstone = new ConditionsChain.All(isUnderworld, Condition.DownedEowOrBoc.ToDropCondition(default));
                var notUnderworld = isUnderworld.Not();

                // Common loot
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<Coal>(), 12, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.CopperOre, 12, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.TinOre, 12, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<AluminumOre>(), 14, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.IronOre, 19, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.LeadOre, 19, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.SilverOre, 20, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.TungstenOre, 20, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<LithiumOre>(), 22, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<Silicon>(), 22, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.GoldOre, 24, notUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.PlatinumOre, 24, notUnderworld));

                // Purity loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.DirtBlock, 40, minAmt: 10, maxAmt: 100, condition: purity));
                loot.Add(new TEDropWithConditionRule(this, ItemID.StoneBlock, 40, minAmt: 10, maxAmt: 100, condition: purity));
                loot.Add(new TEDropWithConditionRule(this, ItemID.SiltBlock, 50, minAmt: 10, maxAmt: 50, condition: new ConditionsChain.All(snow.Not(), desert.Not())));

                // Corruption loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.EbonstoneBlock, 40, minAmt: 10, maxAmt: 100, condition: corruption));
                loot.Add(new TEDropWithConditionRule(this, ItemID.DemoniteOre, 56, condition: corruptionEvilOre));

                // Crimson loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.CrimstoneBlock, 40, minAmt: 10, maxAmt: 100, condition: crimson));
                loot.Add(new TEDropWithConditionRule(this, ItemID.CrimtaneOre, 56, condition: crimsonEvilOre));

                // Hallow loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.PearlstoneBlock, 40, minAmt: 10, maxAmt: 100, condition: hallow));

                // Common desert loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.DesertFossil, 50, minAmt: 2, maxAmt: 10, condition: desert));
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<OilShale>(), 100, minAmt: 1, maxAmt: 5, condition: desert));

                // Pure desert loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.SandBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, noEvil)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.HardenedSand, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, noEvil)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.Sandstone, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, noEvil)));

                // Corrupt desert loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.EbonsandBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, corruption)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptHardenedSand, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, corruption)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptSandstone, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, corruption)));

                // Crimson desert loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsandBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, crimson)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonHardenedSand, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, crimson)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonSandstone, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, crimson)));

                // Hallowed desert loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.PearlsandBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, hallow)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.HallowHardenedSand, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, hallow)));
                loot.Add(new TEDropWithConditionRule(this, ItemID.HallowSandstone, 44, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(desert, hallow)));

                // Pure Ocean biome loot
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaSand>(), 50, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, noEvil)));

                // Corrupt Ocean biome loot
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaEbonsand>(), 50, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, corruption)));

                // Crimson Ocean biome loot
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaCrimsand>(), 50, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, crimson)));

                // Hallow Ocean biome loot
                loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaPearlsand>(), 50, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(beach, hallow)));

                // Pure snow biome loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.IceBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, noEvil)));

                // Common snow biome loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.SnowBlock, 40, minAmt: 10, maxAmt: 100, condition: snow));
                loot.Add(new TEDropWithConditionRule(this, ItemID.SlushBlock, 50, minAmt: 10, maxAmt: 50, condition: snow));

                // Corrupt snow biome loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.PurpleIceBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, corruption)));

                // Crimson snow biome loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.RedIceBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, crimson)));

                // Hallowed snow biome loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.PinkIceBlock, 40, minAmt: 10, maxAmt: 100, condition: new ConditionsChain.All(snow, hallow)));

                // Glowing mushroom loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 40, minAmt: 10, maxAmt: 100, condition: glowshroom));
                loot.Add(new TEDropWithConditionRule(this, ItemID.MushroomGrassSeeds, 50, minAmt: 5, maxAmt: 15, condition: glowshroom));

                // Jungle loot
                loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 40, minAmt: 10, maxAmt: 100, condition: jungle));

                // Underworld loot 
                loot.Add(new TEDropWithConditionRule(this, ItemID.AshBlock, 40, minAmt: 10, maxAmt: 100, condition: isUnderworld));
                loot.Add(new TEDropWithConditionRule(this, ItemID.Hellstone, 20, hellstone));

                break;
            default:
                break;
        }
    }

    public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
    {
        basePosition.X -= 12;
        base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
    }
}
