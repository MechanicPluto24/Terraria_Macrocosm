using Macrocosm.Common.Systems.Connectors;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Players;

public class PipeToolPlayer : ModPlayer
{
    public PipeToolState Wrench = PipeToolState.Default;
    public PipeToolState Tablet = PipeToolState.Default;
    public ulong NextOperation;

    public override void OnEnterWorld() => NextOperation = 0;

    public override void SaveData(TagCompound tag)
    {
        Save(tag, "PipeWrench", Wrench);
        Save(tag, "PipeTablet", Tablet);
    }

    private static void Save(TagCompound tag, string key, PipeToolState state)
        => tag[key] = new TagCompound { ["Colors"] = (int)state.Colors, ["Component"] = (int)state.Component, ["Cutting"] = state.Cutting };

    public override void LoadData(TagCompound tag)
    {
        Wrench = Read(tag, "PipeWrench");
        Wrench.Component = PipeToolComponent.Pipes;
        Tablet = Read(tag, "PipeTablet");
    }

    private static PipeToolState Read(TagCompound tag, string key)
    {
        if (!tag.ContainsKey(key)) return PipeToolState.Default;
        TagCompound value = tag.GetCompound(key);
        var state = new PipeToolState { Colors = (byte)value.GetInt("Colors"), Component = (PipeToolComponent)value.GetInt("Component"), Cutting = value.GetBool("Cutting") };
        return state.IsValid ? state : PipeToolState.Default;
    }
}
