using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using Terraria;
using Macrocosm.Content.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelTile : MachineTile
    {
        public override short Width => 1;
        public override short Height => 1;
        public override MachineTE MachineTE => ModContent.GetInstance<SolarPanelTileTE>();

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            AddMapEntry(new Color(100, 100, 200), CreateMapEntryName());
        }
    }
}
