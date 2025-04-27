using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines.Generators.Fuel
{
    public class BurnerGeneratorTE : GeneratorTE
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
        public override int InventorySize => 6;

        public override void OnFirstUpdate()
        {
            Inventory.SetReserved(
                 (item) => item.type >= ItemID.None && ItemSets.FuelData[item.type].Potency > 0,
                 Language.GetText("Mods.Macrocosm.Machines.Common.BurnFuel"),
                 ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Blueprints/BurnFuel")
            );
        }

        public override void MachineUpdate()
        {
            // If powered off and it was not a manual action, try finding fuel and turn of if so
            if (!PoweredOn && !ManuallyTurnedOff)
            {
                bool fuelFound = false;
                foreach (Item item in Inventory)
                {
                    if (item.stack <= 0)
                        continue;

                    var fuelData = ItemSets.FuelData[item.type];
                    bool canBurn = fuelData.Potency > 0 && fuelData.CanBurn(item, Position.ToWorldCoordinates());
                    if (canBurn)
                    {
                        fuelFound = true;
                        break;
                    }
                }

                if (fuelFound)
                {
                    TurnOn(automatic: true);
                }
            }

            // If current item is not there (absend or consumed) ...
            if (ConsumedItem.IsAir)
            {
                bool fuelFound = false;
                if (PoweredOn)
                {
                    burnTimer = 0;

                    // ... find it
                    foreach (Item item in Inventory)
                    {
                        if (item.stack <= 0)
                            continue;

                        var fuelData = ItemSets.FuelData[item.type];
                        bool canBurn = fuelData.Potency > 0 && fuelData.CanBurn(item, Position.ToWorldCoordinates());
                        if (canBurn)
                        {
                            fuelFound = true;

                            ConsumedItem = new Item(item.type, 1);
                            HullHeatProgress += HullHeatRate * (float)fuelData.Potency;
                            item.stack--;

                            if (item.stack <= 0)
                                item.TurnToAir();

                            break;
                        }
                    }
                }

                // Decrease heat if not found
                if (!fuelFound)
                {
                    HullHeatProgress -= HullHeatRate * 5f;
                }
            }
            else
            {
                // Consume the item
                var fuelData = ItemSets.FuelData[ConsumedItem.type];
                if (fuelData.Potency > 0)
                {
                    HullHeatProgress += HullHeatRate * (float)fuelData.Potency;

                    if (++burnTimer >= fuelData.ConsumptionRate)
                    {
                        burnTimer = 0;
                        fuelData.OnConsumed(ConsumedItem.Clone(), Position.ToWorldCoordinates());
                        ConsumedItem = new(0);
                    }
                    else
                    {
                        fuelData.Burning(ConsumedItem, Position.ToWorldCoordinates());
                    }
                }
            }

            MaxGeneratedPower = 5f;
            GeneratedPower = HullHeatProgress * MaxGeneratedPower;
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);

            ItemIO.Send(ConsumedItem, writer);
            writer.Write(hullHeatProgress);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);

            ConsumedItem = ItemIO.Receive(reader);
            hullHeatProgress = reader.ReadSingle();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            tag[nameof(ConsumedItem)] = ItemIO.Save(ConsumedItem);
            if (hullHeatProgress > 0f)
                tag[nameof(hullHeatProgress)] = hullHeatProgress;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            if (tag.ContainsKey(nameof(ConsumedItem)))
                ItemIO.Load(ConsumedItem, tag.GetCompound(nameof(ConsumedItem)));

            if (tag.ContainsKey(nameof(hullHeatProgress)))
                hullHeatProgress = tag.GetFloat(nameof(hullHeatProgress));
        }
    }
}
