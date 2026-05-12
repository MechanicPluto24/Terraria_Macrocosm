using Macrocosm.Common.Events;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Achievements;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Events;

public class DemonSunEvent : MacrocosmEvent
{
    public static bool Active => MacrocosmEventSystem.IsActive<DemonSunEvent>();
    public static float VisualIntensity => GetState()?.VisualIntensity ?? 0f;

    public override MacrocosmEventScope Scope => MacrocosmEventScope.Global;

    public override MacrocosmEventState CreateState() => new DemonSunEventState();

    public override void Update(MacrocosmEventContext context, MacrocosmEventState state)
    {
        DemonSunEventState demonSunState = (DemonSunEventState)state;
        Main.bloodMoon = state.Active;

        demonSunState.TargetVisualIntensity = state.Active && AppliesVisuals(context.CurrentSubworld) ? 1f : 0f;

        if (demonSunState.VisualIntensity < demonSunState.TargetVisualIntensity)
            demonSunState.VisualIntensity += 0.005f;

        if (demonSunState.VisualIntensity > demonSunState.TargetVisualIntensity)
            demonSunState.VisualIntensity -= 0.005f;
    }

    public override void OnStarted(MacrocosmEventContext context, MacrocosmEventState state)
    {
        Main.bloodMoon = true;
    }

    public override void OnEnded(MacrocosmEventContext context, MacrocosmEventState state)
    {
        Main.bloodMoon = false;

        if (SubworldSystem.IsActive<Moon>())
            ModContent.GetInstance<SurviveDemonSun>()?.Condition?.Complete();
    }

    public override void LoadState(MacrocosmEventState state, TagCompound tag)
    {
        Main.bloodMoon = state.Active;
    }

    public override void NetReceiveState(MacrocosmEventState state, System.IO.BinaryReader reader)
    {
        Main.bloodMoon = state.Active;
    }

    private static DemonSunEventState GetState()
        => MacrocosmEventSystem.GetState<DemonSunEvent>() as DemonSunEventState;

    private static bool AppliesVisuals(MacrocosmSubworld subworld)
        => subworld is Moon or MoonOrbitSubworld;
}

public sealed class DemonSunEventState : MacrocosmEventState
{
    public float TargetVisualIntensity { get; set; }
    public float VisualIntensity { get; set; }
}
