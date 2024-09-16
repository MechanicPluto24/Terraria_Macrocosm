using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Macrocosm.Common.WorldGeneration
{
    public class Tex2GenExporter
    {
        public static void ExportRegion(
            int startX,
            int startY,
            int width,
            int height,
            string tilePath,
            string wallPath,
            string liquidPath,
            string slopePath,
            string tileColorMapPath,
            string wallColorMapPath,
            string objectMapPath)
        {
            // Use strings for type names
            Dictionary<string, Color> tileTypeToColor = new();
            Dictionary<string, Color> wallTypeToColor = new();

            // Prepare color arrays for tiles, walls, liquids, and slopes
            Color[] tileData = new Color[width * height];
            Color[] wallData = new Color[width * height];
            Color[] liquidData = new Color[width * height];
            Color[] slopeData = new Color[width * height];
            Dictionary<Point, string> objectMap = new();

            // Loop through the selected region of the world and capture data
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int worldX = startX + x;
                    int worldY = startY + y;

                    tileData[x + y * width] = GetColorForTile(worldX, worldY, tileTypeToColor);
                    wallData[x + y * width] = GetColorForWall(worldX, worldY, wallTypeToColor);
                    liquidData[x + y * width] = GetColorForLiquid(worldX, worldY);
                    slopeData[x + y * width] = GetColorForSlope(worldX, worldY);
                    RecordObjects(worldX, worldY, objectMap, startX, startY);
                }
            }

            // Create and save textures for each type
            SaveTexture_PNG(tileData, width, height, tilePath);
            SaveTexture_PNG(wallData, width, height, wallPath);
            SaveTexture_PNG(liquidData, width, height, liquidPath);
            SaveTexture_PNG(slopeData, width, height, slopePath);

            SaveColorMap_JSON(tileTypeToColor, tileColorMapPath);
            SaveColorMap_JSON(wallTypeToColor, wallColorMapPath);
            SaveObjectMap_JSON(objectMap, objectMapPath);
        }

        private static Color GetColorForTile(int i, int j, Dictionary<string, Color> tileTypeToColor)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile)
                return Color.Transparent;

            ModTile modTile = TileLoader.GetTile(tile.TileType);
            string tileTypeName = modTile is null ? tile.TileType.ToString() : modTile.FullName;

            if (!tileTypeToColor.TryGetValue(tileTypeName, out Color color))
            {
                color = Utility.GetTileColor(i, j);
                color = GetUniqueColor(color, tileTypeToColor.Values);
                tileTypeToColor[tileTypeName] = color;
            }
            return color;
        }

        private static Color GetColorForWall(int i, int j, Dictionary<string, Color> wallTypeToColor)
        {
            Tile tile = Main.tile[i, j];
            if (tile.WallType == 0)
                return Color.Transparent;

            ModWall modWall = WallLoader.GetWall(tile.WallType);
            string wallTypeName = modWall is null ? tile.WallType.ToString() : modWall.FullName;

            if (!wallTypeToColor.TryGetValue(wallTypeName, out Color color))
            {
                color = Utility.GetWallColor(i, j);
                color = GetUniqueColor(color, wallTypeToColor.Values);
                wallTypeToColor[wallTypeName] = color;
            }

            return color;
        }

        private static Color GetColorForLiquid(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.LiquidAmount < 255)
                return Color.Transparent;

            return Tex2Gen.LiquidToColor[tile.LiquidType];
        }

        private static Color GetColorForSlope(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return Tex2Gen.SlopeToColor[tile.BlockType];
        }

        private static void RecordObjects(int i, int j, Dictionary<Point, string> objectPositions, int startX, int startY)
        {
            Tile tile = Main.tile[i, j];

            if (!tile.HasTile)
                return;

            TileObjectData data = TileObjectData.GetTileData(tile.TileType, TileObjectData.GetTileStyle(tile));
            if (data == null)
                return; 

            Point coords = Utility.GetMultitileTopLeft(i, j).ToPoint();

            // Adjust coordinates to be relative to the exported region
            int relativeX = coords.X - startX;
            int relativeY = coords.Y - startY;

            Point relativeCoords = new(relativeX, relativeY);

            if (!objectPositions.ContainsKey(relativeCoords))
            {
                ModTile modTile = TileLoader.GetTile(tile.TileType);
                string tileTypeName = modTile is null ? tile.TileType.ToString() : modTile.FullName;
                objectPositions[relativeCoords] = tileTypeName;
            }
        }

        private static Color GetUniqueColor(Color baseColor, IEnumerable<Color> existingColors)
        {
            Color color = baseColor;
            int increment = 1;

            while (existingColors.Contains(color))
            {
                int r = (color.R + increment) % 256;
                int g = (color.G + increment) % 256;
                int b = (color.B + increment) % 256;
                color = new Color(r, g, b);
                if (increment > 255)
                    throw new Exception("Unable to find a unique color after 255 increments.");
            }
            return color;
        }

        private static void SaveTexture_PNG(Color[] colorData, int width, int height, string filePath)
        {
            Texture2D texture = new(Main.graphics.GraphicsDevice, width, height);
            texture.SetData(colorData);
            using Stream stream = File.Create(filePath);
            texture.SaveAsPng(stream, width, height);
        }

        private static void SaveColorMap_JSON(Dictionary<string, Color> typeToColorMap, string filePath)
        {
            var colorMap = typeToColorMap.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetHex()
            );

            string json = JsonConvert.SerializeObject(colorMap, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private static void SaveObjectMap_JSON(Dictionary<Point, string> objectPositions, string filePath)
        {
            var objectMap = objectPositions.ToDictionary(
                kvp => new[] { kvp.Key.X, kvp.Key.Y },
                kvp => kvp.Value
            );

            string json = JsonConvert.SerializeObject(objectMap, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
