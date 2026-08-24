using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines.Generators.Fuel;

public class KeroseneGeneratorTE : GeneratorTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<KeroseneGenerator>();

    private const int InputSlotCount = 2;
    private const int OutputSlotStart = InputSlotCount;
    private const int OutputSlotCount = 2;
    private const int TicksPerFuelUnit = 12;
    private const FuelPotency RocketFuelPotency = FuelPotency.VeryHigh;

    public float RPMProgress
    {
        get => rpmProgress;
        set => rpmProgress = MathHelper.Clamp(value, 0f, 1f);
    }
    protected float rpmProgress;

    /// <summary> The hull heat, in degrees Celsius </summary>
    public float RPM => 6000f * RPMProgress;

    /// <summary> The burning progress of the <see cref="ConsumedItem"/> </summary>
    public float BurnProgress => ConsumedItem.type != ItemID.None ? 1f - (float)burnTimer / GetRocketFuelConsumptionRate(ConsumedItem.type) : 0f;
    protected int burnTimer;

    /// <summary> The rate at which <see cref="RPMProgress"/> changes. </summary>
    public float RPMRate => 0.0001f;

    /// <summary> The item currently being burned. </summary>
    public Item ConsumedItem { get; set; } = new(ItemID.None);
    public override int InventorySize => InputSlotCount + OutputSlotCount;

    public override void OnFirstUpdate()
    {
        for (int i = 0; i < InputSlotCount; i++)
        {
            Inventory.SetSlotRole(i, InventorySlotRole.Input);
            Inventory.SetReserved(
                 i,
                 CanConsumeAsFuel,
                 Language.GetText("Mods.Macrocosm.Machines.Common.LiquidContainer"),
                 ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
            );
        }

        for (int i = OutputSlotStart; i < InventorySize; i++)
        {
            Inventory.SetSlotRole(i, InventorySlotRole.Output);
            Inventory.SetReserved(
                 i,
                 IsEmptyContainer,
                 Language.GetText("Mods.Macrocosm.Machines.Common.LiquidContainer"),
                 ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
            );
        }
    }

    public override void MachineUpdate()
    {
        if (!IsOnFrame)
        {
            bool fuelFound = false;

            for (int i = 0; i < InputSlotCount; i++)
            {
                Item item = Inventory[i];
                if (item.stack <= 0)
                    continue;

                if (CanConsumeAsFuel(item))
                {
                    fuelFound = true;
                    break;
                }
            }

            if (fuelFound && IsEnabledByPlayer)
            {
                TurnOn(automatic: true);
            }
        }

        if (ConsumedItem.type == ItemID.None)
        {
            bool fuelFound = false;

            if (IsOnFrame)
            {
                burnTimer = 0;
                for (int i = 0; i < InputSlotCount; i++)
                {
                    Item item = Inventory[i];
                    if (item.stack <= 0)
                        continue;

                    if (CanConsumeAsFuel(item))
                    {
                        fuelFound = true;
                        ConsumedItem = new Item(item.type, 1);
                        RPMProgress += RPMRate * (float)RocketFuelPotency;
                        item.DecreaseStack();

                        break;
                    }
                }
            }

            // RPM winds down whenever nothing is burning, regardless of whether the machine is on or off.
            if (!fuelFound)
                RPMProgress -= RPMRate * 10;
        }
        else
        {
            if (CanConsumeAsFuel(ConsumedItem))
            {
                RPMProgress += RPMRate * (float)RocketFuelPotency;

                if (++burnTimer >= GetRocketFuelConsumptionRate(ConsumedItem.type))
                {
                    burnTimer = 0;
                    ReturnEmptyContainer(ConsumedItem.type);
                    ConsumedItem.TurnToAir(fullReset: true);
                }
            }
        }

        MaxGeneratedPower = 200f;
        GeneratedPower = RPMProgress * MaxGeneratedPower;
    }

    protected override void GeneratorNetSend(BinaryWriter writer)
    {
        base.GeneratorNetSend(writer);
        ItemIO.Send(ConsumedItem, writer);
        writer.Write(rpmProgress);
    }
    protected override void GeneratorNetReceive(BinaryReader reader)
    {
        base.GeneratorNetReceive(reader);
        ConsumedItem = ItemIO.Receive(reader);
        rpmProgress = reader.ReadSingle();
    }
    protected override void GeneratorSaveData(TagCompound tag)
    {
        base.GeneratorSaveData(tag);

        tag[nameof(ConsumedItem)] = ItemIO.Save(ConsumedItem);
        if (rpmProgress > 0f)
            tag[nameof(rpmProgress)] = rpmProgress;
    }
    protected override void GeneratorLoadData(TagCompound tag)
    {
        base.GeneratorLoadData(tag);

        if (tag.ContainsKey(nameof(ConsumedItem)))
            ItemIO.Load(ConsumedItem, tag.GetCompound(nameof(ConsumedItem)));

        if (tag.ContainsKey(nameof(rpmProgress)))
            rpmProgress = tag.GetFloat(nameof(rpmProgress));
    }

    private static bool CanConsumeAsFuel(Item item)
    {
        LiquidContainerData data = ItemSets.LiquidContainerData[item.type];
        return !item.IsAir && data.Valid && !data.Empty && !data.Infinite && data.LiquidType == LiquidLoader.LiquidType<RocketFuel>();
    }

    private static bool IsEmptyContainer(Item item)
    {
        if (item is null || item.IsAir)
            return false;

        LiquidContainerData data = ItemSets.LiquidContainerData[item.type];
        return data.Valid && data.Empty
            && LiquidContainerData.GetFillType(ItemSets.LiquidContainerData, LiquidLoader.LiquidType<RocketFuel>(), item.type) > ItemID.None;
    }

    private void ReturnEmptyContainer(int filledContainerType)
    {
        int emptyType = LiquidContainerData.GetEmptyType(ItemSets.LiquidContainerData, filledContainerType);
        if (emptyType <= ItemID.None)
            return;

        Item emptyContainer = new(emptyType);
        if (!Inventory.TryPlacingItem(ref emptyContainer, InventoryPlacementSource.Internal, sound: false, serverSync: true, startIndex: OutputSlotStart, endIndex: InventorySize - 1) && emptyContainer.stack > 0)
            Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, emptyContainer);
    }

    private static int GetRocketFuelConsumptionRate(int containerType)
    {
        FuelData fuelData = ItemSets.FuelData[containerType];
        if (fuelData.Potency > 0 && fuelData.ConsumptionRate > 0)
            return fuelData.ConsumptionRate;

        LiquidContainerData containerData = ItemSets.LiquidContainerData[containerType];
        if (containerData.Valid && !containerData.Empty && !containerData.Infinite)
            return (int)(containerData.Capacity * TicksPerFuelUnit);

        return 1;
    }
}
