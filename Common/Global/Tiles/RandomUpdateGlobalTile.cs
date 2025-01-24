using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Tiles.Trees;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Tiles
{
    public class RandomUpdateGlobalTile : GlobalTile
    {
        public override void Load()
        {
            On_WorldGen.PlaceAlch += On_WorldGen_PlaceAlch;
        }

        public override void Unload()
        {
            On_WorldGen.PlaceAlch -= On_WorldGen_PlaceAlch;
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            if(type == TileID.JungleGrass)
            {
                bool aboveGround = j < Main.worldSurface + 10;
                if (aboveGround)
                {
                    Tile tileAbove = Main.tile[i, j - 1];
                    if (WorldGen.genRand.NextBool(700) && (!tileAbove.HasTile || (tileAbove.TileType is TileID.JunglePlants or TileID.JunglePlants2 or TileID.JungleThorns)))
                        WorldGen.TryGrowingTreeByType(ModContent.TileType<RubberTree>(), i, j);
                }
            }
        }

        private static bool CanPlantGrow(int x, int y)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if (!RoomOxygenSystem.IsRoomPressurized(x, y))
                    return false;
            }
            else
            {
                SceneData sceneData = new(new Point(x, y));
                if (sceneData.Macrocosm.ZonePollution)
                    return false;
            }

            return true;
        }

        private bool On_WorldGen_PlaceAlch(On_WorldGen.orig_PlaceAlch orig, int x, int y, int style)
        {
            if(!CanPlantGrow(x, y))
                return false;

            return orig(x, y, style);
        }

        private static MethodInfo worldGen_UpdateWorld_OvergroundTile;
        private static MethodInfo worldGen_UpdateWorld_UndergroundTile;

        public static void RandomTileUpdate()
        {
            MacrocosmSubworld current = MacrocosmSubworld.Current;
            if (current is null)
                return;

            double worldUpdateRate = WorldGen.GetWorldUpdateRate();
            if (worldUpdateRate == 0)
                return;

            double overgroundUpdateRate = 3E-05f * (float)worldUpdateRate;
            double overgroundTileUpdateNumber = Main.maxTilesX * Main.maxTilesY * overgroundUpdateRate;
            int plantUpdateChance = (int)Terraria.Utils.Lerp(151, 151 * 2.8, Terraria.Utils.Clamp(Main.maxTilesX / 4200.0 - 1.0, 0.0, 1.0));

            for (int k = 0; k < overgroundTileUpdateNumber; k++)
            {
                if (Main.rand.NextBool(plantUpdateChance * 100))
                    WorldGen.PlantAlch();

                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);

                WorldGen_UpdateWorld_OvergroundTile(i, j, checkNPCSpawns: false, wallDist: 3);
                TownNPCSystem.TrySpawningTownNPC(i, j); // TODO: maybe just let the vanilla method above take care of it
            }

            double undergroundUpdateRate = 1.5E-05f * (float)worldUpdateRate;
            double undergroundTileUpdateNumber = Main.maxTilesX * Main.maxTilesY * undergroundUpdateRate;
            for (int k = 0; k < undergroundTileUpdateNumber; k++)
            {
                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);

                WorldGen_UpdateWorld_UndergroundTile(i, j, checkNPCSpawns: false, wallDist: 3);
                TownNPCSystem.TrySpawningTownNPC(i, j); // TODO: maybe just let the vanilla method above take care of it
            }

            double liquidRandomUpdateRate = 10E-03f * (float)worldUpdateRate;
            double liquidRandomUpdateNumber = Main.maxTilesX * Main.maxTilesY * liquidRandomUpdateRate;

            for (int k = 0; k < liquidRandomUpdateNumber; k++)
            {
                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j = WorldGen.genRand.Next(10, Main.maxTilesY - 20);
                if (Main.tile[i, j].LiquidAmount > 0)
                    if (!RoomOxygenSystem.IsRoomPressurized(i, j))
                        WorldGen.PlaceLiquid(i, j, (byte)Main.tile[i, j].LiquidType, 0);
            }
        }

        public static void WorldGen_UpdateWorld_OvergroundTile(int i, int j, bool checkNPCSpawns, int wallDist)
        {
            worldGen_UpdateWorld_OvergroundTile ??= typeof(WorldGen).GetMethod("UpdateWorld_OvergroundTile", BindingFlags.NonPublic | BindingFlags.Static);
            worldGen_UpdateWorld_OvergroundTile.Invoke(null, [i, j, checkNPCSpawns, wallDist]);
        }

        public static void WorldGen_UpdateWorld_UndergroundTile(int i, int j, bool checkNPCSpawns, int wallDist)
        {
            worldGen_UpdateWorld_UndergroundTile ??= typeof(WorldGen).GetMethod("UpdateWorld_UndergroundTile", BindingFlags.NonPublic | BindingFlags.Static);
            worldGen_UpdateWorld_UndergroundTile.Invoke(null, [i, j, checkNPCSpawns, wallDist]);
        }
    }
}
