using Macrocosm.Common.Sets.Items;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Loot;
using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Loot.DropRules;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Blocks.Sands;
using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Materials.Ores;
using Macrocosm.Content.Items.Materials.Tech;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines
{
    public class OilRefineryTE : MachineTE, IInventoryOwner
    {
        public override MachineTile MachineTile => ModContent.GetInstance<OilRefinery>();

        public float SourceTankAmount { get; private set; }
        public virtual float SourceTankCapacity => 100f;
        public float ResultTankAmount { get; private set; }
        public virtual float ResultTankCapacity => 100f;

        public Item SourceItem => Inventory[0];
        public Item ResultItem => Inventory[1];

        public bool CanInteractWithResultSlot { get; private set; }

        public Inventory Inventory { get; set; }
        protected virtual int InventorySize => 2;
        public Vector2 InventoryItemDropLocation => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

        private bool refining;

        private int sourceTimer;
        private const int maxSourceTimer = 60;

        private int resultTimer;
        private const int maxResultTimer = 60;

        public bool CanRefine => Operating && SourceTankAmount > 0f && ResultTankAmount < ResultTankCapacity;

        public void StartRefining()
        {
            if(CanRefine)
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

            ConsumedPower = 0.6f;

            Extract();
            Refine();
            FillContainers();

            SourceTankAmount = MathHelper.Clamp(SourceTankAmount, 0, SourceTankCapacity);
            ResultTankAmount = MathHelper.Clamp(ResultTankAmount, 0, ResultTankCapacity);
        }

        private void Extract()
        {
            if (Operating && SourceTankAmount < SourceTankCapacity && SourceItem.ModItem is ILiquidExtractable extractable)
            {
                sourceTimer++;
                if (sourceTimer >= maxSourceTimer)
                {
                    sourceTimer = 0;

                    if (SourceItem.stack <= 1)
                        SourceItem.TurnToAir();
                    else
                        SourceItem.stack--;

                    SourceTankAmount += extractable.ExtractedAmount;
                }
            }
            else
            {
                sourceTimer = 0;
            }
        }

        private void Refine()
        {
            if (!CanRefine)
                refining = false;

            if (refining)
            {
                resultTimer++;

                if (resultTimer >= maxResultTimer)
                {
                    resultTimer = 0;

                    SourceTankAmount -= 10f;
                    ResultTankAmount += 5f;
                }
            }
            else
            {
                resultTimer = 0;
            }
        }

        private void FillContainers()
        {
            CanInteractWithResultSlot = true;

            if (ResultItem.ModItem is LiquidContainer container)
            {
                if (ResultTankAmount > 0f && ResultItem.stack > 0 && !container.Full)
                {
                    container.Amount += 100f / ResultItem.stack;
                    ResultTankAmount -= 1f;
                }

                CanInteractWithResultSlot = container.Full || container.Empty;
            }      
        }

        public override void NetSend(BinaryWriter writer)
        {
            TagIO.Write(Inventory.SerializeData(), writer);

            writer.Write(SourceTankAmount);
            writer.Write(ResultTankAmount);
        }

        public override void NetReceive(BinaryReader reader)
        {
            TagCompound tag = TagIO.Read(reader);
            Inventory = Inventory.DeserializeData(tag);

            SourceTankAmount = reader.ReadSingle();
            ResultTankAmount = reader.ReadSingle();
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(Inventory)] = Inventory;

            if(SourceTankAmount != default)
                tag[nameof(SourceTankAmount)] = SourceTankAmount;

            if (ResultTankAmount != default)
                tag[nameof(ResultTankAmount)] = ResultTankAmount;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Inventory)))
                Inventory = tag.Get<Inventory>(nameof(Inventory));

            if(tag.ContainsKey(nameof(SourceTankAmount)))
                SourceTankAmount = tag.GetFloat(nameof(SourceTankAmount));

            if (tag.ContainsKey(nameof(ResultTankAmount)))
                ResultTankAmount = tag.GetFloat(nameof(ResultTankAmount));
        }
    }
}
