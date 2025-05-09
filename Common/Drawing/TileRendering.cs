using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing
{
    public class TileRendering : ILoadable
    {
        public void Load(Mod mod)
        {
        }

        public void Unload()
        {
        }

        public static TileDrawing TileRenderer => Main.instance.TilesRenderer;

        public static double TreeWindCounter => typeof(TileDrawing).GetFieldValue<double>("_treeWindCounter", TileRenderer);
        public static double GrassWindCounter => typeof(TileDrawing).GetFieldValue<double>("_grassWindCounter", TileRenderer);
        public static double SunflowerWindCounter => typeof(TileDrawing).GetFieldValue<double>("_sunflowerWindCounter", TileRenderer);
        public static double VineWindCounter => typeof(TileDrawing).GetFieldValue<double>("_vineWindCounter", TileRenderer);
        public static int TreeLeafFrequency => typeof(TileDrawing).GetFieldValue<int>("_leafFrequency", TileRenderer);
        public static UnifiedRandom TileRendererRandom => typeof(TileDrawing).GetFieldValue<UnifiedRandom>("_rand", TileRenderer);

        /// <summary> Collection of painted tile extra textures for reuse </summary>
        private static readonly Dictionary<TilePaintSystemV2.TileVariationkey, TileExtraTextureRenderTargetHolder> tileExtraTextureRenders = new();

        /// <summary> Tile painted RT holder class for extra textures </summary>
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

        /// <summary> Paint a tile's extra texture using its paint color, or retrieve it if ready </summary>
        public static Texture2D GetOrPreparePaintedExtraTexture(Tile tile, Asset<Texture2D> asset)
        {
            if (!TileDrawing.IsVisible(tile))
                return Macrocosm.EmptyTex.Value;

            Texture2D texture = asset.Value;
            TilePaintSystemV2.TileVariationkey key = new()
            {
                TileType = tile.TileType,
                TileStyle = asset.Name.GetHashCode(), // Ensures each extra texture is uniquely identified
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

        public static void DrawMultiTileGrass(int topLeftX, int topLeftY, float totalWindMultiplier = 0.15f, int rowsToIgnore = 0, Asset<Texture2D> customTexture = null, Color? drawColorOverride = null, bool perTileLighting = true, Vector2 drawOffset = default, bool applyPaint = false)
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
                    Color tileLight = drawColorOverride ?? (perTileLighting ? Lighting.GetColor(i, j) : Lighting.GetColor(topLeftX, topLeftY));
                    AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, canDoDust);
                    tileLight = GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);

                    Vector2 tilePos = new Vector2(i * 16 - (int)screenPosition.X, j * 16 - (int)screenPosition.Y + tileTop) + zero;
                    Vector2 windMod = new(windCycle * 1f, Math.Abs(windCycle) * 2f * heightModifier);
                    Vector2 origin = position - tilePos;
                    Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                    Vector2 drawPos = position + new Vector2(0f, windMod.Y) + drawOffset;
                    float rotation = windCycle * totalWindMultiplier * heightModifier;

                    Texture2D tileDrawTexture;
                    if (customTexture != null)
                        tileDrawTexture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, customTexture) : customTexture.Value;
                    else
                        tileDrawTexture = TileRenderer.GetTileDrawTexture(tile, i, j);

                    if (tileDrawTexture != null)
                        Main.spriteBatch.Draw(tileDrawTexture, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
                }
            }
        }

        /*
        Chandeliers
		    overrideWindCycle = 1f
		    windPushPowerY = 0f
		        Flesh
			        overrideWindCycle = null
			        windPushPowerY = -1f
			        dontRotateTopTiles = true
			        windRotationFactor *= 0.3f
		        Frozen
			        windRotationFactor *= 0.5f
		        Rich Mahogany, Living Wood, Bone
        	        overrideWindCycle = null
			        windPushPowerY = -1f
		        Palm Wood
			        overrideWindCycle = 0f
		        Mushroom
			        overrideWindCycle = null
			        windPushPowerY = -1f
			        dontRotateTopTiles = true
		        Obsidian, Martian
			        windRotationFactor *= 0.5f
		        Granite
			        overrideWindCycle = 0f
		        Marble
			        overrideWindCycle = null
			        windPushPowerY = -1f
			        dontRotateTopTiles = true
		        Crystal
			        overrideWindCycle = null
			        windPushPowerY = -1f
			        dontRotateTopTiles = true
			        windRotationFactor *= 0.5f
		        Lesion
			        overrideWindCycle = null
			        windPushPowerY = -1f
			        dontRotateTopTiles = true
                Solar, Vortex, Nebula, Stardust
			        overrideWindCycle = null
			        windPushPowerY = -2f
			        dontRotateTopTiles = true;
			        windRotationFactor *= 0.5f
		        Sandstone
			        overrideWindCycle = null
			        windPushPowerY = -3f
		Lanterns
			overrideWindCycle = 1f
			windPushPowerY = 0f
				Chain, Meteorite, Flesh, Steampunk, Mushroom, Meteorite, Spider
					overrideWindCycle = null
					windPushPowerY = -1f
				Heart, Slime, Obsidian, Martian, Granite
					overrideWindCycle = 0f
				Lesion
					overrideWindCycle = null
					windPushPowerY = -1f
					dontRotateTopTiles = true
				Solar, Vortex, Nebula, Stardust
					overrideWindCycle = null
					windPushPowerY = -1f
					dontRotateTopTiles = true

		ChineseLanterns, FireflyinaBottle, LightningBuginaBottle, BeeHive, Pigronata, SoulBottles, LavaflyinaBottle, ShimmerflyinaBottle, DiscoGlobe
			overrideWindCycle = 1f
			windPushPowerY = 0f

		PotsSuspended, BrazierSuspended
			windPushPowerY = -2f

        */
        public static void DrawMultiTileVine(int topLeftX, int topLeftY, float? overrideWindCycle = null, float windPushPowerY = -4f, int rowsToIgnore = 0, float totalWindMultiplier = 0.15f, Asset<Texture2D> customTexture = null, Color? drawColorOverride = null, bool perTileLighting = true, Vector2 drawOffset = default, bool applyPaint = false)
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

            totalWindMultiplier *= -1f;

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

                    if (overrideWindCycle.HasValue)
                        windHeightSensitivity = overrideWindCycle.Value;

                    if (j < topLeftY + rowsToIgnore)
                        windHeightSensitivity = 0f;

                    TileRenderer.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out var _, out var _, out var _);

                    bool canDoDust = TileRendererRandom.NextBool(4);
                    Color tileLight = drawColorOverride ?? (perTileLighting ? Lighting.GetColor(i, j) : Lighting.GetColor(topLeftX, topLeftY));
                    AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, canDoDust);
                    tileLight = GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);

                    Vector2 tilePos = new Vector2(i * 16 - (int)screenPosition.X, j * 16 - (int)screenPosition.Y + tileTop) + zero;
                    tilePos += verticalDrawOffset;
                    Vector2 origin = position - tilePos;
                    Vector2 drawPos = position + new Vector2(0f, Math.Abs(windCycle) * windPushPowerY * windHeightSensitivity) + drawOffset;
                    Rectangle frame = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                    float rotation = windCycle * totalWindMultiplier * windHeightSensitivity;

                    Texture2D tileDrawTexture;
                    if (customTexture != null)
                        tileDrawTexture = applyPaint ? GetOrPreparePaintedExtraTexture(tile, customTexture) : customTexture.Value;
                    else
                        tileDrawTexture = TileRenderer.GetTileDrawTexture(tile, i, j);

                    if (tileDrawTexture != null)
                        Main.spriteBatch.Draw(tileDrawTexture, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);

                }
            }
        }

        /// <summary> Submit a tile paint request </summary>
        public static void AddTilePaintRequest(TilePaintSystemV2.ARenderTargetHolder renderTargetHolder)
            => ((List<TilePaintSystemV2.ARenderTargetHolder>)typeof(TilePaintSystemV2).GetFieldValue("_requests", Main.instance.TilePaintSystem)).Add(renderTargetHolder);

        public static void AdjustForVisionChangers(int i, int j, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, ref Color tileLight, bool canDoDust)
            => typeof(TileDrawing).InvokeMethod("DrawAnimatedTile_AdjustForVisionChangers", TileRenderer, parameters: [i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight, canDoDust]);

        public static Color GetLightOverride(int j, int i, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight)
            => typeof(TileDrawing).InvokeMethod<Color>("DrawTiles_GetLightOverride", TileRenderer, parameters: [i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight]);

        public static float GetHighestWindGridPushComplex(int topLeftX, int topLeftY, int sizeX, int sizeY, int totalPushTime, float pushForcePerFrame, int loops, bool swapLoopDir)
            => typeof(TileDrawing).InvokeMethod<float>("GetHighestWindGridPushComplex", TileRenderer, parameters: [topLeftX, topLeftY, sizeX, sizeY, totalPushTime, pushForcePerFrame, loops, swapLoopDir]);

        public static Point[] GetVanillaSpecialPoints(TileDrawing.TileCounterType type)
        {
            Point[][] specialPositions = typeof(TileDrawing).GetFieldValue<Point[][]>("_specialPositions", TileRenderer);

            if (specialPositions == null || type >= TileDrawing.TileCounterType.Count)
                return Array.Empty<Point>();

            return specialPositions[(int)type] ?? Array.Empty<Point>();
        }

        public static Point[] GetVanillaSpecialLegacyPoints()
        {
            int[] specialTileX = typeof(TileDrawing).GetFieldValue<int[]>("_specialTileX", TileRenderer);
            int[] specialTileY = typeof(TileDrawing).GetFieldValue<int[]>("_specialTileY", TileRenderer);

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
