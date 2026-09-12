using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Hevea;

public class HeveaChandelier : ModTile, IToggleableTile
{
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

        TileID.Sets.RoomNeeds.CountsAsTorch[Type] = true;
        AdjTiles = [TileID.Chandeliers];
        DustType = ModContent.DustType<HeveaDust>();

        AddMapEntry(HeveaFurnitureUtils.MapColor, Language.GetText("MapObject.Chandelier"));
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

        if (Main.netMode != NetmodeID.SinglePlayer)
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
            tile.GetEmmitedLight(HeveaFurnitureUtils.LightColor, applyPaint: false, out r, out g, out b);
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
}
