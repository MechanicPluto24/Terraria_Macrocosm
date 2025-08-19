using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Items.LiquidContainers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Machines;

public class KeroseneGeneratorTE : GeneratorTE, IInventoryOwner
{
    public override MachineTile MachineTile => ModContent.GetInstance<KeroseneGenerator>();

    public float RPMProgress
    {
        get => rpmProgress;
        set => rpmProgress = MathHelper.Clamp(value, 0f, 1f);
    }
    protected float rpmProgress;

    /// <summary> The hull heat, in degrees Celsius </summary>
    public float RPM => 6000f * RPMProgress;

    /// <summary> The burning progress of the <see cref="ConsumedItem"/> </summary>
    public float BurnProgress => ConsumedItem.type != ItemID.None ? 1f - (float)burnTimer / ItemSets.FuelData[ConsumedItem.type].ConsumptionRate : 0f;
    protected int burnTimer;

    /// <summary> The rate at which <see cref="RPMProgress"/> changes. </summary>
    public float RPMRate => 0.0001f;

    /// <summary> The item currently being burned. </summary>
    public Item ConsumedItem { get; set; } = new(ItemID.None);

    public Inventory Inventory { get; set; }
    protected virtual int InventorySize => 1;
    public Vector2 InventoryPosition => base.Position.ToVector2() * 16 + new Vector2(MachineTile.Width, MachineTile.Height) * 16 / 2;

    public override void OnFirstUpdate()
    {
        // Create new inventory if none found on world load
        Inventory ??= new(InventorySize, this);
        Inventory.SetReserved(
             (item) => item.type >= ItemID.None && ItemSets.FuelData[item.type].Potency > 0 && item.type == ModContent.ItemType<RocketFuelCanister>(),
             Language.GetText("Kerosene Canister"),
             ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
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
                if (fuelData.Potency > 0 && item.type == ModContent.ItemType<RocketFuelCanister>())
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

        if (ConsumedItem.type == ItemID.None)
        {
            if (PoweredOn)
            {
                burnTimer = 0;
                foreach (Item item in Inventory)
                {
                    if (item.stack <= 0)
                        continue;

                    var fuelData = ItemSets.FuelData[item.type];
                    if (fuelData.Potency > 0 && item.type == ModContent.ItemType<RocketFuelCanister>())
                    {
                        ConsumedItem = new Item(item.type, 1);
                        RPMProgress += RPMRate * (float)fuelData.Potency;
                        item.stack--;

                        if (item.stack <= 0)
                            item.TurnToAir();

                        break;
                    }
                }

                if (ConsumedItem.type == ItemID.None)
                {
                    RPMProgress -= RPMRate * 10;
                }
            }
        }
        else
        {
            var fuelData = ItemSets.FuelData[ConsumedItem.type];
            if (fuelData.Potency > 0 && ConsumedItem.type == ModContent.ItemType<RocketFuelCanister>())
            {
                RPMProgress += RPMRate * (float)fuelData.Potency;

                if (++burnTimer >= fuelData.ConsumptionRate)
                {
                    burnTimer = 0;
                    ConsumedItem.TurnToAir(fullReset: true);
                }
            }
        }

        MaxGeneratedPower = 15f;
        GeneratedPower = RPMProgress * MaxGeneratedPower;
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);

        Inventory ??= new(InventorySize, this);
        TagIO.Write(Inventory.SerializeData(), writer);

        ItemIO.Send(ConsumedItem, writer);

        writer.Write(rpmProgress);
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);

        TagCompound tag = TagIO.Read(reader);
        Inventory = Inventory.DeserializeData(tag);

        ConsumedItem = ItemIO.Receive(reader);

        rpmProgress = reader.ReadSingle();
    }

    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);

        tag[nameof(Inventory)] = Inventory;

        tag[nameof(ConsumedItem)] = ItemIO.Save(ConsumedItem);

        if (rpmProgress > 0f)
            tag[nameof(rpmProgress)] = rpmProgress;
    }

    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);

        if (tag.ContainsKey(nameof(Inventory)))
            Inventory = tag.Get<Inventory>(nameof(Inventory));

        if (tag.ContainsKey(nameof(ConsumedItem)))
            ItemIO.Load(ConsumedItem, tag.GetCompound(nameof(ConsumedItem)));

        if (tag.ContainsKey(nameof(rpmProgress)))
            rpmProgress = tag.GetFloat(nameof(rpmProgress));
    }
}
