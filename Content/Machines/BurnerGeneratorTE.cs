using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines
{
    public class BurnerGeneratorTE : GeneratorTE, IInventoryOwner
    {
        public override MachineTile MachineTile => ModContent.GetInstance<BurnerGenerator>();

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
            Inventory.SetReserved(
                 (item) => item.type >= ItemID.None && ItemSets.FuelData[item.type].Valid,
                 Language.GetText("Mods.Macrocosm.Machines.Common.BurnFuel"),
                 ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Blueprints/BurnFuel")
            );

            // Assign inventory owner if the inventory was found on load
            // IInvetoryOwner does not work well with TileEntities >:(
            if (Inventory.Owner is null)
                Inventory.Owner = this;
        }

        public override void MachineUpdate()
        {
            if (!PoweredOn)
            {
                bool fuelFound = false;

                foreach (Item item in Inventory)
                {
                    if (item.stack <= 0)
                        continue;

                    var fuelData = ItemSets.FuelData[item.type];
                    if (fuelData.Valid)
                    {
                        fuelFound = true;
                        break;
                    }
                }

                if (fuelFound && !ManuallyTurnedOff)
                {
                    TurnOn(automatic: true);
                }
            }

            if (ConsumedItem.IsAir)
            {
                if (PoweredOn)
                {
                    burnTimer = 0;
                    foreach (Item item in Inventory)
                    {
                        if (item.stack <= 0)
                            continue;

                        var fuelData = ItemSets.FuelData[item.type];
                        if (fuelData.Valid)
                        {
                            ConsumedItem = new Item(item.type, 1);
                            HullHeatProgress += HullHeatRate * (float)fuelData.Potency;
                            item.stack--;

                            if (item.stack <= 0)
                                item.TurnToAir();

                            break;
                        }
                    }
                }
                else
                {
                    HullHeatProgress -= HullHeatRate * 5f;
                }
            }
            else
            {
                var fuelData = ItemSets.FuelData[ConsumedItem.type];
                if (fuelData.Valid)
                {
                    HullHeatProgress += HullHeatRate * (float)fuelData.Potency;

                    if (++burnTimer >= fuelData.ConsumptionRate)
                    {
                        burnTimer = 0;
                        ConsumedItem = new(0);
                    }
                }
            }

            MaxGeneratedPower = 5f;
            GeneratedPower = HullHeatProgress * MaxGeneratedPower;
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);

            Inventory ??= new(InventorySize, this);
            TagIO.Write(Inventory.SerializeData(), writer);

            ItemIO.Send(ConsumedItem, writer);

            writer.Write(hullHeatProgress);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);

            TagCompound tag = TagIO.Read(reader);
            Inventory = Inventory.DeserializeData(tag);

            ConsumedItem = ItemIO.Receive(reader);

            hullHeatProgress = reader.ReadSingle();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            tag[nameof(Inventory)] = Inventory;

            tag[nameof(ConsumedItem)] = ItemIO.Save(ConsumedItem);

            if (hullHeatProgress > 0f)
                tag[nameof(hullHeatProgress)] = hullHeatProgress;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            if (tag.ContainsKey(nameof(Inventory)))
                Inventory = tag.Get<Inventory>(nameof(Inventory));

            if (tag.ContainsKey(nameof(ConsumedItem)))
                ItemIO.Load(ConsumedItem, tag.GetCompound(nameof(ConsumedItem)));

            if (tag.ContainsKey(nameof(hullHeatProgress)))
                hullHeatProgress = tag.GetFloat(nameof(hullHeatProgress));
        }
    }
}
