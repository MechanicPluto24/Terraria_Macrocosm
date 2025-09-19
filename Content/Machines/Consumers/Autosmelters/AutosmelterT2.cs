using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.UI.Machines;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Consumers.Autosmelters;

public class AutosmelterT2 : MachineTile
{
    public override short Width => 4;
    public override short Height => 4;
    public override MachineTE MachineTE => ModContent.GetInstance<AutosmelterT2TE>();

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.DefaultToMachine(this);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, Width, 0);
        TileObjectData.addTile(Type);

        AdjTiles = [TileID.Containers];
        DustType = -1;

        LocalizedText name = CreateMapEntryName();
        // style 0: Adamantite, style 1: Titanium
        AddMapEntry(new Color(165, 36, 24), name);
        AddMapEntry(new Color(140, 140, 160), name);
    }

    public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / (Width * 18));

    public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= Height * 18 * 1;

    public override void OnToggleStateFrame(int i, int j, bool skipWire = false)
    {
        Point16 origin = TileObjectData.TopLeft(i, j);
        for (int x = origin.X; x < origin.X + Width; x++)
        {
            for (int y = origin.Y; y < origin.Y + Height; y++)
            {
                Tile tile = Main.tile[x, y];

                if (IsPoweredOnFrame(x, y))
                    tile.TileFrameY -= (short)(Height * 18);
                else
                    tile.TileFrameY += (short)(Height * 18);

                if (skipWire && Wiring.running)
                    Wiring.SkipWire(x, y);
            }
        }

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendTileSquare(-1, origin.X, origin.Y, Width, Height);
    }

    public override bool RightClick(int i, int j)
    {
        Main.mouseRightRelease = false;
        Utility.UICloseOthers();
        if (TileEntity.TryGet(i, j, out MachineTE te))
            UISystem.ShowMachineUI(te, new AutocrafterUI());

        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.mouseInterface)
        {
            player.noThrow = 2;
            Main.LocalPlayer.cursorItemIconEnabled = true;
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        if (IsPoweredOnFrame(i, j))
            frameYOffset = 18 * Height * Main.tileFrame[type];
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        int ticksPerFrame = 5;
        int frameCount = 5;
        if (++frameCounter >= ticksPerFrame)
        {
            frameCounter = 0;
            if (++frame >= frameCount)
                frame = 0;
        }
    }
}
