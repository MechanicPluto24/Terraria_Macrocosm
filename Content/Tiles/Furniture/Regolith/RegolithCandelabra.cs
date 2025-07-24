using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith;

public class RegolithCandelabra : ModTile, IToggleableTile
{
    private static Asset<Texture2D> flame;

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table, 2, 0);
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.DrawYOffset = 2;

        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        AdjTiles = [TileID.Candelabras];

        DustType = ModContent.DustType<RegolithDust>();
        AddMapEntry(new(201, 201, 204), Language.GetText("ItemName.Candelabra"));

    }

    public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / (18 * 2));

    public void ToggleTile(int i, int j, bool skipWire = false)
    {
        int leftX = i - Main.tile[i, j].TileFrameX / 18 % 2;
        int topY = j - Main.tile[i, j].TileFrameY / 18 % 2;

        for (int x = leftX; x < leftX + 2; x++)
        {
            for (int y = topY; y < topY + 2; y++)
            {
                if (Main.tile[x, y].TileFrameX / 18 / 2 == 1)
                    Main.tile[x, y].TileFrameX -= 36;
                else
                    Main.tile[x, y].TileFrameX += 36;

                if (skipWire && Wiring.running)
                    Wiring.SkipWire(x, y);
            }
        }

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendTileSquare(-1, leftX, topY, 2, 2);
    }


    public override void HitWire(int i, int j)
    {
        ToggleTile(i, j, skipWire: true);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        Color color = new(111, 255, 211);
        if (tile.TileFrameX == 0)
            tile.GetEmmitedLight(color, applyPaint: false, out r, out g, out b);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        flame ??= ModContent.Request<Texture2D>(Texture + "_Flame");
        ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
        for (int k = 0; k < 7; k++)
        {
            float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
            float yy = Utils.RandomInt(ref randSeed, -10, 0) * 0.35f;

            TileRendering.DrawTileExtraTexture(i, j, spriteBatch, flame, applyPaint: false, drawColor: new Color(50, 50, 50, 0), drawOffset: new Vector2(xx, yy));
        }
    }
}
