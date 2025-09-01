using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary> Convenience method for getting lighting color using an npc or projectile position.</summary>
        public static Color GetLightColor(Vector2 position) => Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));

        /// <summary> Convenience method for adding lighting using an npc or projectile position, using a Color instance for color. </summary>
        public static void AddLight(Vector2 position, Color color, float brightnessDivider = 1F)
            => AddLight(position, color.R / 255F, color.G / 255F, color.B / 255F, brightnessDivider);

        /// <summary> Convenience method for adding lighting using an npc or projectile position with 0f - 1f color values. </summary>
        public static void AddLight(Vector2 position, float colorR, float colorG, float colorB, float brightnessDivider = 1f)
            => Lighting.AddLight((int)(position.X / 16f), (int)(position.Y / 16f), colorR / brightnessDivider, colorG / brightnessDivider, colorB / brightnessDivider);

        public static void ApplySurfaceLight(Tile tile, int x, int y, ref Vector3 lightColor)
        {
            float R = 0f;
            float G = 0f;
            float B = 0f;
            float tileR = (float)(int)Main.tileColor.R / 255f;
            float tileG = (float)(int)Main.tileColor.G / 255f;
            float tileB = (float)(int)Main.tileColor.B / 255f;
            float brightness = (tileR + tileG + tileB) / 3f;
            if (tile.HasTile && TileID.Sets.AllowLightInWater[tile.TileType])
            {
                if (lightColor.X < brightness && (Main.wallLight[tile.WallType] || tile.WallType == 73 || tile.WallType == 227 || (tile.IsWallInvisible && !Main.ShouldShowInvisibleWalls())))
                {
                    R = tileR;
                    G = tileG;
                    B = tileB;
                }
            }
            else if ((!tile.HasTile || !Main.tileNoSunLight[tile.TileType] || ((tile.Slope != 0 || tile.IsHalfBlock || (tile.IsTileInvisible && !Main.ShouldShowInvisibleWalls())) && Main.tile[x, y - 1].LiquidAmount == 0 && Main.tile[x, y + 1].LiquidAmount == 0 && Main.tile[x - 1, y].LiquidAmount == 0 && Main.tile[x + 1, y].LiquidAmount == 0)) && lightColor.X < brightness && (Main.wallLight[tile.WallType] || tile.WallType == 73 || tile.WallType == 227 || (tile.IsWallInvisible && !Main.ShouldShowInvisibleWalls())))
            {
                if (tile.LiquidAmount < 200)
                {
                    if (!tile.IsHalfBlock || Main.tile[x, y - 1].LiquidAmount < 200)
                    {
                        R = tileR;
                        G = tileG;
                        B = tileB;
                    }
                }
                else if (Main.liquidAlpha[13] > 0f)
                {
                    Main.rand ??= new UnifiedRandom();
                    B = tileB * 0.175f * (1f + Main.rand.NextFloat() * 0.13f) * Main.liquidAlpha[13];
                }
            }

            if ((!tile.HasTile || tile.IsHalfBlock || !Main.tileNoSunLight[tile.TileType]) && ((tile.WallType >= 88 && tile.WallType <= 93) || tile.WallType == 241) && tile.LiquidAmount < byte.MaxValue)
            {
                R = tileR;
                G = tileG;
                B = tileB;
                int wallLight = tile.WallType - 88;
                if (tile.WallType == 241)
                    wallLight = 6;

                switch (wallLight)
                {
                    case 0:
                        R *= 0.9f;
                        G *= 0.15f;
                        B *= 0.9f;
                        break;
                    case 1:
                        R *= 0.9f;
                        G *= 0.9f;
                        B *= 0.15f;
                        break;
                    case 2:
                        R *= 0.15f;
                        G *= 0.15f;
                        B *= 0.9f;
                        break;
                    case 3:
                        R *= 0.15f;
                        G *= 0.9f;
                        B *= 0.15f;
                        break;
                    case 4:
                        R *= 0.9f;
                        G *= 0.15f;
                        B *= 0.15f;
                        break;
                    case 5:
                        R *= 0.5f + (float)Main.DiscoR / 255f * 0.2f;
                        G *= 0.5f + (float)Main.DiscoG / 255f * 0.2f;
                        B *= 0.5f + (float)Main.DiscoB / 255f * 0.2f;
                        break;
                    case 6:
                        R *= 0.9f;
                        G *= 0.5f;
                        B *= 0f;
                        break;
                }
            }

            float shimmerBrightness = 1f - Main.shimmerDarken;
            R *= shimmerBrightness;
            G *= shimmerBrightness;
            B *= shimmerBrightness;
            if (lightColor.X < R)
                lightColor.X = R;

            if (lightColor.Y < G)
                lightColor.Y = G;

            if (lightColor.Z < B)
                lightColor.Z = B;
        }

    }
}
