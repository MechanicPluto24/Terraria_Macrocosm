using Macrocosm.Common.Utils;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Events;

public abstract class MacrocosmEvent : ModType, ILocalizedModType
{
    public static List<MacrocosmEvent> Events { get; } = [];

    protected MacrocosmEvent() { }

    protected sealed override void Register()
    {
        ModTypeLookup<MacrocosmEvent>.Register(this);
        Events.Add(this);
    }

    public sealed override void SetupContent() => SetStaticDefaults();

    public string LocalizationCategory => "Events";
    public string EventFullName => $"{Mod.Name}/{Name}";
    public LocalizedText DisplayName => Language.GetOrRegister($"Mods.{Mod.Name}.{LocalizationCategory}.{Name}.DisplayName", () => Utility.PrettyPrintName(Name));

    public abstract MacrocosmEventScope Scope { get; }

    public virtual MacrocosmEventState CreateState() => new();

    public virtual void OnInitializeState(MacrocosmEventState state, string subworldIdOrNull) { }

    public virtual void Update(MacrocosmEventContext context, MacrocosmEventState state) { }

    public virtual void OnStarted(MacrocosmEventContext context, MacrocosmEventState state) { }

    public virtual void OnEnded(MacrocosmEventContext context, MacrocosmEventState state) { }

    public virtual void SaveState(MacrocosmEventState state, Terraria.ModLoader.IO.TagCompound tag) { }

    public virtual void LoadState(MacrocosmEventState state, Terraria.ModLoader.IO.TagCompound tag) { }

    public virtual void NetSendState(MacrocosmEventState state, System.IO.BinaryWriter writer) { }

    public virtual void NetReceiveState(MacrocosmEventState state, System.IO.BinaryReader reader) { }
}
