namespace Macrocosm.Common.Systems.Power.Oxygen;

public interface IOxygenActiveSource : IOxygenSource
{
    public int MaxPassiveSources { get; }
}
