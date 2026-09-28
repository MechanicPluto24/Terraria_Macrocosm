using Terraria.ModLoader;
using Macrocosm.Common.Systems.Connectors;

namespace Macrocosm.Content.Items.Connectors;

[LegacyName("ConveyorWrenchBlue")]
public class PipeWrenchBlue : PipeWrench
{
    public override PipeType PipeType => PipeType.BluePipe;
}
