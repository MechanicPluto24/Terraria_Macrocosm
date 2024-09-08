using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    /// <summary>
    /// <br/> <b>Tex2Gen, a.k.a. TexGen 2.0, is a reimplementation of the TexGen system from BaseMod for tModLoader 1.3, by GroxTheGreat.</b>
    /// <br/> Reimplementation by Feldy.
    /// <br/> The reimplementation presents these additional features:  
    /// <list type="bullet">
    /// <item> Raw texture loading, for better performance, dedServ generation and avoiding the need to schedule loading and gen on main thread.  </item> 
    /// </list>
    /// Planned features (TODO):
    /// <list type="bullet">
    /// <item> Saving structures built in-game to texture data (colors could be a mix of map colors and type/name hashing) </item>
    /// <item> The possibility to provide a map of objects (multitiles) from a json (easier to manage than an "object" texture) </item>
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

        public static Dictionary<Color, int> colorToLiquid;
        public static Dictionary<Color, BlockType> colorToSlope;
        public static Dictionary<int, Color> liquidToColor;
        public static Dictionary<BlockType, Color> slopeToColor;

        private int width, height;
        private TileInfo[,] tileGen;
        private int torchStyle = 0, platformStyle = 0;

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
                    {
                        Utility.GenerateTile(x2, y2, info.tileType, info.wallType, info.tileStyle != 0 ? info.tileStyle : info.tileType == TileID.Torches ? torchStyle : info.tileType == TileID.Platforms ? platformStyle : 0, info.tileType > -1, info.liquidAmount == 0, info.blockType, silent: silent, sync: sync);
                    }
                    if (info.liquidType != -1)
                    {
                        Utility.GenerateLiquid(x2, y2, info.liquidType, false, info.liquidAmount, sync);
                    }
                    if (info.objectID != 0)
                    {
                        WorldGen.PlaceObject(x2, y2, info.objectID);
                        NetMessage.SendObjectPlacement(-1, x2, y2, info.objectID, 0, 0, -1, -1);
                    }
                }
            }
        }

        /// <summary> Reads texture data for world generation. Alse works on dedicated servers. </summary>
        /// <param name="path"> Path to the texture. </param>
        /// <param name="useAsset"> Whether to load the Texture2D data from the asset repository, or directly from IO. Forced to false on dedicated servers. </param>
        /// <returns></returns>
        public static RawTexture GetRawTexture(string path, bool useAsset = false)
        {
            if (Main.dedServ)
                useAsset = false;

            if (useAsset)
            {
                var texture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
                return RawTexture.FromTexture2D(texture);
            }
            else
            {
                path = path.Replace(nameof(Macrocosm) + "/", "") + ".rawimg";
                return RawTexture.FromStream(Macrocosm.Instance.GetFileStream(path));
            }
        }

        /// <summary> 
        /// Creates a <see cref="Tex2Gen"/> from <see cref="RawTexture"/>s. 
        /// NOTE: all textures MUST be the same size or horrible things happen! 
        /// </summary>
        public static Tex2Gen GetGenerator(RawTexture tileTex, Dictionary<Color, int> colorToTile, RawTexture wallTex = null, Dictionary<Color, int> colorToWall = null, RawTexture liquidTex = null, RawTexture slopeTex = null, RawTexture objectTex = null, Dictionary<Color, int> colorToObject = null)
        {
            PopulateCommonColorDictionaries();

            int x = 0;
            int y = 0;

            Tex2Gen gen = new(tileTex.Width, tileTex.Height);

            for (int m = 0; m < tileTex.Length; m++)
            {
                Color tileColor = tileTex[m];
                Color wallColor = wallTex == null ? Color.Black : wallTex[m];
                Color liquidColor = liquidTex == null ? Color.Black : liquidTex[m];
                Color slopeColor = slopeTex == null ? Color.Black : slopeTex[m];
                Color objectColor = objectTex == null ? Color.Black : objectTex[m];

                int tileType = colorToTile.TryGetValue(tileColor, out int tileID) ? tileID : -1; // If no key assume no action
                int wallType = colorToWall != null && colorToWall.TryGetValue(wallColor, out int wallID) ? wallID : -1;
                int liquidType = colorToLiquid != null && colorToLiquid.TryGetValue(liquidColor, out int liquidID) ? liquidID : -1;
                BlockType blockType = colorToSlope != null && colorToSlope.TryGetValue(slopeColor, out BlockType blockSlope) ? blockSlope : BlockType.Solid;
                int objectIType = colorToObject != null && colorToObject.TryGetValue(objectColor, out int objectID) ? objectID : 0;

                gen.tileGen[x, y] = new TileInfo(tileType, 0, wallType, liquidType, liquidType == -1 ? 0 : 255, blockType, objectIType);
                x++;

                if (x >= tileTex.Width)
                {
                    x = 0;
                    y++;
                }

                // You've somehow reached the end of the texture! (this shouldn't happen!)
                if (y >= tileTex.Height)
                {
                    break;
                }
            }
            return gen;
        }

        /// <summary> 
        /// Creates a <see cref="Tex2Gen"/> directly from <see cref="Texture2D"/>s. 
        /// Old implementation, does not work on dedicated servers, prefer to use <see cref="GetGenerator(RawTexture, Dictionary{Color, int}, RawTexture, Dictionary{Color, int}, RawTexture, RawTexture, RawTexture, Dictionary{Color, int})"> GetTex2Generator(Tex2GenData...) </see> instead 
        /// NOTE: all textures MUST be the same size or horrible things happen! 
        /// </summary>
        public static Tex2Gen GetGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile, Texture2D wallTex = null, Dictionary<Color, int> colorToWall = null, Texture2D liquidTex = null, Texture2D slopeTex = null, Texture2D objectTex = null, Dictionary<Color, int> colorToObject = null)
        {
            PopulateCommonColorDictionaries();

            Color[] tileData = new Color[tileTex.Width * tileTex.Height];
            tileTex.GetData(0, tileTex.Bounds, tileData, 0, tileTex.Width * tileTex.Height);

            Color[] wallData = wallTex != null ? new Color[wallTex.Width * wallTex.Height] : null;
            if (wallData != null)
                wallTex.GetData(0, wallTex.Bounds, wallData, 0, wallTex.Width * wallTex.Height);

            Color[] liquidData = liquidTex != null ? new Color[liquidTex.Width * liquidTex.Height] : null;
            if (liquidData != null)
                liquidTex.GetData(0, liquidTex.Bounds, liquidData, 0, liquidTex.Width * liquidTex.Height);

            Color[] slopeData = slopeTex != null ? new Color[slopeTex.Width * slopeTex.Height] : null;
            if (slopeData != null)
                slopeTex.GetData(0, slopeTex.Bounds, slopeData, 0, slopeTex.Width * slopeTex.Height);

            Color[] objectData = objectTex != null ? new Color[objectTex.Width * objectTex.Height] : null;
            if (objectData != null)
                objectTex.GetData(0, objectTex.Bounds, objectData, 0, objectTex.Width * objectTex.Height);

            int x = 0;
            int y = 0;

            Tex2Gen gen = new(tileTex.Width, tileTex.Height);

            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m];
                Color wallColor = wallTex == null ? Color.Black : wallData[m];
                Color liquidColor = liquidTex == null ? Color.Black : liquidData[m];
                Color slopeColor = slopeTex == null ? Color.Black : slopeData[m];
                Color objectColor = objectTex == null ? Color.Black : objectData[m];

                int tileType = colorToTile.TryGetValue(tileColor, out int tileID) ? tileID : -1; // If no key assume no action
                int wallType = colorToWall != null && colorToWall.TryGetValue(wallColor, out int wallID) ? wallID : -1;
                int liquidType = colorToLiquid != null && colorToLiquid.TryGetValue(liquidColor, out int liquidID) ? liquidID : -1;
                BlockType blockType = colorToSlope != null && colorToSlope.TryGetValue(slopeColor, out BlockType blockSlope) ? blockSlope : BlockType.Solid;
                int objectIType = colorToObject != null && colorToObject.TryGetValue(objectColor, out int objectID) ? objectID : 0;

                gen.tileGen[x, y] = new TileInfo(tileType, 0, wallType, liquidType, liquidType == -1 ? 0 : 255, blockType, objectIType);
                x++;

                if (x >= tileTex.Width)
                {
                    x = 0;
                    y++;
                }

                // You've somehow reached the end of the texture! (this shouldn't happen!)
                if (y >= tileTex.Height)
                {
                    break;
                }
            }
            return gen;
        }

        private static void PopulateCommonColorDictionaries()
        {
            colorToLiquid ??= new Dictionary<Color, int>
            {
                [new Color(0, 0, 255)] = LiquidID.Water,
                [new Color(255, 0, 0)] = LiquidID.Lava,
                [new Color(255, 255, 0)] = LiquidID.Honey,
                [new Color(255, 0, 255)] = LiquidID.Shimmer
            };

            colorToSlope ??= new Dictionary<Color, BlockType>
            {
                [new Color(255, 0, 0)] = BlockType.SlopeUpRight, 
                [new Color(0, 255, 0)] = BlockType.SlopeUpLeft,      
                [new Color(0, 0, 255)] = BlockType.SlopeDownRight,       
                [new Color(255, 255, 0)] = BlockType.SlopeDownLeft,    
                [new Color(255, 255, 255)] = BlockType.HalfBlock,  
                [new Color(0, 0, 0, 0)] = BlockType.Solid  
            };

            liquidToColor ??= new Dictionary<int, Color>
            {
                [LiquidID.Water] = new Color(0, 0, 255),
                [LiquidID.Lava] = new Color(255, 0, 0),
                [LiquidID.Honey] = new Color(255, 255, 0),
                [LiquidID.Shimmer] = new Color(255, 0, 255)
            };

            slopeToColor ??= new Dictionary<BlockType, Color>
            {
                [BlockType.SlopeUpRight] = new Color(255, 0, 0),      
                [BlockType.SlopeUpLeft] = new Color(0, 255, 0),    
                [BlockType.SlopeDownRight] = new Color(0, 0, 255),     
                [BlockType.SlopeDownLeft] = new Color(255, 255, 0),   
                [BlockType.HalfBlock] = new Color(255, 255, 255),  
                [BlockType.Solid] = new Color(0, 0, 0, 0)        
            };
        }

        public static void ExportRegionToPng(int startX, int startY, int width, int height, string tilePath, string wallPath, string liquidPath, string slopePath)
        {
            PopulateCommonColorDictionaries();

            // Prepare color arrays for tiles, walls, liquids, and slopes
            Color[] tileData = new Color[width * height];
            Color[] wallData = new Color[width * height];
            Color[] liquidData = new Color[width * height];
            Color[] slopeData = new Color[width * height];

            // Loop through the selected region of the world and capture data
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int worldX = startX + x;
                    int worldY = startY + y;
                    // Get the color representation for each type
                    tileData[x + y * width] = GetColorForTile(worldX, worldY);
                    wallData[x + y * width] = GetColorForWall(worldX, worldY);
                    liquidData[x + y * width] = GetColorForLiquid(worldX, worldY);
                    slopeData[x + y * width] = GetColorForSlope(worldX, worldY);
                }
            }

            // Create and save textures for each type
            SaveTexture_PNG(tileData, width, height, tilePath);
            SaveTexture_PNG(wallData, width, height, wallPath);
            SaveTexture_PNG(liquidData, width, height, liquidPath);
            SaveTexture_PNG(slopeData, width, height, slopePath);
        }

        /// <summary>
        /// Converts a Tile into a Color representation for the tile layer.
        /// </summary>
        private static Color GetColorForTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile)
                return Color.Transparent;

            return Utility.GetTileColor(i, j);
        }

        /// <summary>
        /// Converts a Tile into a Color representation for the wall layer.
        /// </summary>
        private static Color GetColorForWall(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.WallType <= 0)
                return Color.Transparent;

            return Utility.GetWallColor(i, j);
        }

        /// <summary>
        /// Converts a Tile into a Color representation for the liquid layer.
        /// </summary>
        private static Color GetColorForLiquid(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.LiquidAmount < 255)
                return Color.Transparent;

            return liquidToColor[tile.LiquidType];
        }

        /// <summary>
        /// Converts a Tile into a Color representation for the slope layer.
        /// </summary>
        private static Color GetColorForSlope(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return slopeToColor[tile.BlockType];
        }

        /// <summary>
        /// Saves the color data as a PNG file.
        /// </summary>
        private static void SaveTexture_PNG(Color[] colorData, int width, int height, string filePath)
        {
            Texture2D texture = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            texture.SetData(colorData);
            using Stream stream = File.Create(filePath);
            texture.SaveAsPng(stream, width, height);
        }
    }
}
