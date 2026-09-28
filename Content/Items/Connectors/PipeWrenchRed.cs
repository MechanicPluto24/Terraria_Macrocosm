using Terraria.ModLoader;
using Macrocosm.Common.Systems.Connectors;

namespace Macrocosm.Content.Items.Connectors;

[LegacyName("ConveyorWrenchRed")]
public class PipeWrenchRed : PipeWrench
{
    public override PipeType PipeType => PipeType.RedPipe;
}
