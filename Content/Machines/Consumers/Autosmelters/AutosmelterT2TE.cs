using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Macrocosm.Common.Systems.Power;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autosmelters;

public class AutosmelterT2TE : AutocrafterTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<AutosmelterT2>();
    public override int OutputSlots => 2;

    protected override int[] AvailableCraftingStations =>
    [
        TileID.Furnaces,
        TileID.Hellforge,
        TileID.AdamantiteForge
    ];

    public override void MachineUpdate()
    {
        MaxPower = 0.1f;
        base.MachineUpdate();
    }
}

