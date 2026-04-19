using Macrocosm.Common.Systems.Power;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters;

public class AutocrafterT2TE : AutocrafterTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<AutocrafterT2>();
    public override int OutputSlots => 2;

    protected override int[] AvailableCraftingStations =>
    [
        TileID.WorkBenches,
        TileID.Anvils,
        TileID.MythrilAnvil,
        ModContent.TileType<Tiles.Crafting.Fabricator>()
    ];

    public override void MachineUpdate()
    {
        MaxPower = 35f;
        base.MachineUpdate();
    }
}
