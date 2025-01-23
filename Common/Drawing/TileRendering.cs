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
using Macrocosm.Common.Utils;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Common.Drawing
{
    public class TileRendering : ILoadable
    {
        public void Load(Mod mod)
        {
            On_TileDrawing.PreDrawTiles += On_TileDrawing_PreDrawTiles; ;
            On_TileDrawing.PostDrawTiles += On_TileDrawing_PostDrawTiles;

            tileDrawing_DrawAnimatedTile_AdjustForVisionChangers_methodInfo = typeof(TileDrawing).GetMethod("DrawAnimatedTile_AdjustForVisionChangers", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_DrawTiles_GetLightOverride_methodInfo = typeof(TileDrawing).GetMethod("DrawTiles_GetLightOverride", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_GetHighestWindGridPushComplex_methodInfo = typeof(TileDrawing).GetMethod("GetHighestWindGridPushComplex", BindingFlags.NonPublic | BindingFlags.Instance);

            tilePaintSystemV2_requests_fieldInfo = typeof(TilePaintSystemV2).GetField("_requests", BindingFlags.NonPublic | BindingFlags.Instance);

            tileDrawing_treeWindCounter_fieldInfo = typeof(TileDrawing).GetField("_treeWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_grassWindCounter_fieldInfo = typeof(TileDrawing).GetField("_grassWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_sunflowerWindCounter_fieldInfo = typeof(TileDrawing).GetField("_sunflowerWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_vineWindCounter_fieldInfo = typeof(TileDrawing).GetField("_vineWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);

            tileDrawing_leafFrequency_fieldInfo = typeof(TileDrawing).GetField("_leafFrequency", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_rand_fieldInfo = typeof(TileDrawing).GetField("_rand", BindingFlags.NonPublic | BindingFlags.Instance);

            tileDrawing_specialPositions_fieldInfo = typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_specialTileX_fieldInfo = typeof(TileDrawing).GetField("_specialTileX", BindingFlags.NonPublic | BindingFlags.Instance);
            tileDrawing_specialTileY_fieldInfo = typeof(TileDrawing).GetField("_specialTileY", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Unload()
        {
            On_TileDrawing.PostDrawTiles -= On_TileDrawing_PostDrawTiles;
        }

        public static TileDrawing TileRenderer => Main.instance.TilesRenderer;

        public static double TreeWindCounter => (double)tileDrawing_treeWindCounter_fieldInfo.GetValue(TileRenderer);
        public static double GrassWindCounter => (double)tileDrawing_grassWindCounter_fieldInfo.GetValue(TileRenderer);
        public static double SunflowerWindCounter => (double)tileDrawing_sunflowerWindCounter_fieldInfo.GetValue(TileRenderer);
        public static double VineWindCounter => (double)tileDrawing_vineWindCounter_fieldInfo.GetValue(TileRenderer);
        public static int TreeLeafFrequency => (int)tileDrawing_leafFrequency_fieldInfo.GetValue(TileRenderer);
        public static UnifiedRandom TileRendererRandom => (UnifiedRandom)tileDrawing_rand_fieldInfo.GetValue(TileRenderer);

        private static MethodInfo tileDrawing_DrawAnimatedTile_AdjustForVisionChangers_methodInfo;
        private static MethodInfo tileDrawing_DrawTiles_GetLightOverride_methodInfo;
        private static MethodInfo tileDrawing_GetHighestWindGridPushComplex_methodInfo;

        private static FieldInfo tilePaintSystemV2_requests_fieldInfo;

        private static FieldInfo tileDrawing_treeWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_grassWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_sunflowerWindCounter_fieldInfo;
        private static FieldInfo tileDrawing_vineWindCounter_fieldInfo;

        private static FieldInfo tileDrawing_leafFrequency_fieldInfo;
        private static FieldInfo tileDrawing_rand_fieldInfo;

        private static FieldInfo tileDrawing_specialPositions_fieldInfo;
        private static FieldInfo tileDrawing_specialTileX_fieldInfo;
        private static FieldInfo tileDrawing_specialTileY_fieldInfo;


        private static readonly Dictionary<TilePaintSystemV2.TileVariationkey, TileExtraTextureRenderTargetHolder> tileExtraTextureRenders = new();

        private static readonly Dictionary<Point, Action<int, int, SpriteBatch>> customSpecialPoints = new();

        /// <summary>
        /// Used for custom special drawing of tiles (drawcode run after single tiles are drawn, used for things who would be impossible to draw due to tile draw order).
        /// <br/> Options, in this order:
        /// <br/> - <see cref="TileDrawing.AddSpecialLegacyPoint(int, int)"/> for custom drawing. Use <see cref="ModTile.SpecialDraw(int, int, SpriteBatch)"></see>
        /// <br/> - <see cref="AddCustomSpecialPoint"/> for other custom drawing. Simply pass the drawcode action.
        /// <br/> - <see cref="TileDrawing.AddSpecialPoint"/> for vanilla style rendering of things such as tiles swaying in wind, pylons, etc. Limited to <see cref="TileDrawing.TileCounterType"/>.
        /// </summary>
        public static void AddCustomSpecialPoint(int i, int j, Action<int, int, SpriteBatch> drawMethod)
        {
            customSpecialPoints.Add(new(i, j), drawMethod);
        }

        private void On_TileDrawing_PreDrawTiles(On_TileDrawing.orig_PreDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);

            if (!solidLayer && (intoRenderTargets || Lighting.UpdateEveryFrame))
                customSpecialPoints.Clear();
        }

        private void On_TileDrawing_PostDrawTiles(On_TileDrawing.orig_PostDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            if (!solidLayer && !intoRenderTargets)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                foreach(var (coords, drawMethod) in customSpecialPoints)
                    drawMethod.Invoke(coords.X, coords.Y, Main.spriteBatch);
                Main.spriteBatch.End();
            }

            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
        }

        private class TileExtraTextureRenderTargetHolder(Asset<Texture2D> extraTextureAsset) : TilePaintSystemV2.TileRenderTargetHolder
        {
            public override void Prepare()
            {
                extraTextureAsset.Wait?.Invoke();
                PrepareTextureIfNecessary(extraTextureAsset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, TreePaintSystemData.GetTileSettings(Key.TileType, Key.TileStyle));
            }
        }

        public static Texture2D GetOrPreparePaintedExtraTexture(Tile tile, Asset<Texture2D> asset)
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
                value = new TileExtraTextureRenderTargetHolder(asset) { Key = key };
                tileExtraTextureRenders.TryAdd(key, value);

                if (!value.IsReady)
                    AddTilePaintRequest(value);
            }

            return texture;
        }

        public static void DrawTileExtraTexture(int i, int j, SpriteBatch spriteBatch, Asset<Texture2D> asset, bool applyPaint, Color? drawColor = null, Vector2 drawOffset = default)
        {
            Tile tile = Main.tile[i, j];
            if (!TileDrawing.IsVisible(tile))
                return;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Texture2D texture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, asset) : asset.Value;

            ushort type = tile.TileType;
            short tileFrameX = tile.TileFrameX;
            short tileFrameY = tile.TileFrameY;
            TileRenderer.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out int tileWidth, out int tileHeight, out int tileTop, out int halfBrickHeight, out int addFrX, out int addFrY, out _, out _, out _, out _);

            Color tileLight = drawColor ?? Lighting.GetColor(i, j);
            AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, false);
            tileLight = GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);

            Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
            Vector2 position = new Vector2(i * 16 + (int)drawOffset.X - (int)Main.screenPosition.X - (tileWidth - 16f) / 2f, j * 16 + (int)drawOffset.Y - (int)Main.screenPosition.Y + tileTop + halfBrickHeight) + zero;
            spriteBatch.Draw(texture, position, frame, tileLight, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void DrawMultiTileInWindBottomAnchor(int topLeftX, int topLeftY, Asset<Texture2D> textureOverride = null, Color? colorOverride = null, bool perTileLighting = true, Vector2 drawOffset = default, bool applyPaint = false, float windSensitivity = 0.15f, int rowsToIgnore = 0)
        {
            Tile sourceTile = Main.tile[topLeftX, topLeftY];
            TileObjectData data = TileObjectData.GetTileData(sourceTile);
            if (data is null)
                return;

            int sizeX = data.Width;
            int sizeY = data.Height;

            Vector2 screenPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = Vector2.Zero;
            float windCycle = TileRenderer.GetWindCycle(topLeftX, topLeftY, SunflowerWindCounter);

            Vector2 position = new Vector2(topLeftX * 16 - (int)screenPosition.X + sizeX * 16f * 0.5f, topLeftY * 16 - (int)screenPosition.Y + 16 * sizeY) + zero;
            bool wind = WorldGen.InAPlaceWithWind(topLeftX, topLeftY, sizeX, sizeY - rowsToIgnore);

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
                    float heightModifier = 1f - (j - topLeftY + 1) / (float)sizeY;

                    if (heightModifier == 0f)
                        heightModifier = 0.1f;

                    if (!wind)
                        heightModifier = 0f;

                    if (j >= topLeftY + sizeY - rowsToIgnore)
                        heightModifier = 0f;

                    TileRenderer.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out _, out _, out _);
                    
                    bool canDoDust = TileRendererRandom.NextBool(4);
                    Color tileLight = colorOverride ?? (perTileLighting ? Lighting.GetColor(i, j) : Lighting.GetColor(topLeftX, topLeftY));
                    AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, canDoDust);
                    tileLight = GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);

                    Vector2 tilePos = new Vector2(i * 16 - (int)screenPosition.X, j * 16 - (int)screenPosition.Y + tileTop) + zero;
                    Vector2 windMod = new(windCycle * 1f, Math.Abs(windCycle) * 2f * heightModifier);
                    Vector2 origin = position - tilePos;
                    Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                    Vector2 drawPos = position + new Vector2(0f, windMod.Y) + drawOffset;
                    float rotation = windCycle * windSensitivity * heightModifier;

                    Texture2D tileDrawTexture;
                    if (textureOverride != null)
                        tileDrawTexture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, textureOverride) : textureOverride.Value;
                    else
                        tileDrawTexture = TileRenderer.GetTileDrawTexture(tile, i, j);

                    if (tileDrawTexture != null)
                        Main.spriteBatch.Draw(tileDrawTexture, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
                }
            }
        }

        /*
        Chandeliers
		    windHeightSensitivityOverride = 1f
		    windOffsetFactorY = 0f
		        Flesh
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -1f
			        firstRowSolid = true
			        windRotationFactor *= 0.3f
		        Frozen
			        windRotationFactor *= 0.5f
		        Rich Mahogany, Living Wood, Bone
        	        windHeightSensitivityOverride = null
			        windOffsetFactorY = -1f
		        Palm Wood
			        windHeightSensitivityOverride = 0f
		        Mushroom
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -1f
			        firstRowSolid = true
		        Obsidian, Martian
			        windRotationFactor *= 0.5f
		        Granite
			        windHeightSensitivityOverride = 0f
		        Marble
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -1f
			        firstRowSolid = true
		        Crystal
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -1f
			        firstRowSolid = true
			        windRotationFactor *= 0.5f
		        Lesion
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -1f
			        firstRowSolid = true
                Solar, Vortex, Nebula, Stardust
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -2f
			        firstRowSolid = true;
			        windRotationFactor *= 0.5f
		        Sandstone
			        windHeightSensitivityOverride = null
			        windOffsetFactorY = -3f
		Lanterns
			windHeightSensitivityOverride = 1f
			windOffsetFactorY = 0f
				Chain, Meteorite, Flesh, Steampunk, Mushroom, Meteorite, Spider
					windHeightSensitivityOverride = null
					windOffsetFactorY = -1f
				Heart, Slime, Obsidian, Martian, Granite
					windHeightSensitivityOverride = 0f
				Lesion
					windHeightSensitivityOverride = null
					windOffsetFactorY = -1f
					firstRowSolid = true
				Solar, Vortex, Nebula, Stardust
					windHeightSensitivityOverride = null
					windOffsetFactorY = -1f
					firstRowSolid = true

		ChineseLanterns, FireflyinaBottle, LightningBuginaBottle, BeeHive, Pigronata, SoulBottles, LavaflyinaBottle, ShimmerflyinaBottle, DiscoGlobe
			windHeightSensitivityOverride = 1f
			windOffsetFactorY = 0f

		PotsSuspended, BrazierSuspended
			windOffsetFactorY = -2f

        */

        public static void DrawMultiTileInWindTopAnchor(int topLeftX, int topLeftY, Asset<Texture2D> texture = null, Color? color = null, bool perTileLighting = true, Vector2 offset = default, bool applyPaint = false, float windSensitivity = 0.15f, float windOffsetFactorY = -4f, int rowsToIgnore = 0,  float? windHeightSensitivityOverride = null)
        {
            Tile sourceTile = Main.tile[topLeftX, topLeftY];
            TileObjectData data = TileObjectData.GetTileData(sourceTile);
            if (data is null)
                return;

            int sizeX = data.Width;
            int sizeY = data.Height;

            Vector2 screenPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = Vector2.Zero;
            float windCycle = WorldGen.InAPlaceWithWind(topLeftX, topLeftY, sizeX, sizeY) ? TileRenderer.GetWindCycle(topLeftX, topLeftY, SunflowerWindCounter) : 0;
            float highestWindGridPush = GetHighestWindGridPushComplex(topLeftX, topLeftY, sizeX, sizeY, totalPushTime: 60, pushForcePerFrame: 1.26f, loops: 3, swapLoopDir: true);
            windCycle += highestWindGridPush;

            Vector2 position = new Vector2((float)(topLeftX * 16 - (int)screenPosition.X) + (float)sizeX * 16f * 0.5f, topLeftY * 16 - (int)screenPosition.Y) + zero;
            Vector2 verticalDrawOffset = new(0f, -2f);
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

            if (rowsToIgnore > 0)
                position += new Vector2(0f, rowsToIgnore * 16f);

            windSensitivity *= -1f;


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

                    float windHeightSensitivity = (j - topLeftY + 1) / (float)sizeY;
                    if (windHeightSensitivity == 0f)
                        windHeightSensitivity = 0.1f;

                    if (windHeightSensitivityOverride.HasValue)
                        windHeightSensitivity = windHeightSensitivityOverride.Value;

                    if (j < topLeftY + rowsToIgnore)
                        windHeightSensitivity = 0f;

                    TileRenderer.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out var _, out var _, out var _);
                    
                    bool canDoDust = TileRendererRandom.NextBool(4);
                    Color tileLight = color ?? (perTileLighting ? Lighting.GetColor(i, j) : Lighting.GetColor(topLeftX, topLeftY));
                    AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, canDoDust);
                    tileLight = GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);

                    Vector2 tilePos = new Vector2(i * 16 - (int)screenPosition.X, j * 16 - (int)screenPosition.Y + tileTop) + zero;
                    tilePos += verticalDrawOffset;
                    Vector2 origin = position - tilePos;
                    Vector2 drawPos = position + new Vector2(0f, Math.Abs(windCycle) * windOffsetFactorY * windHeightSensitivity) + offset;
                    Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                    float rotation = windCycle * windSensitivity * windHeightSensitivity;

                    Texture2D tileDrawTexture;
                    if (texture != null)
                        tileDrawTexture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, texture) : texture.Value;
                    else
                        tileDrawTexture = TileRenderer.GetTileDrawTexture(tile, i, j);

                    if (tileDrawTexture != null)
                        Main.spriteBatch.Draw(tileDrawTexture, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);

                }
            }
        }

        public static void AddTilePaintRequest(TilePaintSystemV2.ARenderTargetHolder renderTargetHolder)
        {
            ((List<TilePaintSystemV2.ARenderTargetHolder>)tilePaintSystemV2_requests_fieldInfo.GetValue(Main.instance.TilePaintSystem)).Add(renderTargetHolder);
        }

        public static void AdjustForVisionChangers(int i, int j, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, ref Color tileLight, bool canDoDust)
        {
            tileDrawing_DrawAnimatedTile_AdjustForVisionChangers_methodInfo.Invoke(TileRenderer, [i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight, canDoDust]);
        }

        public static Color GetLightOverride(int j, int i, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight)
        {
            return (Color)tileDrawing_DrawTiles_GetLightOverride_methodInfo.Invoke(TileRenderer, [i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight]);
        }

        public static float GetHighestWindGridPushComplex(int topLeftX, int topLeftY, int sizeX, int sizeY, int totalPushTime, float pushForcePerFrame, int loops, bool swapLoopDir)
        {
            return (float)tileDrawing_GetHighestWindGridPushComplex_methodInfo.Invoke(TileRenderer, [topLeftX, topLeftY, sizeX, sizeY, totalPushTime, pushForcePerFrame, loops, swapLoopDir]);
        }

        public static Point[] GetVanillaSpecialPoints(TileDrawing.TileCounterType type)
        {
            Point[][] specialPositions = (Point[][])tileDrawing_specialPositions_fieldInfo.GetValue(TileRenderer);

            if (specialPositions == null || type >= TileDrawing.TileCounterType.Count)
                return Array.Empty<Point>();

            return specialPositions[(int)type] ?? Array.Empty<Point>();
        }

        public static Point[] GetVanillaSpecialLegacyPoints()
        {
            int[] specialTileX = (int[])tileDrawing_specialTileX_fieldInfo.GetValue(TileRenderer);
            int[] specialTileY = (int[])tileDrawing_specialTileY_fieldInfo.GetValue(TileRenderer);

            if (specialTileX == null || specialTileY == null)
                return Array.Empty<Point>();

            Point[] points = new Point[specialTileX.Length];
            for (int i = 0; i < specialTileX.Length; i++)
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
        public static bool IsVanillaSpecialPoint(int i, int j, TileDrawing.TileCounterType type) => GetVanillaSpecialPoints(type).Any(p => p.X == i && p.Y == j);

        /// <summary>
        /// Checks if the given (i, j) coordinate is a special legacy point.
        /// </summary>
        /// <param name="i">The X coordinate of the tile.</param>
        /// <param name="j">The Y coordinate of the tile.</param>
        /// <returns>True if the point is a special legacy point, otherwise false.</returns>
        public static bool IsVanillaSpecialLegacyPoint(int i, int j) => GetVanillaSpecialLegacyPoints().Any(p => p.X == i && p.Y == j);

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
