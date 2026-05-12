using Macrocosm.Common.Events;

namespace Macrocosm.Content.Events;

public class SolarStormEvent : MacrocosmEvent
{
    public override MacrocosmEventScope Scope => MacrocosmEventScope.SubworldLocal;

    public override MacrocosmEventState CreateState() => new SolarStormEventState();

    public override void Update(MacrocosmEventContext context, MacrocosmEventState state)
    {
        if (context.AppliesToCurrentSubworld)
            context.CurrentSubworld.ShouldUpdateWiring = !state.Active;
    }
}

public sealed class SolarStormEventState : MacrocosmEventState
{
}
