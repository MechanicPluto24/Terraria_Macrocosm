using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines;

public class SolarPanelTile : MachineTile
{
    public override short Width => 1;
    public override short Height => 1;
    public override MachineTE MachineTE => ModContent.GetInstance<SolarPanelTileTE>();

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = false;

        DustType = ModContent.DustType<IndustrialPlatingDust>();

        AddMapEntry(new Color(100, 100, 200), CreateMapEntryName());
    }
}
