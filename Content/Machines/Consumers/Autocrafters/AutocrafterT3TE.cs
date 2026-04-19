using Macrocosm.Common.Systems.Power;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters;

public class AutocrafterT3TE : AutocrafterTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<AutocrafterT3>();
    public override int OutputSlots => 4;

    protected override int[] AvailableCraftingStations =>
    [
        TileID.WorkBenches,
        TileID.Anvils,
        TileID.MythrilAnvil,
        ModContent.TileType<Tiles.Crafting.Fabricator>(),
        TileID.LunarCraftingStation
    ];

    public override void MachineUpdate()
    {
        MaxPower = 65f;
        base.MachineUpdate();
    }
}
