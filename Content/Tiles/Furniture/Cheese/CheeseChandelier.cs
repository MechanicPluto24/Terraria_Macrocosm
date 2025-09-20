using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Cheese;

public class CheeseChandelier : ModTile, IToggleableTile
{
    private static Asset<Texture2D> flameTexture;

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileID.Sets.MultiTileSway[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Chandeliers, 0));
        TileObjectData.addTile(Type);

        AdjTiles = [TileID.Chandeliers];
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        DustType = ModContent.DustType<CheeseDust>();

        AddMapEntry(new Color(220, 216, 121), Language.GetText("ItemName.Chandelier"));
    }

    public void ToggleTile(int i, int j, bool skipWire = false)
    {
        int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
        int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
        for (int x = left; x < left + 3; x++)
        {
            for (int y = top; y < top + 3; y++)
            {
                if (Main.tile[x, y].TileFrameX >= 54)
                    Main.tile[x, y].TileFrameX -= 54;
                else
                    Main.tile[x, y].TileFrameX += 54;

                if (skipWire && Wiring.running)
                    Wiring.SkipWire(x, y);
            }
        }

        NetMessage.SendTileSquare(-1, left, top, 3, 3);
    }

    public override void HitWire(int i, int j)
    {
        ToggleTile(i, j, skipWire: true);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX == 0)
            tile.GetEmmitedLight(new Color(87, 230, 158), applyPaint: false, out r, out g, out b);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (TileObjectData.IsTopLeft(i, j))
            Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.MultiTileVine);

        return false;
    }

    public override void AdjustMultiTileVineParameters(int i, int j, ref float? overrideWindCycle, ref float windPushPowerX, ref float windPushPowerY, ref bool dontRotateTopTiles, ref float totalWindMultiplier, ref Texture2D glowTexture, ref Color glowColor)
    {
        overrideWindCycle = 1f;
        windPushPowerY = 0f;
    }

    public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
    {
        flameTexture ??= ModContent.Request<Texture2D>(Texture + "_Flame");
        tileFlameData.flameTexture = flameTexture.Value;
        tileFlameData.flameSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

        tileFlameData.flameCount = 12;
        tileFlameData.flameColor = new Color(25, 25, 25, 0);

        tileFlameData.flameRangeXMin = -10;
        tileFlameData.flameRangeXMax = 11;
        tileFlameData.flameRangeYMin = -10;
        tileFlameData.flameRangeYMax = 1;
        tileFlameData.flameRangeMultX = 0.15f;
        tileFlameData.flameRangeMultY = 0.35f;
    }
}
