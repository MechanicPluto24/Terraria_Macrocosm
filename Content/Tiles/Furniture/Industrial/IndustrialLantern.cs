using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.Industrial
{
    public class IndustrialLantern : ModTile, IToggleableTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.MultiTileSway[Type] = true;
            TileSets.RandomStyles[Type] = 2;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);

            AdjTiles = [TileID.HangingLanterns];
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            DustType = ModContent.DustType<IndustrialPlatingDust>();

            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.Lantern"));

            // All styles
            RegisterItemDrop(ModContent.ItemType<Items.Furniture.Industrial.IndustrialLantern>());
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

        // Workaround for platform hanging, alternates don't work currently
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 18 && tile.TileFrameY < 18 * 2)
                tile.GetEmmitedLight(Color.White, applyPaint: true, out r, out g, out b);
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
}
