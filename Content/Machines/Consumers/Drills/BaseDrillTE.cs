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

namespace Macrocosm.Content.Machines.Consumers.Drills;

public abstract class BaseDrillTE : ConsumerTE
{
    public List<int> BlacklistedItems { get; set; } = new();
    public LootTable LootTable { get; set; }

    protected abstract float ExcavateRate { get; }

    protected abstract void PopulateItemLoot(LootTable lootTable);

    protected float excavateTimer;
    protected int sceneCheckTimer;
    protected SceneData scene;

    public override void MachineUpdate()
    {
        // Capture current scene
        scene ??= new(Position);

        // Generate loot table based on current subworld and biome
        if (LootTable is null)
        {
            LootTable = new();
            PopulateItemLoot(LootTable);
        }

        if (PoweredOn)
        {
            excavateTimer += 1f * PowerProgress;
            if (excavateTimer >= ExcavateRate)
            {
                excavateTimer -= ExcavateRate;
                if(LootTable is not null)
                {
                    foreach (var entry in LootTable.Where((rule) => rule is IBlacklistable).Cast<IBlacklistable>())
                        entry.Blacklisted = BlacklistedItems.Contains(entry.ItemID);

                    LootTable.Drop(Utility.GetClosestPlayer(Position, MachineTile.Width * 16, MachineTile.Height * 16));
                }
            }

            sceneCheckTimer++;
            if (sceneCheckTimer >= 5 * 60 * 60)
            {
                sceneCheckTimer = 0;
                scene?.Scan();
            }
        }
    }
   
    public override void MachineNetSend(BinaryWriter writer)
    {
        writer.Write(BlacklistedItems.Count);
        foreach (int itemId in BlacklistedItems)
            writer.Write(itemId);
    }

    public override void MachineNetReceive(BinaryReader reader)
    {
        int blacklistedCount = reader.ReadInt32();
        BlacklistedItems = new(blacklistedCount);
        for (int i = 0; i < blacklistedCount; i++)
            BlacklistedItems.Add(reader.ReadInt32());
    }

    public override void MachineSaveData(TagCompound tag)
    {
        tag[nameof(BlacklistedItems)] = BlacklistedItems;
    }

    public override void MachineLoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(BlacklistedItems)))
            BlacklistedItems = tag.GetList<int>(nameof(BlacklistedItems)) as List<int>;
    }
}
