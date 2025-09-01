using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Macrocosm.Common.Systems.Power;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autosmelters;

public class AutosmelterT3TE : AutocrafterTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<AutosmelterT3>();
    public override int OutputSlots => 4;

    protected override int[] AvailableCraftingStations =>
    [
        TileID.Furnaces,
        TileID.GlassKiln,
        TileID.Hellforge,
        TileID.AdamantiteForge
    ];

    public override void MachineUpdate()
    {
        MaxPower = 0.1f;
        base.MachineUpdate();
    }
}

