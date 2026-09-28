using Terraria.ModLoader;
using Macrocosm.Common.Systems.Connectors;

namespace Macrocosm.Content.Items.Connectors;

[LegacyName("ConveyorWrenchGreen")]
public class PipeWrenchGreen : PipeWrench
{
    public override PipeType PipeType => PipeType.GreenPipe;
}
