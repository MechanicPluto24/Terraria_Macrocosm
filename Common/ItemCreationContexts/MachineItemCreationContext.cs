using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.ItemCreationContexts;

public class MachineItemCreationContext : ItemCreationContext
{
    public Item DestinationStack { get; }
    public MachineTE Machine { get; }
    public MachineItemCreationContext(Item destinationStack, MachineTE machine)
    {
        DestinationStack = destinationStack;
        Machine = machine;
    }
}
