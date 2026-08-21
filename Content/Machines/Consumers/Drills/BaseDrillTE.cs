using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.ItemCreationContexts;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Consumers.Drills;

public abstract class BaseDrillTE : ConsumerTE
{
    public List<int> BlacklistedItems { get; set; } = new();

    
    // Tile sampling
    

    /// <summary>
    /// Flat row-major snapshot of tiles directly below the machine.
    /// 0 = air, -1 = solid but not drillable, &gt;0 = item ID the tile drops.
    /// </summary>
    public int[] SampledItems { get; private set; } = Array.Empty<int>();

    /// <summary> Width of the sample area. Defaults to the machine footprint width. </summary>
    public virtual int SampleGridWidth => MachineTile.Width;

    /// <summary> Depth (rows) of the sample area below the machine footprint. </summary>
    public virtual int SampleGridHeight => 5;

    /// <summary> Ticks between automatic re-samples (~3 real-time minutes). </summary>
    protected virtual int SampleRate => 3 * 60 * 60;

    // Start at max so the first update triggers an immediate sample.
    private int sampleTimer = int.MaxValue;

    /// <summary>
    /// Immediately (re)samples the tiles below the machine and, in multiplayer, syncs the result.
    /// Safe to call from the UI thread; the sample is snapped from the current tile state.
    /// </summary>
    public void RequestResample()
    {
        SampleTilesUnderMachine();
        if (Main.netMode != NetmodeID.SinglePlayer)
            NetSync();
    }

    protected abstract float ExcavateRate { get; }

    protected float excavateTimer;
    protected int sceneCheckTimer;
    protected SceneData scene;

    public override float PowerDemand => IsEnabledByPlayer && HasAvailableLoot() ? MaxPower : 0f;

    public override void OnFirstUpdate()
    {
        SampleTilesUnderMachine();
        sampleTimer = 0;
    }

    public override void MachineUpdate()
    {
        UpdateActiveSounds();

        scene ??= new(Position);

        if (IsRunning)
        {
            excavateTimer += 1f * RatedPowerProgress;
            if (excavateTimer >= ExcavateRate)
            {
                excavateTimer -= ExcavateRate;
                DropFromSample();
            }

            // Periodic tile re-sample
            if (++sampleTimer >= SampleRate)
            {
                sampleTimer = 0;
                SampleTilesUnderMachine();
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetSync();
            }

            sceneCheckTimer++;
            if (sceneCheckTimer >= 5 * 60 * 60)
            {
                sceneCheckTimer = 0;
                scene?.Scan();
            }
        }
    }

    
    // Tile sampling
    

    protected void SampleTilesUnderMachine()
    {
        Point16 origin = TileObjectData.TopLeft(Position.X, Position.Y);
        int w = SampleGridWidth;
        int h = SampleGridHeight;
        int startX = origin.X + (MachineTile.Width - w) / 2;
        int startY = origin.Y + MachineTile.Height; // first row directly below footprint

        SampledItems = new int[w * h];
        for (int row = 0; row < h; row++)
        {
            for (int col = 0; col < w; col++)
            {
                int wx = startX + col;
                int wy = startY + row;
                if (!WorldGen.InWorld(wx, wy))
                    continue;

                Tile tile = Main.tile[wx, wy];
                if (!tile.HasTile)
                {
                    SampledItems[row * w + col] = 0; // air
                }
                else
                {
                    DrillDropData data = TileSets.DrillDrop[tile.TileType];
                    bool drillable = data.ItemDrop >= 0 && (data.Condition?.IsMet() ?? true);
                    SampledItems[row * w + col] = drillable ? data.ItemDrop : -1;
                }
            }
        }
    }

    
    // Proportional loot from sample
    

    private void DropFromSample()
    {
        if (SampledItems.Length == 0)
            return;

        // Count each drillable item type, skipping blacklisted entries
        var counts = new Dictionary<int, int>();
        foreach (int id in SampledItems)
            if (id > 0 && !BlacklistedItems.Contains(id))
                counts[id] = counts.GetValueOrDefault(id) + 1;

        if (counts.Count == 0)
            return;

        int total = SampledItems.Length;
        int roll = Main.rand.Next(total);
        int selectedItemId = 0;
        foreach (var (itemId, count) in counts)
        {
            if (roll < count)
            {
                selectedItemId = itemId;
                break;
            }

            roll -= count;
        }

        if (selectedItemId <= 0)
            return;

        Vector2 dropPosition = Position.ToWorldCoordinates();
        TileObjectData tileData = TileObjectData.GetTileData(Main.tile[Position]);
        if (tileData is not null)
            dropPosition = new(dropPosition.X + tileData.Width * 16 / 2f, dropPosition.Y + (tileData.Height + 3) * 16);

        Item item = new(selectedItemId);
        bool placed = InventorySize > 0 && Inventory.TryPlacingItem(ref item, InventoryPlacementSource.Internal, sound: false);

        if (placed)
        {
            Item clone = new(selectedItemId);
            Particle.Create<ItemTransferParticle>((p) =>
            {
                p.StartPosition = dropPosition + Main.rand.NextVector2Circular(32, 16);
                p.EndPosition   = dropPosition + new Vector2(0, -96) + Main.rand.NextVector2Circular(16, 16);
                p.ItemType      = clone.type;
                p.TimeToLive    = Main.rand.Next(60, 80);
            });
        }
        else
        {
            CommonCode.DropItem(dropPosition, new EntitySource_TileEntity(this), selectedItemId, 1);
        }
    }

    private bool HasAvailableLoot()
        => SampledItems.Any(id => id > 0 && !BlacklistedItems.Contains(id));

    
    // Sound helpers
    

    public override void OnKill()
    {
        StopActiveSounds();
        base.OnKill();
    }

    protected Vector2 ActiveSoundPosition => Position.ToVector2() * 16f + new Vector2(MachineTile.Width, MachineTile.Height) * 8f;

    protected float ActiveSoundPowerProgress => RatedPowerProgress > 0f ? RatedPowerProgress : 1f;

    protected bool ShouldPlayActiveSound => !Main.dedServ && Main.hasFocus && IsRunning;

    protected virtual void UpdateActiveSounds()
    {
    }

    protected virtual void StopActiveSounds()
    {
    }

    protected void UpdateLoopedActiveSound(ref SlotId slot, SoundStyle soundStyle, string identifier, Func<float> getVolume, Func<float> getPitch)
    {
        if (!ShouldPlayActiveSound)
        {
            StopLoopedActiveSound(ref slot);
            return;
        }

        if (SoundEngine.TryGetActiveSound(slot, out ActiveSound activeSound))
        {
            UpdateLoopedActiveSound(activeSound, getVolume, getPitch);
            return;
        }

        slot = SoundEngine.PlaySound(soundStyle with
        {
            IsLooped = true,
            Volume = 1f,
            Pitch = getPitch(),
            MaxInstances = 1,
            PlayOnlyIfFocused = true,
            PauseBehavior = PauseBehavior.StopWhenGamePaused,
            Identifier = $"Macrocosm/{identifier}/{GetType().FullName}/{ID}"
        },
        ActiveSoundPosition, updateCallback: (sound) =>
        {
            if (!ShouldPlayActiveSound)
                return false;

            UpdateLoopedActiveSound(sound, getVolume, getPitch);
            return true;
        });
    }

    private void UpdateLoopedActiveSound(ActiveSound activeSound, Func<float> getVolume, Func<float> getPitch)
    {
        activeSound.Position = ActiveSoundPosition;
        activeSound.Volume = getVolume();
        activeSound.Pitch = getPitch();
    }

    protected void StopLoopedActiveSound(ref SlotId slot)
    {
        if (SoundEngine.TryGetActiveSound(slot, out ActiveSound activeSound))
            activeSound.Stop();

        slot = SlotId.Invalid;
    }

    
    // Net / Save
    

    protected override void ConsumerNetSend(BinaryWriter writer)
    {
        base.ConsumerNetSend(writer);

        writer.Write(BlacklistedItems.Count);
        foreach (int itemId in BlacklistedItems)
            writer.Write(itemId);

        writer.Write(SampledItems.Length);
        foreach (int id in SampledItems)
            writer.Write(id);
    }

    protected override void ConsumerNetReceive(BinaryReader reader)
    {
        base.ConsumerNetReceive(reader);

        int blacklistedCount = reader.ReadInt32();
        BlacklistedItems = new(blacklistedCount);
        for (int i = 0; i < blacklistedCount; i++)
            BlacklistedItems.Add(reader.ReadInt32());

        int sampledCount = reader.ReadInt32();
        SampledItems = new int[sampledCount];
        for (int i = 0; i < sampledCount; i++)
            SampledItems[i] = reader.ReadInt32();
    }

    protected override void ConsumerSaveData(TagCompound tag)
    {
        base.ConsumerSaveData(tag);

        tag[nameof(BlacklistedItems)] = BlacklistedItems;
        tag[nameof(SampledItems)]     = SampledItems;
    }

    protected override void ConsumerLoadData(TagCompound tag)
    {
        base.ConsumerLoadData(tag);

        if (tag.ContainsKey(nameof(BlacklistedItems)))
            BlacklistedItems = tag.GetList<int>(nameof(BlacklistedItems)) as List<int>;

        if (tag.ContainsKey(nameof(SampledItems)))
            SampledItems = tag.Get<int[]>(nameof(SampledItems)) ?? Array.Empty<int>();
    }
}
