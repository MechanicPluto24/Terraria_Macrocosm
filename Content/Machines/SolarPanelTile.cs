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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = false;
            DustType = ModContent.DustType<IndustrialPlatingDust>();
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 100, 200), CreateMapEntryName());
        }
    }
}
