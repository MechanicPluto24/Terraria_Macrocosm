using Macrocosm.Common.Drawing;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines
{
    public class OilRefinery : MachineTile
    {
        public override short Width => 4;
        public override short Height => 4;
        public override MachineTE MachineTE => ModContent.GetInstance<OilRefineryTE>();

        public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= (Height * 18);

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = false;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(MachineTE.Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            AddMapEntry(new Color(121, 107, 91), CreateMapEntryName());
        }


        public override void TogglePowerStateFrame(int i, int j)
        {
            Point16 origin = Utility.GetMultitileTopLeft(i, j);
            for (int x = origin.X; x < origin.X + Width; x++)
            {
                for (int y = origin.Y; y < origin.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (IsPoweredOnFrame(x, y))
                        tile.TileFrameY -= (short)(Height * 18);
                    else
                        tile.TileFrameY += (short)(Height * 18);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, Width, Height);
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;
            Utility.UICloseOthers();
           
            if (Utility.TryGetTileEntityAs(i, j, out OilRefineryTE refinery))
            {
                UISystem.ShowMachineUI(refinery, new OilRefineryUI());
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!UISystem.Active && !player.mouseInterface)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<Items.Machines.OilRefinery>();
            }
        }
    }
}
