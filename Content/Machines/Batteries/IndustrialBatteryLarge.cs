using Macrocosm.Common.Drawing;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils; 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Machines.Batteries;

public class IndustrialBatteryLarge : MachineTile
{
    private static Asset<Texture2D> glowmask;

    public override short Width => 5;
    public override short Height => 5;
    public override MachineTE MachineTE => ModContent.GetInstance<IndustrialBatteryLargeTE>();

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.DefaultToMachine(this);
        TileObjectData.newTile.Origin = new Point16(2, 4); 
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, Width, 0);
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;
        DustType = -1;

        AddMapEntry(new Color(139, 75, 14), CreateMapEntryName());
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

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        float fill = 0f;
        if (TileEntity.ByPosition.TryGetValue(TileObjectData.TopLeft(i, j), out var entity) && entity is BatteryTE batt)
            fill = batt.EnergyCapacity > 0 ? batt.StoredEnergy / batt.EnergyCapacity : 0f;
        TileRendering.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true, Color.White * fill);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        int tileOffsetX = tile.TileFrameX % (Width * 18) / 18;
        int tileOffsetY = tile.TileFrameY % (Height * 18) / 18;
        bool powered = IsPoweredOnFrame(i, j);

        float fill = 0f;
        if (TileEntity.ByPosition.TryGetValue(TileObjectData.TopLeft(i, j), out var entity) && entity is BatteryTE batt)
            fill = batt.EnergyCapacity > 0 ? batt.StoredEnergy / batt.EnergyCapacity : 0f;

        if (powered && (tileOffsetX is 0 or 2 or 4) && (tileOffsetY is 1 or 2 or 3))
            tile.GetEmmitedLight(new Color(233, 214, 92) * fill, applyPaint: true, out r, out g, out b);
    }
}
