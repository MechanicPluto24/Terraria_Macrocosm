using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.TileFrame;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Solar
{
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

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.PlatingStyle(i, j, resetFrame);
            return false;
        }
    }
}
