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

public class OilRefineryTE : ConsumerTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<OilRefinery>();

    public float InputTankAmount { get; private set; }
    public virtual float SourceTankCapacity => 100f;
    public float OutputTankAmount { get; private set; }
    public virtual float ResultTankCapacity => 100f;

    public Item ContainerSlot => Inventory[0];
    public Item OutputSlot => Inventory[1];

    public override int InventorySize => 1 + 1 + 5;

    private bool refining;

    private float inputExtractTimer;
    private float refineTimer;
    private float fillTimer;

    private float ExtractRate => 60;
    private float RefineRate => 60;
    private float FillRate => 60;

    public float ExtractProgress => inputExtractTimer / ExtractRate;
    public float RefineProgress => refineTimer / RefineRate;


    public bool CanRefine => PoweredOn && InputTankAmount > 0f && OutputTankAmount < ResultTankCapacity;

    public void StartRefining()
    {
        if (CanRefine)
            refining = true;
    }

    public override void OnFirstUpdate()
    {
        for (int i = 0; i <= 1; i++)
        {
            Inventory.SetReserved(
                i,
                (item) => item.type >= ItemID.None && ItemSets.LiquidContainerData[item.type].Valid,
                Language.GetText("Mods.Macrocosm.Machines.Common.LiquidContainer"),
                ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
            );
        }

        for (int i = 2; i < Inventory.Size; i++)
        {
            Inventory.SetReserved(
                i,
                (item) => item.type >= ItemID.None && ItemSets.LiquidExtractData[item.type].Valid,
                Language.GetText("Mods.Macrocosm.Machines.Common.LiquidExtract"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Blueprints/LiquidExtract")
            );
        }
    }

    public override void MachineUpdate()
    {
        StartRefining();

        MaxPower = 5f;

        Extract();
        Refine();
        FillContainers();

        InputTankAmount = MathHelper.Clamp(InputTankAmount, 0, SourceTankCapacity);
        OutputTankAmount = MathHelper.Clamp(OutputTankAmount, 0, ResultTankCapacity);
    }

    private void Extract()
    {
        for (int i = 2; i < Inventory.Size; i++)
        {
            Item inputItem = Inventory[i];
            LiquidExtractData data = ItemSets.LiquidExtractData[inputItem.type];
            if (PoweredOn && InputTankAmount < SourceTankCapacity && data.Valid)
            {
                inputExtractTimer += 1f * PowerProgress;
                if (inputExtractTimer >= ExtractRate)
                {
                    inputExtractTimer -= ExtractRate;
                    Inventory[i].DecreaseStack();
                    InputTankAmount += data.ExtractedAmount;
                }

                return;
            }
        }

        inputExtractTimer = 0;
    }

    private void Refine()
    {
        if (!CanRefine)
            refining = false;

        if (refining)
        {
            refineTimer += 1f * PowerProgress;
            if (refineTimer >= RefineRate)
            {
                refineTimer -= RefineRate;

                InputTankAmount -= 20f;
                OutputTankAmount += 15f;
            }
        }
        else
        {
            refineTimer = 0;
        }
    }

    private void FillContainers()
    {
        LiquidContainerData data = ItemSets.LiquidContainerData[ContainerSlot.type];
        if (data.Valid && data.Empty && OutputTankAmount > 0f && !ContainerSlot.IsAir && OutputSlot.stack <= OutputSlot.maxStack)
        {
            fillTimer += 1f * PowerProgress;
            if (fillTimer >= FillRate)
            {
                fillTimer -= FillRate;

                int fillType = LiquidContainerData.GetFillType(ItemSets.LiquidContainerData, LiquidLoader.LiquidType<RocketFuel>(), ContainerSlot.type);
                if (fillType > 0)
                {
                    Item filledItem = new(fillType);
                    filledItem.OnCreated(new MachineItemCreationContext(filledItem, this));

                    if (!Inventory.TryPlacingItem(ref filledItem, sound: false, serverSync: true, startIndex: 1, endIndex: 1) && filledItem.stack > 0)
                        Item.NewItem(new EntitySource_TileEntity(this), InventoryPosition, filledItem);

                    ContainerSlot.DecreaseStack();
                    OutputTankAmount -= ItemSets.LiquidContainerData[fillType].Capacity;
                }
            }
        }
    }

    protected override void ConsumerNetSend(BinaryWriter writer)
    {
        base.ConsumerNetSend(writer);

        writer.Write(InputTankAmount);
        writer.Write(OutputTankAmount);
    }

    protected override void ConsumerNetReceive(BinaryReader reader)
    {
        base.ConsumerNetReceive(reader);

        InputTankAmount = reader.ReadSingle();
        OutputTankAmount = reader.ReadSingle();
    }

    protected override void ConsumerSaveData(TagCompound tag)
    {
        base.ConsumerSaveData(tag);

        if (InputTankAmount != default)
            tag[nameof(InputTankAmount)] = InputTankAmount;

        if (OutputTankAmount != default)
            tag[nameof(OutputTankAmount)] = OutputTankAmount;
    }

    protected override void ConsumerLoadData(TagCompound tag)
    {
        base.ConsumerLoadData(tag);

        if (tag.ContainsKey(nameof(InputTankAmount)))
            InputTankAmount = tag.GetFloat(nameof(InputTankAmount));

        if (tag.ContainsKey(nameof(OutputTankAmount)))
            OutputTankAmount = tag.GetFloat(nameof(OutputTankAmount));
    }
}
