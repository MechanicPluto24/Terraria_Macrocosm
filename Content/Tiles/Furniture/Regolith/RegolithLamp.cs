using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith;

public class RegolithLamp : ModTile, IToggleableTile
{
    private static Asset<Texture2D> glowmask;

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Lamps, 0));
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        AdjTiles = [TileID.Lamps];
        DustType = ModContent.DustType<RegolithDust>();
        AddMapEntry(new(201, 201, 204), Language.GetText("ItemName.Lamp"));
    }

    public void ToggleTile(int i, int j, bool skipWire = false)
    {
        Tile tile = Main.tile[i, j];
        int topY = j - tile.TileFrameY / 18 % 3;
        short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);

        for (int y = topY; y < topY + 3; y++)
        {
            Main.tile[i, y].TileFrameX += frameAdjustment;

            if (skipWire && Wiring.running)
                Wiring.SkipWire(i, y);
        }

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendTileSquare(-1, i, topY, 1, 3);
    }

    public override void HitWire(int i, int j)
    {
        ToggleTile(i, j, skipWire: true);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        Color color = new(111, 255, 211);
        if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            tile.GetEmmitedLight(color, applyPaint: true, out r, out g, out b);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        TileRendering.DrawTileExtraTexture(i, j, spriteBatch, glowmask, applyPaint: true, Color.White);
    }
}
