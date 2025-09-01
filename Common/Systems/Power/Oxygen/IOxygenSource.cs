namespace Macrocosm.Common.Systems.Power.Oxygen;

public interface IOxygenSource
{
    public bool IsProvidingOxygen { get; }
    public int MaxRoomSize { get; }
}
