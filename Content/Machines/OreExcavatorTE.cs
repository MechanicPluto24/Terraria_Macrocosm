using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Loot;
using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Loot.DropRules;
using Macrocosm.Common.Storage;
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

namespace Macrocosm.Content.Machines
{
    public class OreExcavatorTE : ConsumerTE, IInventoryOwner
    {
        public override MachineTile MachineTile => ModContent.GetInstance<OreExcavator>();

        public SimpleLootTable Loot { get; set; }
        protected virtual int OreGenerationRate => 60;

        public List<int> BlacklistedItems { get; set; } = new();


        public Inventory Inventory { get; set; }
        protected virtual int InventorySize => 50;
        public Vector2 InventoryItemDropLocation => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

        protected int checkTimer;

        protected int sceneCheckTimer;
        protected SceneData scene;

        public override void OnFirstUpdate()
        {
            // Create new inventory if none found on world load
            Inventory ??= new(InventorySize, this);

            // Assign inventory owner if the inventory was found on load
            // IInvetoryOwner does not work well with TileEntities >:(
            if (Inventory.Owner is null)
                Inventory.Owner = this;

            // Generate loot table based on current subworld and biome
            Loot = new();
            PopulateItemLoot();
        }

        public override void MachineUpdate()
        {
            RequiredPower = 25f;

            if (PoweredOn)
            {
                checkTimer++;
                sceneCheckTimer++;
            }

            if (checkTimer >= OreGenerationRate)
            {
                checkTimer = 0;
                ApplyBlacklist();
                Loot?.Drop(Utility.GetClosestPlayer(Position, MachineTile.Width * 16, MachineTile.Height * 16));
            }

            if (sceneCheckTimer >= 5 * 60 * 60)
            {
                sceneCheckTimer = 0;
                scene?.Scan();
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


                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<SeleniteOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<ArtemiteOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<ChandriumOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<DianiteOre>(), 10));
                    Loot.Add(new TECommonDrop(this, ItemID.LunarOre, 8));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<NickelOre>(), 16));

                    break;

                default:

                    // Common loot
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<Coal>(), 6));
                    Loot.Add(new TECommonDrop(this, ItemID.CopperOre, 6));
                    Loot.Add(new TECommonDrop(this, ItemID.TinOre, 6));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<AluminumOre>(), 7));
                    Loot.Add(new TECommonDrop(this, ItemID.IronOre, 8));
                    Loot.Add(new TECommonDrop(this, ItemID.LeadOre, 8));
                    Loot.Add(new TECommonDrop(this, ItemID.SilverOre, 10));
                    Loot.Add(new TECommonDrop(this, ItemID.TungstenOre, 10));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<LithiumOre>(), 11));
                    Loot.Add(new TECommonDrop(this, ModContent.ItemType<Silicon>(), 11));
                    Loot.Add(new TECommonDrop(this, ItemID.GoldOre, 12));
                    Loot.Add(new TECommonDrop(this, ItemID.PlatinumOre, 12));

                    // Common Hardmode loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CobaltOre, 12, condition: new Conditions.IsHardmode()));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PalladiumOre, 12, condition: new Conditions.IsHardmode()));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MythrilOre, 16, condition: new Conditions.IsHardmode()));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.OrichalcumOre, 16, condition: new Conditions.IsHardmode()));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.AdamantiteOre, 20, condition: new Conditions.IsHardmode()));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.TitaniumOre, 20, condition: new Conditions.IsHardmode()));

                    // Purity loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.DirtBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsPurity(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.StoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsPurity(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SiltBlock, 25, amountDroppedMinimum: 10, amountDroppedMaximum: 50, condition: new ConditionsChain.All(new SceneDataConditions.IsSnow(scene).Not(), new SceneDataConditions.IsDesert(scene).Not())));

                    // Corruption loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.EbonstoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsCorruption(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.DemoniteOre, 14, condition: new SceneDataConditions.IsCorruption(scene)));

                    // Crimson loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimstoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsCrimson(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimtaneOre, 14, condition: new SceneDataConditions.IsCrimson(scene)));

                    // Hallow loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PearlstoneBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsHallow(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrystalShard, 25, amountDroppedMinimum: 2, amountDroppedMaximum: 5, condition: new SceneDataConditions.IsHallow(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.QueenSlimeCrystal, 50, condition: new SceneDataConditions.IsHallow(scene)));

                    // Common desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.DesertFossil, 25, amountDroppedMinimum: 2, amountDroppedMaximum: 10, condition: new SceneDataConditions.IsDesert(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<OilShale>(), 50, amountDroppedMinimum: 1, amountDroppedMaximum: 5, condition: new SceneDataConditions.IsDesert(scene)));

                    // Pure desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsSpreadableBiome(scene).Not())));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.HardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsSpreadableBiome(scene).Not())));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.Sandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsSpreadableBiome(scene).Not())));

                    // Corrupt desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.EbonsandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsCorruption(scene))));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptHardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsCorruption(scene))));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CorruptSandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsCorruption(scene))));

                    // Crimson desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsCrimson(scene))));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonHardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsCrimson(scene))));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.CrimsonSandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsCrimson(scene))));

                    // Hallowed desert loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PearlsandBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsHallow(scene))));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.HallowHardenedSand, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsHallow(scene))));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.HallowSandstone, 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsDesert(scene), new SceneDataConditions.IsHallow(scene))));

                    // Pure Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaSand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new TilePositionConditions.IsBeach(Position.ToPoint()), new SceneDataConditions.IsSpreadableBiome(scene).Not())));

                    // Corrupt Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaEbonsand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new TilePositionConditions.IsBeach(Position.ToPoint()), new SceneDataConditions.IsCorruption(scene))));

                    // Crimson Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaCrimsand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new TilePositionConditions.IsBeach(Position.ToPoint()), new SceneDataConditions.IsCrimson(scene))));

                    // Hallow Ocean biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ModContent.ItemType<SilicaPearlsand>(), 22, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new TilePositionConditions.IsBeach(Position.ToPoint()), new SceneDataConditions.IsHallow(scene))));

                    // Pure snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.IceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsSnow(scene), new SceneDataConditions.IsSpreadableBiome(scene).Not())));

                    // Common snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SnowBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsSnow(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.SlushBlock, 25, amountDroppedMinimum: 10, amountDroppedMaximum: 50, condition: new SceneDataConditions.IsSnow(scene)));

                    // Corrupt snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PurpleIceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsSnow(scene), new SceneDataConditions.IsCorruption(scene))));

                    // Crimson snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.RedIceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsSnow(scene), new SceneDataConditions.IsCrimson(scene))));

                    // Hallowed snow biome loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.PinkIceBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new ConditionsChain.All(new SceneDataConditions.IsSnow(scene), new SceneDataConditions.IsHallow(scene))));

                    // Glowing mushroom loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsGlowingMushroom(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MushroomGrassSeeds, 25, amountDroppedMinimum: 5, amountDroppedMaximum: 15, condition: new SceneDataConditions.IsGlowingMushroom(scene)));

                    // Jungle loot
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.MudBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsJungle(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.ChlorophyteOre, 12, condition: new ConditionsChain.All(new SceneDataConditions.IsJungle(scene), new Conditions.DownedPlantera())));

                    // Underworld loot 
                    // TODO: maybe exclude common drops from underworld and keep only these?
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.AshBlock, 20, amountDroppedMinimum: 10, amountDroppedMaximum: 100, condition: new SceneDataConditions.IsUnderworld(scene)));
                    Loot.Add(new TEDropWithConditionRule(this, ItemID.Hellstone, 10, condition: new SceneDataConditions.IsUnderworld(scene)));

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

            Inventory ??= new(InventorySize, this);
            TagIO.Write(Inventory.SerializeData(), writer);

            writer.Write(BlacklistedItems.Count);
            foreach (int itemId in BlacklistedItems)
                writer.Write(itemId);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);

            TagCompound tag = TagIO.Read(reader);
            Inventory = Inventory.DeserializeData(tag);

            int blacklistedCount = reader.ReadInt32();
            BlacklistedItems = new(blacklistedCount);
            for (int i = 0; i < blacklistedCount; i++)
                BlacklistedItems.Add(reader.ReadInt32());
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            tag[nameof(Inventory)] = Inventory;
            tag[nameof(BlacklistedItems)] = BlacklistedItems;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            if (tag.ContainsKey(nameof(Inventory)))
                Inventory = tag.Get<Inventory>(nameof(Inventory));

            if (tag.ContainsKey(nameof(BlacklistedItems)))
                BlacklistedItems = tag.GetList<int>(nameof(BlacklistedItems)) as List<int>;
        }
    }
}
