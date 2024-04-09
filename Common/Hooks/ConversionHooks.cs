using Macrocosm.Content.Tiles.Blocks.Sands;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class ConversionHooks : ModSystem
    {
        public override void Load()
        {
            On_WorldGen.Convert += On_WorldGen_Convert;
        }

        public override void Unload()
        {
            On_WorldGen.Convert -= On_WorldGen_Convert;
        }

        private void On_WorldGen_Convert(On_WorldGen.orig_Convert orig, int i, int j, int conversionType, int size)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (!WorldGen.InWorld(k, l, 1) || Math.Abs(k - i) + Math.Abs(l - j) >= 6)
                        orig(i, j, conversionType, size);

                    Tile tile = Main.tile[k, l];
                    int tileType = tile.TileType;
                    int wallType = tile.WallType;

                    if (tileType == ModContent.TileType<SilicaSand>() ||
                       tileType == ModContent.TileType<SilicaEbonsand>() ||
                       tileType == ModContent.TileType<SilicaCrimsand>() ||
                       tileType == ModContent.TileType<SilicaPearlsand>())
                    {
                        switch (conversionType)
                        {
                            case BiomeConversionID.Purity:
                                tile.TileType = (ushort)ModContent.TileType<SilicaSand>();
                                break;

                            case BiomeConversionID.Corruption:
                                tile.TileType = (ushort)ModContent.TileType<SilicaEbonsand>();
                                break;

                            case BiomeConversionID.Crimson:
                                tile.TileType = (ushort)ModContent.TileType<SilicaCrimsand>();
                                break;

                            case BiomeConversionID.Hallow:
                                tile.TileType = (ushort)ModContent.TileType<SilicaPearlsand>();
                                break;
                        }

                        WorldGen.SquareWallFrame(k, l);
                        NetMessage.SendTileSquare(-1, k, l);
                    }
                }
            }

            orig(i, j, conversionType, size);
        }
    }
}
