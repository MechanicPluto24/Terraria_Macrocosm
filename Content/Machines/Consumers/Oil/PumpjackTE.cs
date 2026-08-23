using Macrocosm.Common.DataStructures;
using Macrocosm.Common.ItemCreationContexts;
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

namespace Macrocosm.Content.Machines.Consumers.Oil;

public class PumpjackTE : ConsumerTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<Pumpjack>();

    public float TankAmount { get; private set; }
    public virtual float TankCapacity => 100f;

    public Item ContainerSlot => Inventory[0];
    public Item OutputSlot => Inventory[1];

    public override int InventorySize => 2;

    private float extractTimer;
    private float ExtractRate => 60;
    public float ExtractProgress => extractTimer / ExtractRate;
    public float ExtractAmount => 1f;

    private float fillTimer;
    private float FillRate => 60;
    public float FillProgress => fillTimer / FillRate;

    public override float PowerDemand => IsEnabledByPlayer && (CanExtractOil || CanFillContainer) ? MaxPower : 0f;

    private bool CanExtractOil => TankAmount < TankCapacity;

    private bool CanFillContainer
    {
        get
        {
            LiquidContainerData containerData = ItemSets.LiquidContainerData[ContainerSlot.type];
            if (!containerData.Valid || !containerData.Empty || ContainerSlot.IsAir || TankAmount <= 0f)
                return false;

            int fillType = LiquidContainerData.GetFillType(ItemSets.LiquidContainerData, LiquidLoader.LiquidType<Liquids.Oil>(), ContainerSlot.type);
            if (fillType <= 0)
                return false;

            Item filledItem = new(fillType);
            return Inventory.TryPlacingItem(ref filledItem, InventoryPlacementSource.Internal, justCheck: true, sound: false, serverSync: false, startIndex: 1, endIndex: 1);
        }
    }

    public override void OnFirstUpdate()
    {
        Inventory.SetSlotRole(0, InventorySlotRole.Input);
        Inventory.SetSlotRole(1, InventorySlotRole.Output);

        for (int i = 0; i < InventorySize; i++)
        {
            Inventory.SetReserved(
                i,
                (item) => item.type >= ItemID.None && ItemSets.LiquidContainerData[item.type].Valid,
                Language.GetText("Mods.Macrocosm.Machines.Common.LiquidContainer"),
                ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
            );
        }

    }

    public override void MachineUpdate()
    {
        MaxPower = 80f;

        Extract();
        FillContainers();

        TankAmount = MathHelper.Clamp(TankAmount, 0, TankCapacity);
    }

    private void Extract()
    {
        if (IsRunning && CanExtractOil)
        {
            extractTimer += 1f * RatedPowerProgress;
            if (extractTimer >= ExtractRate)
            {
                extractTimer -= ExtractRate;
                TankAmount += ExtractAmount;
            }

            return;
        }

        extractTimer = 0;
    }

    private void FillContainers()
    {
        if (!IsRunning || !CanFillContainer)
            return;

        int fillType = LiquidContainerData.GetFillType(ItemSets.LiquidContainerData, LiquidLoader.LiquidType<Liquids.Oil>(), ContainerSlot.type);
        fillTimer += 1f * RatedPowerProgress;
        if (fillTimer >= FillRate)
        {
            fillTimer -= FillRate;

            Item filledItem = new(fillType);
            filledItem.OnCreated(new MachineItemCreationContext(filledItem, this));

            // Place result in OutputSlot
            if (!Inventory.TryPlacingItem(ref filledItem, InventoryPlacementSource.Internal, sound: false, serverSync: true, startIndex: 1, endIndex: 1) && filledItem.stack > 0)
                Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, filledItem);

            ContainerSlot.DecreaseStack();
            TankAmount -= ItemSets.LiquidContainerData[fillType].Capacity;
        }
    }

    protected override void ConsumerNetSend(BinaryWriter writer)
    {
        base.ConsumerNetSend(writer); 
        writer.Write(TankAmount);
    }
    protected override void ConsumerNetReceive(BinaryReader reader)
    {
        base.ConsumerNetReceive(reader);
        TankAmount = reader.ReadSingle();
    }
    protected override void ConsumerSaveData(TagCompound tag)
    {
        base.ConsumerSaveData(tag);
        if (TankAmount != default)
            tag[nameof(TankAmount)] = TankAmount;
    }
    protected override void ConsumerLoadData(TagCompound tag)
    {
        base.ConsumerLoadData(tag);
        if (tag.ContainsKey(nameof(TankAmount)))
            TankAmount = tag.GetFloat(nameof(TankAmount));
    }
}
