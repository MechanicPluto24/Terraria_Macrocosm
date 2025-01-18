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
using Macrocosm.Common.DataStructures;
using static Macrocosm.Content.Tiles.Furniture.Industrial.IndustrialChest;

namespace Macrocosm.Common.Utils
{
    public partial class Utility
    {
        public static TileDrawing TileRenderer => Main.instance.TilesRenderer;

        private static FieldInfo tilePaintSystemV2_requests_fieldInfo;

        private static MethodInfo tileDrawing_DrawAnimatedTile_AdjustForVisionChangers_methodInfo;
        private static MethodInfo tileDrawing_DrawTiles_GetLightOverride_methodInfo;
        private static MethodInfo tileDrawing_GetHighestWindGridPushComplex_methodInfo;

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
                return (double)tileDrawing_treeWindCounter_fieldInfo.GetValue(TileRenderer);
            }
        }

        public static double GrassWindCounter
        {
            get
            {
                tileDrawing_grassWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_grassWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_grassWindCounter_fieldInfo.GetValue(TileRenderer);
            }
        }

        public static double SunflowerWindCounter
        {
            get
            {
                tileDrawing_sunflowerWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_sunflowerWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_sunflowerWindCounter_fieldInfo.GetValue(TileRenderer);
            }
        }

        public static double VineWindCounter
        {
            get
            {
                tileDrawing_vineWindCounter_fieldInfo ??= typeof(TileDrawing).GetField("_vineWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                return (double)tileDrawing_vineWindCounter_fieldInfo.GetValue(TileRenderer);
            }
        }


        public static int TreeLeafFrequency
        {
            get
            {
                tileDrawing_leafFrequency_fieldInfo ??= typeof(TileDrawing).GetField("_leafFrequency", BindingFlags.NonPublic | BindingFlags.Instance);
                return (int)tileDrawing_leafFrequency_fieldInfo.GetValue(TileRenderer);
            }
        }

        public static UnifiedRandom TileRendererRandom
        {
            get
            {
                tileDrawing_rand_fieldInfo ??= typeof(TileDrawing).GetField("_rand", BindingFlags.NonPublic | BindingFlags.Instance);
                return (UnifiedRandom)tileDrawing_rand_fieldInfo.GetValue(TileRenderer);
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

        private static SpriteBatchState state1;
        public static void DrawMultiTileInWind_GrassStyle(int topLeftX, int topLeftY, Asset<Texture2D> textureOverride = null, Color? drawColor = null, Vector2 drawOffset = default, bool applyPaint = false, float windFactor = 0.07f)
        {
            state1.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, state1.Matrix);

            Tile sourceTile = Main.tile[topLeftX, topLeftY];

            TileObjectData objectData = TileObjectData.GetTileData(sourceTile);
            int sizeX = objectData.Width;
            int sizeY = objectData.Height;

            Vector2 screenPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            float windCycle = TileRenderer.GetWindCycle(topLeftX, topLeftY, SunflowerWindCounter);

            Vector2 position = new Vector2((float)(topLeftX * 16 - (int)screenPosition.X) + (float)sizeX * 16f * 0.5f, topLeftY * 16 - (int)screenPosition.Y + 16 * sizeY) + zero;
            bool wind = WorldGen.InAPlaceWithWind(topLeftX, topLeftY, sizeX, sizeY);

            for (int i = topLeftX; i < topLeftX + sizeX; i++)
            {
                for (int j = topLeftY; j < topLeftY + sizeY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    ushort type = tile.TileType;
                    if (type != sourceTile.TileType || !TileDrawing.IsVisible(tile))
                        continue;

                    short tileFrameX = tile.TileFrameX;
                    short tileFrameY = tile.TileFrameY;
                    float yMod = 1f - (float)(j - topLeftY + 1) / (float)sizeY;
                    if (yMod == 0f)
                        yMod = 0.1f;

                    if (!wind)
                        yMod = 0f;

                    TileRenderer.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out var _, out var _, out var _);
                    bool canDoDust = TileRendererRandom.NextBool(4);
                    Color tileLight = Lighting.GetColor(i, j);
                    TileDrawing_DrawAnimatedTile_AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, canDoDust);
                    tileLight = TileDrawing_DrawTiles_GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);
                    tileLight = drawColor ?? tileLight;

                    Vector2 tilePos = new Vector2(i * 16 - (int)screenPosition.X, j * 16 - (int)screenPosition.Y + tileTop) + zero;
                    Vector2 windMod = new(windCycle * 1f, Math.Abs(windCycle) * 2f * yMod);
                    Vector2 origin = position - tilePos;
                    Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                    Vector2 drawPos = position + new Vector2(0f, windMod.Y) + drawOffset;

                    Texture2D tileDrawTexture;
                    if (textureOverride != null)
                        tileDrawTexture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, textureOverride) : textureOverride.Value;
                    else
                        tileDrawTexture = TileRenderer.GetTileDrawTexture(tile, i, j);

                    if (tileDrawTexture != null)
                    {
                        Main.spriteBatch.Draw(tileDrawTexture, drawPos, frame, tileLight, windCycle * windFactor * yMod, origin, 1f, tileSpriteEffect, 0f);
                    }
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state1);
        }


        // TODO: figure out why it's more mixely than vanilla rendering
        private static SpriteBatchState state2;
        public static void DrawMultiTileInWind_VineStyle(int topLeftX, int topLeftY, Asset<Texture2D> textureOverride = null, Color? drawColor = null, Vector2 drawOffset = default, bool applyPaint = false, float windFactor = 0.07f, bool extraOffset = false, Vector2? windModOverride = null, float? yModOverride = null)
        {
            state2.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, state2.Matrix);

            Tile sourceTile = Main.tile[topLeftX, topLeftY];
            TileObjectData objectData = TileObjectData.GetTileData(sourceTile);
            int sizeX = objectData.Width;
            int sizeY = objectData.Height;

            Vector2 screenPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            float windCycle = TileRenderer.GetWindCycle(topLeftX, topLeftY, SunflowerWindCounter);

            float num = windCycle;
            int totalPushTime = 60;
            float pushForcePerFrame = 1.26f;
            float highestWindGridPushComplex = TileDrawing_GetHighestWindGridPushComplex(topLeftX, topLeftY, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
            windCycle += highestWindGridPushComplex;

            Vector2 position = new Vector2((float)(topLeftX * 16 - (int)screenPosition.X) + (float)sizeX * 16f * 0.5f, topLeftY * 16 - (int)screenPosition.Y) + zero;
            Vector2 verticalDrawOffset = new Vector2(0f, -2f);
            position += verticalDrawOffset;

            bool isBelowNonHammeredPlatform = true;
            for (int i = 0; i < sizeX; i++)
            {
                if (WorldGen.IsBelowANonHammeredPlatform(topLeftX + i, topLeftY))
                    continue;

                isBelowNonHammeredPlatform = false;
                break;
            }

            if (isBelowNonHammeredPlatform)
            {
                position.Y -= 8f;
                verticalDrawOffset.Y -= 8f;
            }

            float windModX = windModOverride.HasValue ? windModOverride.Value.X : 1f;
            float windModY = windModOverride.HasValue ? windModOverride.Value.X : -4f;
            windFactor = 0.15f;

            if (extraOffset)
                position += new Vector2(0f, 16f);

            windFactor *= -1f;
            if (!WorldGen.InAPlaceWithWind(topLeftX, topLeftY, sizeX, sizeY))
                windCycle -= num;

            for (int i = topLeftX; i < topLeftX + sizeX; i++)
            {
                for (int j = topLeftY; j < topLeftY + sizeY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    ushort type = tile.TileType;
                    if (type != sourceTile.TileType || !TileDrawing.IsVisible(tile))
                        continue;

                    short tileFrameX = tile.TileFrameX;
                    short tileFrameY = tile.TileFrameY;
                    float heightModifier = (float)(j - topLeftY + 1) / (float)sizeY;
                    if (heightModifier == 0f)
                        heightModifier = 0.1f;

                    if (yModOverride.HasValue)
                        heightModifier = yModOverride.Value;

                    if (extraOffset && j == topLeftY)
                        heightModifier = 0f;

                    TileRenderer.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out var _, out var _, out var _);
                    bool canDoDust = TileRendererRandom.NextBool(4);
                    Color tileLight = Lighting.GetColor(i, j);
                    TileDrawing_DrawAnimatedTile_AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, canDoDust);
                    tileLight = TileDrawing_DrawTiles_GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);
                    tileLight = drawColor ?? tileLight;

                    Vector2 tilePos = new Vector2(i * 16 - (int)screenPosition.X, j * 16 - (int)screenPosition.Y + tileTop) + zero;
                    tilePos += verticalDrawOffset;
                    Vector2 windModifier = new(windCycle * windModX, Math.Abs(windCycle) * windModY * heightModifier);
                    Vector2 origin = position - tilePos;

                    Texture2D tileDrawTexture;
                    if (textureOverride != null)
                        tileDrawTexture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, textureOverride) : textureOverride.Value;
                    else
                        tileDrawTexture = TileRenderer.GetTileDrawTexture(tile, i, j);

                    if (tileDrawTexture != null)
                    {
                        Vector2 drawPos = position + new Vector2(0f, windModifier.Y) + drawOffset;
                        Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                        float rotation = windCycle * windFactor * heightModifier;
                        Main.spriteBatch.Draw(tileDrawTexture, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
                    }
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state2);
        }

        public static void TileDrawing_DrawAnimatedTile_AdjustForVisionChangers(int i, int j, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, ref Color tileLight, bool canDoDust)
        {
            tileDrawing_DrawAnimatedTile_AdjustForVisionChangers_methodInfo ??= typeof(TileDrawing).GetMethod("DrawAnimatedTile_AdjustForVisionChangers", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_DrawAnimatedTile_AdjustForVisionChangers_methodInfo.Invoke(TileRenderer, [i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight, canDoDust]);
        }

        public static Color TileDrawing_DrawTiles_GetLightOverride(int j, int i, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight)
        {
            tileDrawing_DrawTiles_GetLightOverride_methodInfo ??= typeof(TileDrawing).GetMethod("DrawTiles_GetLightOverride", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Color)tileDrawing_DrawTiles_GetLightOverride_methodInfo.Invoke(TileRenderer, [i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight]);
        }

        public static float TileDrawing_GetHighestWindGridPushComplex(int topLeftX, int topLeftY, int sizeX, int sizeY, int totalPushTime, float pushForcePerFrame, int loops, bool swapLoopDir)
        {
            tileDrawing_GetHighestWindGridPushComplex_methodInfo ??= typeof(TileDrawing).GetMethod("GetHighestWindGridPushComplex", BindingFlags.NonPublic | BindingFlags.Instance);
            return (float)tileDrawing_GetHighestWindGridPushComplex_methodInfo.Invoke(TileRenderer, [topLeftX, topLeftY, sizeX, sizeY, totalPushTime, pushForcePerFrame, loops, swapLoopDir]);
        }

        public static Point[] GetTileRenderSpecialPoints(TileDrawing.TileCounterType type)
        {
            _specialPositionsField ??= typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            Point[][] specialPositions = (Point[][])_specialPositionsField.GetValue(TileRenderer);

            if (specialPositions == null || type >= TileDrawing.TileCounterType.Count)
                return Array.Empty<Point>();

            return specialPositions[(int)type] ?? Array.Empty<Point>();
        }

        public static Point[] GetTileRenderSpecialLegacyPoints()
        {
            _specialTileXField ??= typeof(TileDrawing).GetField("_specialTileX", BindingFlags.NonPublic | BindingFlags.Instance);
            _specialTileYField ??= typeof(TileDrawing).GetField("_specialTileY", BindingFlags.NonPublic | BindingFlags.Instance);

            int[] specialTileX = (int[])_specialTileXField.GetValue(TileRenderer);
            int[] specialTileY = (int[])_specialTileYField.GetValue(TileRenderer);

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
        public static bool IsTileRenderSpecialPoint(int i, int j, TileDrawing.TileCounterType type) => GetTileRenderSpecialPoints(type).Any(p => p.X == i && p.Y == j);

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
            TileRenderer.GetTileDrawData(
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
            TileRenderer.GetTileDrawData(
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
            TileRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out _, out _, out _, out _,
                out addFrameX, out addFrameY,
                out _, out _, out _, out _
            );
        }
    }
}
