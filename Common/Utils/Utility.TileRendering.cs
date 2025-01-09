using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria.Utilities;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils
{
    public partial class Utility
    {
        private static FieldInfo tilePaintSystemV2_requests_fieldInfo;
        private static FieldInfo tilePaintSystemV2_tilesRenders_fieldInfo;
        private static FieldInfo tilePaintSystemV2_wallsRenders_fieldInfo;

        private static FieldInfo tileDrawing_treeWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_leafFrequency_fieldInfo;
        private static FieldInfo tileDrawing_rand_fieldInfo;

        public static double TreeWindCounter
        {
            get
            {
                tileDrawing_treeWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_treeWindCounter", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
                return (double)tileDrawing_treeWindCounter_fieldInfo.GetValue(Main.instance.TilesRenderer);
            }
        }

        public static int TreeLeafFrequency
        {
            get
            {
                tileDrawing_leafFrequency_fieldInfo ??= typeof(TileDrawing).GetField("_leafFrequency", BindingFlags.NonPublic | BindingFlags.Instance);
                return (int)tileDrawing_leafFrequency_fieldInfo.GetValue(Main.instance.TilesRenderer);
            }
        }

        public static UnifiedRandom TileRendererRandom
        {
            get
            {
                tileDrawing_rand_fieldInfo ??= typeof(TileDrawing).GetField("_rand", BindingFlags.NonPublic | BindingFlags.Instance);
                return (UnifiedRandom)tileDrawing_rand_fieldInfo.GetValue(Main.instance.TilesRenderer);
            }
        }

        public static void TilePaintSystemV2_AddRequest(TilePaintSystemV2.ARenderTargetHolder renderTargetHolder)
        {
            tilePaintSystemV2_requests_fieldInfo ??= typeof(TilePaintSystemV2).GetField("_requests", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            var requests = (List<TilePaintSystemV2.ARenderTargetHolder>)tilePaintSystemV2_requests_fieldInfo.GetValue(Main.instance.TilePaintSystem);
            requests.Add(renderTargetHolder);
        }

        private static readonly Dictionary<TilePaintSystemV2.TileVariationkey, TileRenderTargetHolder> tileExtraTextureRenders = new();
        private class TileRenderTargetHolder(Asset<Texture2D> asset) : TilePaintSystemV2.TileRenderTargetHolder
        {
            public override void Prepare()
            {
                asset.Wait?.Invoke();
                PrepareTextureIfNecessary(asset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, TreePaintSystemData.GetTileSettings(Key.TileType, Key.TileStyle));
            }
        }

        public static void DrawTileExtraTexture(int i, int j, SpriteBatch spriteBatch, Asset<Texture2D> asset, bool applyPaint = true, Vector2 drawOffset = default, Color? drawColor = null)
        {
            Tile tile = Main.tile[i, j];
            if (!TileDrawing.IsVisible(tile))
                return;

            Texture2D texture = asset.Value;
            if (applyPaint)
            {
                TilePaintSystemV2.TileVariationkey key = new()
                {
                    TileType = tile.TileType,
                    TileStyle = 0,
                    PaintColor = tile.TileColor
                };

                if (tileExtraTextureRenders.TryGetValue(key, out var value) && value.IsReady)
                    texture = (Texture2D)(object)value.Target;

                else
                {
                    value = new TileRenderTargetHolder(asset) { Key = key };
                    tileExtraTextureRenders.TryAdd(key, value);

                    if (!value.IsReady)
                        TilePaintSystemV2_AddRequest(value);
                }
            }

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            GetTileFrameOffset(i, j, out int addFrameX, out int addFrameY);

            var data = TileObjectData.GetTileData(tile);
            int tileSize = 16 + data.CoordinatePadding;
            int width = data.CoordinateWidth;

            if (tile.TileFrameY < 0)
                return;

            int height = data.CoordinateHeights[tile.TileFrameY / tileSize % data.Height];

            drawOffset += new Vector2(data.DrawXOffset, data.DrawYOffset);
            drawColor ??= Color.White;

            Vector2 position = new Vector2(i, j) * 16 + zero + drawOffset - Main.screenPosition;
            Rectangle frame = new(tile.TileFrameX + addFrameX, tile.TileFrameY + addFrameY, width, height);

            spriteBatch.Draw(texture, position, frame, drawColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static SpriteEffects GetTileSpriteEffects(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            short drawFrameX = tile.TileFrameX;
            short drawFrameY = tile.TileFrameY;
            Main.instance.TilesRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out _, out _, out _, out _,
                out _, out _,
                out SpriteEffects effect,
                out _, out _, out _
            );

            return effect;
        }

        public static void GetTileDrawPositions(int i, int j, out int tileWidth, out int offsetY, out int tileHeight, out short drawFrameX, out short drawFrameY)
        {
            Tile tile = Main.tile[i, j];
            drawFrameX = tile.TileFrameX;
            drawFrameY = tile.TileFrameY;
            Main.instance.TilesRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out tileWidth, out tileHeight, out offsetY, out _,
                out _, out _,
                out _, out _, out _, out _
            );
        }

        public static void GetTileFrameOffset(int i, int j, out int addFrameX, out int addFrameY)
        {
            Tile tile = Main.tile[i, j];
            short drawFrameX = tile.TileFrameX;
            short drawFrameY = tile.TileFrameY;
            Main.instance.TilesRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out _, out _, out _, out _,
                out addFrameX, out addFrameY,
                out _, out _, out _, out _
            );
        }
    }
}
