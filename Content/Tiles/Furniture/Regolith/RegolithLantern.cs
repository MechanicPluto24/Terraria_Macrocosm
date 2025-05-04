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
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Regolith
{
    public class RegolithLantern : ModTile, IToggleableTile
    {
        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.MultiTileSway[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.HangingLanterns, 0));
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = [TileID.HangingLanterns];
            DustType = ModContent.DustType<RegolithDust>();
            AddMapEntry(new(201, 201, 204), Language.GetText("ItemName.Lantern"));
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
            Color color = new(111, 255, 211);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 18 * 1)
                tile.GetEmmitedLight(color, applyPaint: true, out r, out g, out b);
        }

        // Workaround for platform hanging, alternates don't work currently
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Point16 topLeft = TileObjectData.TopLeft(i, j);
            if (WorldGen.IsBelowANonHammeredPlatform(topLeft.X, topLeft.Y))
                offsetY -= 8;
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

            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            glowTexture = TileRendering.GetOrPreparePaintedExtraTexture(Main.tile[i, j], glowmask);
            glowColor = Color.White;
        }
    }
}
