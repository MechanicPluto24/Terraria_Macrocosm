using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines
{
    public class BurnerGeneratorTE : MachineTE, IInventoryOwner
    {
        public override MachineTile MachineTile => ModContent.GetInstance<BurnerGenerator>();

        public override MachineType MachineType => MachineType.Generator; 

        /// <summary> The hull heat progress, 0 to 1, increases when burning and decreases otherwise. </summary>
        public float HullHeatProgress 
        { 
            get => hullHeatProgress; 
            set => hullHeatProgress = MathHelper.Clamp(value, 0f, 1f);
        }
        protected float hullHeatProgress;

        /// <summary> The hull heat, in degrees Celsius </summary>
        public float HullHeat => 1200f * HullHeatProgress;

        /// <summary> The burning progress of the <see cref="ConsumedItem"/> </summary>
        public float BurnProgress => ConsumedItem.type != ItemID.None ? 1f - (float)burnTimer / ItemSets.FuelData[ConsumedItem.type].ConsumptionRate : 0f;
        protected int burnTimer;

        /// <summary> The max power out that the Burner can have, at max <see cref="HullHeatProgress"/></summary>
        public float MaxPower => 5f;

        /// <summary> The rate at which <see cref="HullHeatProgress"/> changes. </summary>
        public float HullHeatRate => 0.00005f;

        /// <summary> The item currently being burned. </summary>
        public Item ConsumedItem { get; set; } = new(ItemID.None);

        public Inventory Inventory { get; set; }
        protected virtual int InventorySize => 6;
        public Vector2 InventoryItemDropLocation => Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

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
            CanAutoPowerOn = false;
            CanAutoPowerOff = false;

            if (PoweredOn)
            {
                if (ConsumedItem.type != ItemID.None)
                {
                    FuelData fuelData = ItemSets.FuelData[ConsumedItem.type];
                    HullHeatProgress += HullHeatRate * (float)fuelData.Potency;

                    if (burnTimer++ >= fuelData.ConsumptionRate)
                    {
                        burnTimer = 0;
                        ConsumedItem.TurnToAir(fullReset: true);
                    }
                }
                else
                {
                    burnTimer = 0;
                    bool fuelFound = false;

                    foreach (Item item in Inventory)
                    {
                        FuelData fuelData = ItemSets.FuelData[item.type];
                        if (fuelData.Potency > FuelPotency.None)
                        {
                            ConsumedItem = new(item.type, stack: 1);
                            HullHeatProgress += HullHeatRate * (float)fuelData.Potency;

                            item.stack--;
                            if (item.stack < 0)
                                item.TurnToAir();

                            fuelFound = true;
                            break;
                        }
                    }

                    if (!fuelFound)
                    {
                        Power = 0;
                        MachineTile.TogglePowerStateFrame(Position.X, Position.Y);
                    }
                }
            }
            else
            {
                HullHeatProgress -= HullHeatRate;
            }

            Power = HullHeatProgress * MaxPower;
        }

        public override void NetSend(BinaryWriter writer)
        {
            Inventory ??= new(InventorySize, this);
            TagIO.Write(Inventory.SerializeData(), writer);

            ItemIO.Send(ConsumedItem, writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            TagCompound tag = TagIO.Read(reader);
            Inventory = Inventory.DeserializeData(tag);

            ConsumedItem = ItemIO.Receive(reader);
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(Inventory)] = Inventory;
            tag[nameof(ConsumedItem)] = ItemIO.Save(ConsumedItem);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Inventory)))
                Inventory = tag.Get<Inventory>(nameof(Inventory));

            if (tag.ContainsKey(nameof(ConsumedItem)))
                ItemIO.Load(ConsumedItem, tag.GetCompound(nameof(ConsumedItem)));
        }
    }
}
