﻿using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Cheese
{
    public class CheeseLantern : ModTile, IToggleableTile
    {
        private static Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.HangingLanterns, 0));
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.HangingLanterns];
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            DustType = ModContent.DustType<CheeseDust>();

            AddMapEntry(new Color(220, 216, 121), Language.GetText("ItemName.Lantern"));
        }

        public void ToggleTile(int i, int j, bool skipWire = false)
        {
            Tile tile = Main.tile[i, j];
            int topY = j - tile.TileFrameY / 18 % 2;
            short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);

            for (int y = topY; y < topY + 2; y++)
            {
                Main.tile[i, y].TileFrameX += frameAdjustment;

                if (skipWire && Wiring.running)
                    Wiring.SkipWire(i, y);
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, topY, 1, 2);
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

        // Workaround for platform hanging, alternates don't work currently
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Point16 topLeft = Utility.GetMultitileTopLeft(i, j);
            if (WorldGen.IsBelowANonHammeredPlatform(topLeft.X, topLeft.Y))
                offsetY -= 8;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (TileObjectData.IsTopLeft(tile))
                TileRendering.AddCustomSpecialPoint(i, j, CustomSpecialDraw);

            return false; // We must return false here to prevent the normal tile drawing code from drawing the default static tile. Without this a duplicate tile will be drawn.
        }

        public void CustomSpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileRendering.DrawMultiTileInWindTopAnchor(i, j, windHeightSensitivityOverride: 1f, windOffsetFactorY: 0f);

            flameTexture ??= ModContent.Request<Texture2D>(Texture + "_Flame");
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                TileRendering.DrawMultiTileInWindTopAnchor(i, j, flameTexture, drawColor: new Color(100, 100, 100, 0), drawOffset: new Vector2(xx, yy), applyPaint: false, windHeightSensitivityOverride: 1f, windOffsetFactorY: 0f);
            }
        }
    }
}
