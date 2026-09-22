using Macrocosm.Common.Systems.Connectors;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Players;

public class ConveyorToolPlayer : ModPlayer
{
    public ConveyorToolState Wrench = ConveyorToolState.Default;
    public ConveyorToolState Tablet = ConveyorToolState.Default;
    public ulong NextOperation;

    public override void OnEnterWorld() => NextOperation = 0;

    public override void SaveData(TagCompound tag)
    {
        Save(tag, "ConveyorWrench", Wrench);
        Save(tag, "ConveyorTablet", Tablet);
    }

    private static void Save(TagCompound tag, string key, ConveyorToolState state)
        => tag[key] = new TagCompound { ["Colors"] = (int)state.Colors, ["Component"] = (int)state.Component, ["Cutting"] = state.Cutting };

    public override void LoadData(TagCompound tag)
    {
        Wrench = Read(tag, "ConveyorWrench");
        Wrench.Component = ConveyorToolComponent.Pipes;
        Tablet = Read(tag, "ConveyorTablet");
    }

    private static ConveyorToolState Read(TagCompound tag, string key)
    {
        if (!tag.ContainsKey(key)) return ConveyorToolState.Default;
        TagCompound value = tag.GetCompound(key);
        var state = new ConveyorToolState { Colors = (byte)value.GetInt("Colors"), Component = (ConveyorToolComponent)value.GetInt("Component"), Cutting = value.GetBool("Cutting") };
        return state.IsValid ? state : ConveyorToolState.Default;
    }
}
