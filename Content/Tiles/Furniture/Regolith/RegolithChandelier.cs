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
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith
{
    public class RegolithChandelier : ModTile, IToggleableTile
    {
        private static Asset<Texture2D> glowmask;
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Chandeliers, 0));
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.Chandeliers];
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            DustType = ModContent.DustType<RegolithDust>();
            AddMapEntry(new(201, 201, 204), Language.GetText("ItemName.Chandelier"));
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
            Color color = new(111, 255, 211);
            if ((tile.TileFrameX == 0 || tile.TileFrameX == 18 * 2) && (tile.TileFrameY >= 18 * 1 && tile.TileFrameY <= 18 * 2))
                tile.GetEmmitedLight(color, applyPaint: true, out r, out g, out b);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileObjectData.IsTopLeft(i, j))
                Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

            return false;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileRendering.DrawMultiTileInWindTopAnchor(i, j, windHeightSensitivityOverride: 1f, windOffsetFactorY: 0f);

            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            TileRendering.DrawMultiTileInWindTopAnchor(i, j, glowmask, Color.White, applyPaint: true, windHeightSensitivityOverride: 1f, windOffsetFactorY: 0f);
        }
    }
}
