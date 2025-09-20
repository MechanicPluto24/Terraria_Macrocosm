using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Generators.Steam;

public class SteamEngine : MachineTile
{
    public override short Width => 6;
    public override short Height => 4;
    public override MachineTE MachineTE => ModContent.GetInstance<SteamEngineTE>();

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

        DustType = -1;

        AddMapEntry(new Color(140, 140, 140), CreateMapEntryName());
    }

    public override bool IsPoweredOnFrame(int i, int j) => Main.tile[i, j].TileFrameY >= Height * 18;

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

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        if (IsPoweredOnFrame(i, j))
            frameYOffset = 18 * Height * Main.tileFrame[type];
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        int ticksPerFrame = 3;
        int frameCount = 16;  
        if (++frameCounter >= ticksPerFrame)
        {
            frameCounter = 0;
            if (++frame >= frameCount)
                frame = 0;
        }
    }
}

