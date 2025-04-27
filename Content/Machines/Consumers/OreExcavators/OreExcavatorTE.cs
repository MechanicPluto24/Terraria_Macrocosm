using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Loot;
using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Loot.DropRules;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Blocks.Sands;
using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines.Consumers.OreExcavators
{
    public class OreExcavatorTE : ConsumerTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<OreExcavator>();

        public LootTable Loot { get; set; }
        public List<int> BlacklistedItems { get; set; } = new();
        public override int InventorySize => 50;

        private float excavateTimer;
        private float ExcavateRate => 60;


        private int sceneCheckTimer;
        private SceneData scene;

        public override void OnFirstUpdate()
        {
            // Generate loot table based on current subworld and biome
            Loot = new();
            PopulateItemLoot();
        }

        public override void MachineUpdate()
        {
            MinPower = 1f;
            MaxPower = 25f;

            if (PoweredOn)
            {
                excavateTimer += 1f * PowerProgress;
                if (excavateTimer >= ExcavateRate)
                {
                    excavateTimer -= ExcavateRate;
                    ApplyBlacklist();
                    Loot?.Drop(Utility.GetClosestPlayer(Position, MachineTile.Width * 16, MachineTile.Height * 16));
                }

                sceneCheckTimer++;
                if (sceneCheckTimer >= 5 * 60 * 60)
                {
                    sceneCheckTimer = 0;
                    scene?.Scan();
                }
            }
        }

        private void ApplyBlacklist()
        {
            foreach (var entry in Loot.Where((rule) => rule is IBlacklistable).Cast<IBlacklistable>())
                entry.Blacklisted = BlacklistedItems.Contains(entry.ItemID);
        }

        protected virtual void PopulateItemLoot()
        {
            scene = new(Position);

            switch (MacrocosmSubworld.SanitizeID(MacrocosmSubworld.CurrentID))
            {
                case nameof(Moon):

                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<Regolith>(), 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<Cynthalith>(), 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<Protolith>(), 20, amountDroppedMinimum: 15, amountDroppedMaximum: 150));

                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<QuartzFragment>(), 25));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<SeleniteOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<ArtemiteOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<ChandriumOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<DianiteOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ItemID.LunarOre, 8));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<NickelOre>(), 16));

                    break;

                default:

                    var purity = new SceneDataConditions.IsPurity(scene);
                    var corruption = new SceneDataConditions.IsCorruption(scene);
                    var hallow = new SceneDataConditions.IsHallow(scene);
                    var noEvil = new SceneDataConditions.IsSpreadableBiome(scene).Not();
                    var desert = new SceneDataConditions.IsDesert(scene);
                    var snow = new SceneDataConditions.IsSnow(scene);
                    var beach = new TilePositionConditions.IsBeach(Position.ToPoint());
                    var glowshroom = new SceneDataConditions.IsGlowingMushroom(scene);
                    var jungle = new SceneDataConditions.IsJungle(scene); 
                    var isUnderworld = new SceneDataConditions.IsUnderworld(scene);
                    var notUnderworld = isUnderworld.Not();

                    // Common loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<Coal>(), 6, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CopperOre, 6, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.TinOre, 6, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<AluminumOre>(), 7, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.IronOre, 8, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.LeadOre, 8, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SilverOre, 10, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.TungstenOre, 10, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<LithiumOre>(), 11, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<Silicon>(), 11, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.GoldOre, 12, notUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PlatinumOre, 12, notUnderworld));

                    // Common Hardmode loot
                    var hardmode = new Conditions.IsHardmode();
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CobaltOre, 12, hardmode));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PalladiumOre, 12, hardmode));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MythrilOre, 16, hardmode));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.OrichalcumOre, 16, hardmode));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.AdamantiteOre, 20, hardmode));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.TitaniumOre, 20, hardmode));

                    // Purity loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.DirtBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: purity));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.StoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: purity));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SiltBlock, 25, amountDroppedMinimum: 10, amountDroppedMaximum: 50, condition: new ConditionsChain.All(snow.Not(), desert.Not())));

                    // Corruption loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.EbonstoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: corruption));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.DemoniteOre, 14, condition: corruption));

                    // Crimson loot
                    var crimson = new SceneDataConditions.IsCrimson(scene);
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimstoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: crimson));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimtaneOre, 14, condition: crimson));

                    // Hallow loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PearlstoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: hallow));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrystalShard, 25, amountDroppedMinimum: 2, amountDroppedMaximum: 5, condition: hallow));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.QueenSlimeCrystal, 50, condition: hallow));

                    // Common desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.DesertFossil, 25, amountDroppedMinimum: 2, amountDroppedMaximum: 10, condition: desert));
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<OilShale>(), 50, amountDroppedMinimum: 1, amountDroppedMaximum: 5, condition: desert));

                    // Pure desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, noEvil)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.HardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, noEvil)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.Sandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, noEvil)));

                    // Corrupt desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.EbonsandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, corruption)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptHardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, corruption)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptSandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, corruption)));

                    // Crimson desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, crimson)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonHardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, crimson)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonSandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, crimson)));

                    // Hallowed desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PearlsandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, hallow)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.HallowHardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, hallow)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.HallowSandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(desert, hallow)));

                    // Pure Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaSand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(beach, noEvil)));

                    // Corrupt Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaEbonsand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(beach, corruption)));

                    // Crimson Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaCrimsand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(beach, crimson)));

                    // Hallow Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaPearlsand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(beach, hallow)));

                    // Pure snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.IceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(snow, noEvil)));

                    // Common snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SnowBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: snow));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SlushBlock, 25, amountDroppedMinimum: 10, amountDroppedMaximum: 50, condition: snow));

                    // Corrupt snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PurpleIceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(snow, corruption)));

                    // Crimson snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.RedIceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(snow, crimson)));

                    // Hallowed snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PinkIceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(snow, hallow)));

                    // Glowing mushroom loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: glowshroom));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MushroomGrassSeeds, 25, amountDroppedMinimum: 5, amountDroppedMaximum: 15, condition: glowshroom));

                    // Jungle loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: jungle));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.ChlorophyteOre, 12, condition: new ConditionsChain.All(jungle, new Conditions.DownedPlantera())));

                    // Underworld loot 
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.AshBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: isUnderworld));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.Hellstone, 10, isUnderworld));

                    break;
            }
        }

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            basePosition.X -= 12;
            base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);

            writer.Write(BlacklistedItems.Count);
            foreach (int itemId in BlacklistedItems)
                writer.Write(itemId);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);

            int blacklistedCount = reader.ReadInt32();
            BlacklistedItems = new(blacklistedCount);
            for (int i = 0; i < blacklistedCount; i++)
                BlacklistedItems.Add(reader.ReadInt32());
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag[nameof(BlacklistedItems)] = BlacklistedItems;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey(nameof(BlacklistedItems)))
                BlacklistedItems = tag.GetList<int>(nameof(BlacklistedItems)) as List<int>;
        }
    }
}
