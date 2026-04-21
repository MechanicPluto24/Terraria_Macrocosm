using Macrocosm.Common.Subworlds;

namespace Macrocosm.Common.Events;

public sealed class MacrocosmEventContext
{
    private readonly MacrocosmEvent _event;

    internal MacrocosmEventContext(MacrocosmEvent @event, MacrocosmSubworld currentSubworld, string targetSubworldId, double worldUpdateDelta)
    {
        _event = @event;
        CurrentSubworld = currentSubworld;
        CurrentSubworldId = currentSubworld?.ID;
        TargetSubworldId = targetSubworldId;
        WorldUpdateDelta = worldUpdateDelta;
    }

    public MacrocosmSubworld CurrentSubworld { get; }
    public string CurrentSubworldId { get; }
    public string TargetSubworldId { get; }
    public double WorldUpdateDelta { get; }
    public bool AppliesToCurrentSubworld => CurrentSubworld is not null && CurrentSubworldId == TargetSubworldId;

    public void Start() => MacrocosmEventSystem.Start(_event.EventFullName, TargetSubworldId);
    public void End() => MacrocosmEventSystem.End(_event.EventFullName, TargetSubworldId);

    public bool IsActive<T>(string subworldId = null) where T : MacrocosmEvent
        => MacrocosmEventSystem.IsActive<T>(subworldId);

    public MacrocosmEventState GetState<T>(string subworldId = null) where T : MacrocosmEvent
        => MacrocosmEventSystem.GetState<T>(subworldId);
}
