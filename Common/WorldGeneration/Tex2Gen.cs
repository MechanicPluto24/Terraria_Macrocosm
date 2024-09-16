using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    /// <summary>
    /// <br/> <b>Tex2Gen, a.k.a. TexGen 2.0, is a reimplementation of the TexGen system from BaseMod for tModLoader 1.3, by GroxTheGreat.</b>
    /// <br/> <br/> Reimplementation by Feldy.
    /// <br/> <br/> The reimplementation presents these additional features:  
    /// <list type="bullet">
    /// <item> Raw texture loading, for better performance, dedServ generation and avoiding the need to schedule loading on main thread.  </item> 
    /// <item> Saving structures built in-game to texture and json data</item>
    /// <item> The possibility to provide a map of objects (multitiles) from a json</item>
    /// </list>
    /// </summary>
    public class Tex2Gen
    {
        private record TileInfo(int tileType, int tileStyle, int wallType = -1, int liquidType = -1, int liquidAmount = 0, BlockType blockType = BlockType.Solid, int objectID = 0, int wireType = -1)
        {
            public int tileType = tileType;
            public int tileStyle = tileStyle;
            public int wallType = wallType;
            public int objectID = objectID;
            public int liquidType = liquidType;
            public int liquidAmount = liquidAmount;
            public BlockType blockType = blockType;
            public int wireType = wireType;
        }

        public static Dictionary<Color, int> ColorToLiquid => _colorToLiquid ??= new Dictionary<Color, int>
        {
            [new Color(0, 0, 255)] = LiquidID.Water,
            [new Color(255, 0, 0)] = LiquidID.Lava,
            [new Color(255, 255, 0)] = LiquidID.Honey,
            [new Color(255, 0, 255)] = LiquidID.Shimmer
        };

        public static Dictionary<Color, BlockType> ColorToSlope => _colorToSlope ??= new Dictionary<Color, BlockType>
        {
            [new Color(255, 0, 0)] = BlockType.SlopeUpRight,
            [new Color(0, 255, 0)] = BlockType.SlopeUpLeft,
            [new Color(0, 0, 255)] = BlockType.SlopeDownRight,
            [new Color(255, 255, 0)] = BlockType.SlopeDownLeft,
            [new Color(255, 255, 255)] = BlockType.HalfBlock,
            [new Color(0, 0, 0, 0)] = BlockType.Solid
        };

        public static Dictionary<int, Color> LiquidToColor => _liquidToColor ??= new Dictionary<int, Color>
        {
            [LiquidID.Water] = new Color(0, 0, 255),
            [LiquidID.Lava] = new Color(255, 0, 0),
            [LiquidID.Honey] = new Color(255, 255, 0),
            [LiquidID.Shimmer] = new Color(255, 0, 255)
        };

        public static Dictionary<BlockType, Color> SlopeToColor => _slopeToColor ??= new Dictionary<BlockType, Color>
        {
            [BlockType.SlopeUpRight] = new Color(255, 0, 0),
            [BlockType.SlopeUpLeft] = new Color(0, 255, 0),
            [BlockType.SlopeDownRight] = new Color(0, 0, 255),
            [BlockType.SlopeDownLeft] = new Color(255, 255, 0),
            [BlockType.HalfBlock] = new Color(255, 255, 255),
            [BlockType.Solid] = new Color(0, 0, 0, 0)
        };

        private static Dictionary<Color, int> _colorToLiquid;
        private static Dictionary<Color, BlockType> _colorToSlope;
        private static Dictionary<int, Color> _liquidToColor;
        private static Dictionary<BlockType, Color> _slopeToColor;

        private readonly int width, height;
        private readonly TileInfo[,] tileGen;
        public Tex2Gen(int w, int h)
        {
            width = w;
            height = h;
            tileGen = new TileInfo[width, height];
        }

        /// <summary>
        /// Generate the TexGen. (x,y) are top left coordinates of the generation. 
        /// </summary>
        /// <param name="silent"> Play sound on tile place </param>
        /// <param name="sync"> Sync placement </param>
        public void Generate(int x, int y, bool silent, bool sync, GenerationProgress progress = null)
        {
            for (int x1 = 0; x1 < width; x1++)
            {
                progress?.Set(x1 / (float)width);
                for (int y1 = 0; y1 < height; y1++)
                {
                    int x2 = x + x1;
                    int y2 = y + y1;
                    TileInfo info = tileGen[x1, y1];
                    if (info.tileType == -1 && info.wallType == -1 && info.liquidType == -1 && info.wireType == -1)
                        continue;

                    if (info.tileType != -1 || info.wallType > -1 || info.wireType > -1)
                        Utility.GenerateTile(x2, y2, info.tileType, info.wallType, info.tileStyle != 0 ? info.tileStyle : 0, info.tileType > -1, info.liquidAmount == 0, info.blockType, silent: silent, sync: sync);

                    if (info.liquidType != -1)
                        Utility.GenerateLiquid(x2, y2, info.liquidType, false, info.liquidAmount, sync);

                    if (info.objectID != 0)
                    {
                        WorldGen.PlaceObject(x2, y2, info.objectID);
                        NetMessage.SendObjectPlacement(-1, x2, y2, info.objectID, 0, 0, -1, -1);
                    }
                }
            }
        }

        public static Tex2Gen GetGenerator(
            string tileTexPath,
            string tileColorMapPath,
            string wallTexPath = null,
            string wallColorMapPath = null,
            string liquidTexPath = null,
            string slopeTexPath = null,
            string objectMapPath = null
        )
        {
            RawTexture tileTex = RawTexture.FromFile(tileTexPath);
            RawTexture wallTex = wallTexPath != null ? RawTexture.FromFile(wallTexPath) : null;
            RawTexture liquidTex = liquidTexPath != null ? RawTexture.FromFile(liquidTexPath) : null;
            RawTexture slopeTex = slopeTexPath != null ? RawTexture.FromFile(slopeTexPath) : null;

            var colorToTile = LoadColorMap_JSON(tileColorMapPath);
            var colorToWall = wallColorMapPath != null ? LoadColorMap_JSON(wallColorMapPath) : null;

            // Load object map from JSON file
            var objectMap = objectMapPath != null ? LoadObjectMap_JSON(objectMapPath) : null;

            int x = 0;
            int y = 0;

            Tex2Gen gen = new(tileTex.Width, tileTex.Height);

            for (int m = 0; m < tileTex.Length; m++)
            {
                Color tileColor = tileTex[m];
                Color wallColor = wallTex == null ? Color.Black : wallTex[m];
                Color liquidColor = liquidTex == null ? Color.Black : liquidTex[m];
                Color slopeColor = slopeTex == null ? Color.Black : slopeTex[m];

                int tileType = -1;
                if (colorToTile.TryGetValue(tileColor, out string tileTypeName))
                {
                    ModTile modTile = ModContent.Find<ModTile>(tileTypeName);
                    tileType = int.TryParse(tileTypeName, out int vanillaType) ? vanillaType : (modTile is not null ? modTile.Type : ModContent.TileType<UnloadedTile>());
                }

                int wallType = -1;
                if (colorToWall != null && colorToWall.TryGetValue(wallColor, out string wallTypeName))
                {
                    ModWall modWall = ModContent.Find<ModWall>(wallTypeName);
                    wallType = int.TryParse(wallTypeName, out int vanillaType) ? vanillaType : (modWall is not null ? modWall.Type : ModContent.WallType<UnloadedWall>());
                }

                int liquidType = ColorToLiquid.TryGetValue(liquidColor, out int liquidID) ? liquidID : -1;
                BlockType blockType = ColorToSlope.TryGetValue(slopeColor, out BlockType blockSlope) ? blockSlope : BlockType.Solid;

                int objectType = 0;
                if (objectMap != null && objectMap.TryGetValue(new Point(x, y), out string objectTypeName))
                {
                    ModTile modTile = ModContent.Find<ModTile>(tileTypeName);
                    objectType = int.TryParse(objectTypeName, out int vanillaType) ? vanillaType : (modTile is not null ? modTile.Type : ModContent.TileType<UnloadedTile>());
                }

                gen.tileGen[x, y] = new TileInfo(
                    tileType,
                    0,
                    wallType,
                    liquidType,
                    liquidType == -1 ? 0 : 255,
                    blockType,
                    objectType
                );

                x++;
                if (x >= tileTex.Width)
                {
                    x = 0;
                    y++;
                }

                if (y >= tileTex.Height)
                {
                    break;
                }
            }
            return gen;
        }


        private static Dictionary<Color, string> LoadColorMap_JSON(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var colorMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            var colorToTypeName = new Dictionary<Color, string>();
            foreach (var kvp in colorMap)
            {
                string typeName = kvp.Key;

                if (!Utility.TryGetColorFromHex(kvp.Value, out Color color))
                    throw new SerializationException($"Error: Invalid color code: {kvp.Value}.");

                colorToTypeName[color] = typeName;
            }

            return colorToTypeName;
        }

        private static Dictionary<Point, string> LoadObjectMap_JSON(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var objectMap = JsonConvert.DeserializeObject<Dictionary<int[], string>>(json);

            var positionToObjectType = new Dictionary<Point, string>();
            foreach (var kvp in objectMap)
            {
                int[] coords = kvp.Key;
                Point position = new(coords[0], coords[1]);
                string objectTypeName = kvp.Value;
                positionToObjectType[position] = objectTypeName;
            }

            return positionToObjectType;
        }
    }
}
