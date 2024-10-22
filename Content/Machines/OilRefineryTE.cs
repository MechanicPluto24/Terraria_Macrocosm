using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Macrocosm.Content.Machines
{
    public class OilRefineryTE : MachineTE, IInventoryOwner
    {
        public override MachineTile MachineTile => ModContent.GetInstance<OilRefinery>();

        public override MachineType MachineType => MachineType.Consumer;

        public float InputTankAmount { get; private set; }
        public virtual float SourceTankCapacity => 100f;
        public float OutputTankAmount { get; private set; }
        public virtual float ResultTankCapacity => 100f;

        public float RefineProgress => refineTimer / (float)maxRefineTimer;

        public Item InputSlot => Inventory[0];
        public Item ContainerSlot => Inventory[1];
        public Item OutputSlot => Inventory[2];

        public Inventory Inventory { get; set; }
        protected virtual int InventorySize => 3;
        public Vector2 InventoryItemDropLocation => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

        private bool refining;

        private int inputExtractTimer;
        private const int maxInputExtractTimer = 60;

        private int refineTimer;
        private const int maxRefineTimer = 60;

        private int fillTimer;
        private const int maxFillTimer = 60;

        public bool CanRefine => Operating && InputTankAmount > 0f && OutputTankAmount < ResultTankCapacity;

        public void StartRefining()
        {
            if (CanRefine)
                refining = true;
        }

        public override void OnFirstUpdate()
        {
            // Create new inventory if none found on world load
            Inventory ??= new(InventorySize, this);

            // Assign inventory owner if the inventory was found on load
            // IInvetoryOwner does not work well with TileEntities >:(
            if (Inventory.Owner is null)
                Inventory.Owner = this;
        }

        public override void MachineUpdate()
        {
            StartRefining();

            Power = 5f;

            Extract();
            Refine();
            FillContainers();

            InputTankAmount = MathHelper.Clamp(InputTankAmount, 0, SourceTankCapacity);
            OutputTankAmount = MathHelper.Clamp(OutputTankAmount, 0, ResultTankCapacity);
        }

        private void Extract()
        {
            LiquidExtractData data = ItemSets.LiquidExtractData[InputSlot.type];
            if (Operating && InputTankAmount < SourceTankCapacity && data.Valid)
            {
                inputExtractTimer++;
                if (inputExtractTimer >= maxInputExtractTimer)
                {
                    inputExtractTimer = 0;

                    if (InputSlot.stack <= 1)
                        InputSlot.TurnToAir();
                    else
                        InputSlot.stack--;

                    InputTankAmount += data.ExtractedAmount;
                }
            }
            else
            {
                inputExtractTimer = 0;
            }
        }

        private void Refine()
        {
            if (!CanRefine)
                refining = false;

            if (refining)
            {
                refineTimer++;

                if (refineTimer >= maxRefineTimer)
                {
                    refineTimer = 0;

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
            LiquidContainerData containerData = ItemSets.LiquidContainerData[ContainerSlot.type];
            LiquidContainerData outputData = ItemSets.LiquidContainerData[OutputSlot.type];
            if (containerData.Valid && containerData.Empty)
            {
                if (OutputTankAmount > 0f && !ContainerSlot.IsAir)
                {
                    fillTimer++;
                    if (fillTimer >= maxFillTimer)
                    {
                        fillTimer = 0;

                        int fillType = LiquidContainerData.GetFillType(ItemSets.LiquidContainerData, LiquidType.RocketFuel, ContainerSlot.type);
                        OutputSlot.type = fillType;
                        OutputSlot.stack++;

                        if (ContainerSlot.stack <= 1)
                            ContainerSlot.TurnToAir();
                        else
                            ContainerSlot.stack--;

                        OutputTankAmount -= ItemSets.LiquidContainerData[fillType].Capacity;
                    }
                }
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            TagIO.Write(Inventory.SerializeData(), writer);

            writer.Write(InputTankAmount);
            writer.Write(OutputTankAmount);
        }

        public override void NetReceive(BinaryReader reader)
        {
            TagCompound tag = TagIO.Read(reader);
            Inventory = Inventory.DeserializeData(tag);

            InputTankAmount = reader.ReadSingle();
            OutputTankAmount = reader.ReadSingle();
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(Inventory)] = Inventory;

            if (InputTankAmount != default)
                tag[nameof(InputTankAmount)] = InputTankAmount;

            if (OutputTankAmount != default)
                tag[nameof(OutputTankAmount)] = OutputTankAmount;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Inventory)))
                Inventory = tag.Get<Inventory>(nameof(Inventory));

            if (tag.ContainsKey(nameof(InputTankAmount)))
                InputTankAmount = tag.GetFloat(nameof(InputTankAmount));

            if (tag.ContainsKey(nameof(OutputTankAmount)))
                OutputTankAmount = tag.GetFloat(nameof(OutputTankAmount));
        }
    }
}
