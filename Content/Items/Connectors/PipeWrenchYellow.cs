using Terraria.ModLoader;
using Macrocosm.Common.Systems.Connectors;

namespace Macrocosm.Content.Items.Connectors;

[LegacyName("ConveyorWrenchYellow")]
public class PipeWrenchYellow : PipeWrench
{
    public override PipeType PipeType => PipeType.YellowPipe;
}
