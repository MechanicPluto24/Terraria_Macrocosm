using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Macrocosm.Common.Systems.Power;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autosmelters;

public class AutosmelterT1TE : AutocrafterTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<AutosmelterT1>();
    public override int OutputSlots => 1;

    protected override int[] AvailableCraftingStations =>
    [
        TileID.Furnaces,
        TileID.GlassKiln
    ];

    public override void MachineUpdate()
    {
        MaxPower = 0.1f;
        base.MachineUpdate();
    }
}
