using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using System.IO;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Power;

public abstract class BatteryTE : MachineTE
{
    public float PowerFlow { get; set; }

    /// <summary> Current stored energy </summary>
    public float StoredEnergy { get; set; }

    /// <summary> Maximum storage energy </summary>
    public abstract float EnergyCapacity { get; }

    public override bool CanToggleWithWire => false;

    public override void UpdatePowerState()
    {
        if (IsOnFrame && StoredEnergy <= 0f)
            TurnOff(automatic: true);
        else if (!IsOnFrame && StoredEnergy > 0f && IsEnabledByPlayer)
            TurnOn(automatic: true);
    }

    public override Color DisplayColor => Color.Cyan;
    public override string GetPowerInfo()
    {
        string energy = $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Battery").Format($"{StoredEnergy:F2}", $"{EnergyCapacity:F2}")}";

        string flow = PowerFlow >= 0 ? $"+{PowerFlow:F2}" : $"{PowerFlow:F2}";
        flow = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format(flow);

        string percent = $"{(StoredEnergy / EnergyCapacity * 100):F2}%";

        return energy + " (" + percent + "), " + flow;
    }

    public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
    {
        string percent = $"{(StoredEnergy / EnergyCapacity * 100):F2}%";
        DrawPowerText(spriteBatch, basePosition, percent);
    }

    protected virtual void BatteryNetSend(BinaryWriter writer) { }
    protected virtual void BatteryNetReceive(BinaryReader reader) { }
    protected virtual void BatterySaveData(TagCompound tag) { }
    protected virtual void BatteryLoadData(TagCompound tag) { }

    public sealed override void MachineNetSend(BinaryWriter writer)
    {
        writer.Write(StoredEnergy);
        writer.Write(PowerFlow);
        BatteryNetSend(writer);
    }

    public sealed override void MachineNetReceive(BinaryReader reader)
    {
        StoredEnergy = reader.ReadSingle();
        PowerFlow = reader.ReadSingle();
        BatteryNetReceive(reader);
    }

    public sealed override void MachineSaveData(TagCompound tag)
    {
        if (StoredEnergy > 0f)
            tag[nameof(StoredEnergy)] = StoredEnergy;
        if (PowerFlow != 0f)
            tag[nameof(PowerFlow)] = PowerFlow;
        BatterySaveData(tag);
    }

    public sealed override void MachineLoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(StoredEnergy)))
            StoredEnergy = tag.GetFloat(nameof(StoredEnergy));
        if (tag.ContainsKey(nameof(PowerFlow)))
            PowerFlow = tag.GetFloat(nameof(PowerFlow));
        BatteryLoadData(tag);
    }
}
