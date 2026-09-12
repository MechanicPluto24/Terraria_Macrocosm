using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Hevea;

public class HeveaCandle : ModTile, IToggleableTile
{
    private static Asset<Texture2D> flameTexture;

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Candles, 0));
        TileObjectData.newTile.CoordinateHeights = [20];

        TileObjectData.addTile(Type);

        AdjTiles = [TileID.Candles];
        TileID.Sets.RoomNeeds.CountsAsTorch[Type] = true;

        DustType = ModContent.DustType<HeveaDust>();

        AddMapEntry(HeveaFurnitureUtils.MapColor, Language.GetText("MapObject.Candle"));
    }

    public void ToggleTile(int i, int j, bool skipWire = false)
    {
        if (Main.tile[i, j].TileFrameX >= 18)
            Main.tile[i, j].TileFrameX -= 18;
        else
            Main.tile[i, j].TileFrameX += 18;

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendTileSquare(-1, i, j, 1, 1);
    }

    public override void HitWire(int i, int j)
    {
        ToggleTile(i, j, skipWire: true);
    }

    public override bool RightClick(int i, int j)
    {
        ToggleTile(i, j, skipWire: false);
        return true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, TileObjectData.GetTileStyle(Main.tile[i, j]));
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX == 0)
            tile.GetEmmitedLight(HeveaFurnitureUtils.LightColor, applyPaint: false, out r, out g, out b);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        flameTexture ??= ModContent.Request<Texture2D>(Texture + "_Flame");
        ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

        for (int k = 0; k < 7; k++)
        {
            float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
            float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

            TileRendering.DrawTileExtraTexture(i, j, spriteBatch, flameTexture, applyPaint: false, drawColor: new Color(50, 50, 50, 0), drawOffset: new Vector2(xx, yy));
        }
    }
}
