using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria.Utilities;
using Terraria.GameContent;
using static Terraria.GameContent.Drawing.TileDrawing;

namespace Macrocosm.Common.Utils
{
    public partial class Utility
    {
        private static FieldInfo tilePaintSystemV2_requests_fieldInfo;

        private static FieldInfo tileDrawing_treeWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_grassWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_sunflowerWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_vineWindCounter_fieldInfo;

        private static FieldInfo tileDrawing_leafFrequency_fieldInfo;
        private static FieldInfo tileDrawing_rand_fieldInfo;

        private static FieldInfo _specialPositionsField;
        private static FieldInfo _specialTileXField;
        private static FieldInfo _specialTileYField;

        public static double TreeWindCounter
        {
            get
            {
                tileDrawing_treeWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_treeWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_treeWindCounter_fieldInfo.GetValue(Main.instance.TilesRenderer);
            }
        }

        public static double GrassWindCounter
        {
            get
            {
                tileDrawing_grassWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_grassWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_grassWindCounter_fieldInfo.GetValue(Main.instance.TilesRenderer);
            }
        }

        public static double SunflowerWindCounter
        {
            get
            {
                tileDrawing_sunflowerWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_sunflowerWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_sunflowerWindCounter_fieldInfo.GetValue(Main.instance.TilesRenderer);
            }
        }

        public static double VineWindCounter
        {
            get
            {
                tileDrawing_vineWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_vineWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_vineWindCounter_fieldInfo.GetValue(Main.instance.TilesRenderer);
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
            tilePaintSystemV2_requests_fieldInfo ??= typeof(TilePaintSystemV2).GetField("_requests", BindingFlags.NonPublic | BindingFlags.Instance);
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

        public static Texture2D GetOrPreparePaintedExtraTexture(this Tile tile, Asset<Texture2D> asset)
        {
            if (!TileDrawing.IsVisible(tile))
                return Macrocosm.EmptyTex.Value;

            Texture2D texture = asset.Value;
            TilePaintSystemV2.TileVariationkey key = new()
            {
                TileType = tile.TileType,
                TileStyle = asset.Name.GetHashCode(),
                PaintColor = tile.TileColor
            };

            if (tileExtraTextureRenders.TryGetValue(key, out var value) && value.IsReady)
            {
                texture = (Texture2D)(object)value.Target;
                return texture;
            }
            else
            {
                value = new TileRenderTargetHolder(asset) { Key = key };
                tileExtraTextureRenders.TryAdd(key, value);

                if (!value.IsReady)
                    TilePaintSystemV2_AddRequest(value);
            }

            return texture;
        }

        public static void DrawTileExtraTexture(int i, int j, SpriteBatch spriteBatch, Asset<Texture2D> asset, bool applyPaint, Vector2 drawOffset = default, Color? drawColor = null)
        {
            Tile tile = Main.tile[i, j];
            if (!TileDrawing.IsVisible(tile))
                return;

            Texture2D texture = asset.Value;
            if (applyPaint)
                texture = tile.GetOrPreparePaintedExtraTexture(asset);

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

        public static Point[] GetTileRenderSpecialPoints(TileCounterType type)
        {
            _specialPositionsField ??= typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            Point[][] specialPositions = (Point[][])_specialPositionsField.GetValue(Main.instance.TilesRenderer);

            if (specialPositions == null || type >= TileCounterType.Count)
                return Array.Empty<Point>();

            return specialPositions[(int)type] ?? Array.Empty<Point>();
        }

        public static Point[] GetTileRenderSpecialLegacyPoints()
        {
            _specialTileXField ??= typeof(TileDrawing).GetField("_specialTileX", BindingFlags.NonPublic | BindingFlags.Instance);
            _specialTileYField ??= typeof(TileDrawing).GetField("_specialTileY", BindingFlags.NonPublic | BindingFlags.Instance);

            int[] specialTileX = (int[])_specialTileXField.GetValue(Main.instance.TilesRenderer);
            int[] specialTileY = (int[])_specialTileYField.GetValue(Main.instance.TilesRenderer);

            if (specialTileX == null || specialTileY == null)
                return Array.Empty<Point>();

            Point[] points = new Point[specialTileX.Length];
            for(int i = 0; i < specialTileX.Length; i++)
                points[i] = new Point(specialTileX[i], specialTileY[i]);

            return points;
        }

        /// <summary>
        /// Checks if the given (i, j) coordinate is a special point for a specific <see cref="TileCounterType"/>.
        /// </summary>
        /// <param name="i">The X coordinate of the tile.</param>
        /// <param name="j">The Y coordinate of the tile.</param>
        /// <param name="type">The <see cref="TileCounterType"/> to check.</param>
        /// <returns>True if the point is a special point for the given type, otherwise false.</returns>
        public static bool IsTileRenderSpecialPoint(int i, int j, TileCounterType type) => GetTileRenderSpecialPoints(type).Any(p => p.X == i && p.Y == j);

        /// <summary>
        /// Checks if the given (i, j) coordinate is a special legacy point.
        /// </summary>
        /// <param name="i">The X coordinate of the tile.</param>
        /// <param name="j">The Y coordinate of the tile.</param>
        /// <returns>True if the point is a special legacy point, otherwise false.</returns>
        public static bool IsTileRenderSpecialLegacyPoint(int i, int j) => GetTileRenderSpecialLegacyPoints().Any(p => p.X == i && p.Y == j);

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
