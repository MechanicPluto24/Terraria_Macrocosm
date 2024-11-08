using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;
//using static Terraria.WorldGen;

namespace Macrocosm.Content.WorldGeneration.DunGen
{
    public class MoonBaseBuilder
    {
        int dEntranceX;
        int numDRooms;
        int numDDoors;
        int numDungeonPlatforms;
        int vDungeonX;
        int vDungeonY;
        int dMinX;
        int dMaxX;
        int dMinY;
        int dMaxY;
        int dxStrength1;
        int dyStrength1;
        int dxStrength2;
        int dyStrength2;
        bool dungeonLake;
        bool dSurface;
        int[] dRoomSize;
        int[] dRoomX;
        int[] dRoomY;
        int[] dRoomL;
        int[] dRoomR;
        int[] dRoomT;
        int[] dRoomB;
        int[] dungeonPlatformX;
        int[] dungeonPlatformY;
        int[] DDoorX;
        int[] DDoorY;
        int[] DDoorPos;

        int[] tileTypes;
        int[] wallTypes;

        Vector2D lastDungeonHall;

        int dungeonX;
        int dungeonY;

        int lastMaxTilesX;
        int lastMaxTilesY;

        public MoonBaseBuilder(int dungeonX) 
        {
            this.dungeonX = dungeonX;
        }

        public void MakeDungeon(int x, int y)
        {
            tileTypes = [
                ModContent.TileType<IndustrialPlating>()
            ];

            wallTypes = [
                ModContent.WallType<IndustrialHazardWall>(),
                ModContent.WallType<IndustrialPlatingWall>(),
                ModContent.WallType<IndustrialSquarePaneledWall>(),
                ModContent.WallType<IndustrialTrimmingWall>(),
            ];

            dEntranceX = 0;
            numDRooms = 0;
            numDDoors = 0;
            numDungeonPlatforms = 0;

            dxStrength1 = genRand.Next(25, 30);
            dyStrength1 = genRand.Next(20, 25);
            dxStrength2 = genRand.Next(35, 50);
            dyStrength2 = genRand.Next(10, 15);

            int maxDRooms = 100;
            dRoomX = new int[maxDRooms];
            dRoomY = new int[maxDRooms];
            dRoomSize = new int[maxDRooms];
            //dRoomTreasure = new bool[maxDRooms];
            dRoomL = new int[maxDRooms];
            dRoomR = new int[maxDRooms];
            dRoomT = new int[maxDRooms];
            dRoomB = new int[maxDRooms];

            DDoorX = new int[500];
            DDoorY = new int[500];
            DDoorPos = new int[500];

            dungeonPlatformX = new int[500];
            dungeonPlatformY = new int[500];

            WorldGen.genRand.Next(3);
            ushort tileType;
            int wallType;

            tileType = (ushort)ModContent.TileType<IndustrialPlating>();
            wallType = ModContent.WallType<IndustrialPlatingWallNatural>();

            int[] roomWalls = new int[3];
            roomWalls[0] = ModContent.WallType<IndustrialTrimmingWall>();
            roomWalls[1] = ModContent.WallType<IndustrialHazardWallNatural>();
            roomWalls[2] = ModContent.WallType<IndustrialSquarePaneledWall>();

            int crackedType = -1;
            dungeonLake = true;
            numDDoors = 0;
            numDungeonPlatforms = 0;
            numDRooms = 0;
            vDungeonX = x;
            vDungeonY = y;
            dMinX = x;
            dMaxX = x;
            dMinY = y;
            dMaxY = y;
            dxStrength1 = WorldGen.genRand.Next(25, 30);
            dyStrength1 = WorldGen.genRand.Next(20, 25);
            dxStrength2 = WorldGen.genRand.Next(35, 50);
            dyStrength2 = WorldGen.genRand.Next(10, 15);
            double num4 = Main.maxTilesX / 60;
            num4 += (double)WorldGen.genRand.Next(0, (int)(num4 / 3.0));
            double num5 = num4;
            int num6 = 5;
            DungeonRoom(vDungeonX, vDungeonY, tileType, wallType);
            while (num4 > 0.0)
            {
                if (vDungeonX < dMinX)
                    dMinX = vDungeonX;

                if (vDungeonX > dMaxX)
                    dMaxX = vDungeonX;

                if (vDungeonY > dMaxY)
                    dMaxY = vDungeonY;

                num4 -= 1.0;
                Main.statusText = Lang.gen[58].Value + " " + (int)((num5 - num4) / num5 * 60.0) + "%";
                if (num6 > 0)
                    num6--;

                if ((num6 == 0) & (WorldGen.genRand.NextBool(3)))
                {
                    num6 = 5;
                    if (WorldGen.genRand.NextBool(2))
                    {
                        int dungeonX = this.vDungeonX;
                        int dungeonY = this.vDungeonY;
                        DungeonHalls(dungeonX, dungeonY, tileType, wallType);
                        if (WorldGen.genRand.NextBool(2))
                            DungeonHalls(dungeonX, dungeonY, tileType, wallType);

                        DungeonRoom(dungeonX, dungeonY, tileType, wallType);
                        this.vDungeonX = dungeonX;
                        this.vDungeonY = dungeonY;
                    }
                    else
                    {
                        DungeonRoom(vDungeonX, vDungeonY, tileType, wallType);
                    }
                }
                else
                {
                    DungeonHalls(vDungeonX, vDungeonY, tileType, wallType);
                }
            }

            DungeonRoom(vDungeonX, vDungeonY, tileType, wallType);
            int num7 = dRoomX[0];
            int num8 = dRoomY[0];
            for (int i = 0; i < numDRooms; i++)
            {
                if (dRoomY[i] < num8)
                {
                    num7 = dRoomX[i];
                    num8 = dRoomY[i];
                }
            }

            vDungeonX = num7;
            vDungeonY = num8;
            dEntranceX = num7;
            dSurface = false;
            num6 = 5;
            if (WorldGen.drunkWorldGen)
                dSurface = true;

            while (!dSurface)
            {
                if (num6 > 0)
                    num6--;

                if (num6 == 0 && WorldGen.genRand.NextBool(5) && (double)vDungeonY > Main.worldSurface + 100.0)
                {
                    num6 = 10;
                    int dungeonX2 = vDungeonX;
                    int dungeonY2 = vDungeonY;
                    DungeonHalls(vDungeonX, vDungeonY, tileType, wallType, forceX: true);
                    DungeonRoom(vDungeonX, vDungeonY, tileType, wallType);
                    vDungeonX = dungeonX2;
                    vDungeonY = dungeonY2;
                }

                DungeonStairs(vDungeonX, vDungeonY, tileType, wallType);
            }

            DungeonEnt(vDungeonX, vDungeonY, tileType, wallType);
            Main.statusText = Lang.gen[58].Value + " 65%";
            int num9 = Main.maxTilesX * 2;
            int num10;
            for (num10 = 0; num10 < num9; num10++)
            {
                int i2 = WorldGen.genRand.Next(dMinX, dMaxX);
                int num11 = dMinY;
                if ((double)num11 < Main.worldSurface)
                    num11 = (int)Main.worldSurface;

                int j = WorldGen.genRand.Next(num11, dMaxY);
                //num10 = ((!DungeonPitTrap(i2, j, tileType, wallType)) ? (num10 + 1) : (num10 + 1500));
            }

            for (int k = 0; k < numDRooms; k++)
            {
                for (int l = dRoomL[k]; l <= dRoomR[k]; l++)
                {
                    if (!Main.tile[l, dRoomT[k] - 1].HasTile)
                    {
                        dungeonPlatformX[numDungeonPlatforms] = l;
                        dungeonPlatformY[numDungeonPlatforms] = dRoomT[k] - 1;
                        if (numDungeonPlatforms < 499)
                            numDungeonPlatforms++;
                        break;
                    }
                }

                for (int m = dRoomL[k]; m <= dRoomR[k]; m++)
                {
                    if (!Main.tile[m, dRoomB[k] + 1].HasTile)
                    {
                        dungeonPlatformX[numDungeonPlatforms] = m;
                        dungeonPlatformY[numDungeonPlatforms] = dRoomB[k] + 1;
                        if (numDungeonPlatforms < 499)
                            numDungeonPlatforms++;
                        break;
                    }
                }

                for (int n = dRoomT[k]; n <= dRoomB[k]; n++)
                {
                    if (!Main.tile[dRoomL[k] - 1, n].HasTile)
                    {
                        DDoorX[numDDoors] = dRoomL[k] - 1;
                        DDoorY[numDDoors] = n;
                        DDoorPos[numDDoors] = -1;
                        if (numDDoors < 499)
                            numDDoors++;
                        break;
                    }
                }

                for (int num12 = dRoomT[k]; num12 <= dRoomB[k]; num12++)
                {
                    if (!Main.tile[dRoomR[k] + 1, num12].HasTile)
                    {
                        DDoorX[numDDoors] = dRoomR[k] + 1;
                        DDoorY[numDDoors] = num12;
                        DDoorPos[numDDoors] = 1;
                        if (numDDoors < 499)
                            numDDoors++;
                        break;
                    }
                }
            }

            Main.statusText = Lang.gen[58].Value + " 70%";
            int num13 = 0;
            int num14 = 1000;
            int num15 = 0;
            int num16 = Main.maxTilesX / 100;
            if (WorldGen.getGoodWorldGen)
                num16 *= 3;

            while (num15 < num16)
            {
                num13++;
                int num17 = WorldGen.genRand.Next(dMinX, dMaxX);
                int num18 = WorldGen.genRand.Next((int)Main.worldSurface + 25, dMaxY);
                if (WorldGen.drunkWorldGen)
                    num18 = WorldGen.genRand.Next(vDungeonY + 25, dMaxY);

                int num19 = num17;
                if (Main.tile[num17, num18].WallType == wallType && !Main.tile[num17, num18].HasTile)
                {
                    int num20 = 1;
                    if (WorldGen.genRand.NextBool(2))
                        num20 = -1;

                    for (; !Main.tile[num17, num18].HasTile; num18 += num20)
                    {
                    }

                    if (Main.tile[num17 - 1, num18].HasTile && Main.tile[num17 + 1, num18].HasTile && Main.tile[num17 - 1, num18].TileType != crackedType && !Main.tile[num17 - 1, num18 - num20].HasTile && !Main.tile[num17 + 1, num18 - num20].HasTile)
                    {
                        num15++;
                        int num21 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile[num17 - 1, num18].HasTile && Main.tile[num17 - 1, num18].TileType != crackedType && Main.tile[num17, num18 + num20].HasTile && Main.tile[num17, num18].HasTile && !Main.tile[num17, num18 - num20].HasTile && num21 > 0)
                        {
                            Main.tile[num17, num18].TileType = TileID.Spikes;
                            if (!Main.tile[num17 - 1, num18 - num20].HasTile && !Main.tile[num17 + 1, num18 - num20].HasTile)
                            {
                                Main.tile[num17, num18 - num20].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20].TileType = TileID.Spikes;

                                Main.tile[num17, num18 - num20 * 2].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20 * 2].TileType = TileID.Spikes;

                            }

                            num17--;
                            num21--;
                        }

                        num21 = WorldGen.genRand.Next(5, 13);
                        num17 = num19 + 1;
                        while (Main.tile[num17 + 1, num18].HasTile && Main.tile[num17 + 1, num18].TileType != crackedType && Main.tile[num17, num18 + num20].HasTile && Main.tile[num17, num18].HasTile && !Main.tile[num17, num18 - num20].HasTile && num21 > 0)
                        {
                            Main.tile[num17, num18].TileType = TileID.Spikes;
                            if (!Main.tile[num17 - 1, num18 - num20].HasTile && !Main.tile[num17 + 1, num18 - num20].HasTile)
                            {
                                Main.tile[num17, num18 - num20].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20].TileType = TileID.Spikes;

                                Main.tile[num17, num18 - num20 * 2].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20 * 2].TileType = TileID.Spikes;

                            }

                            num17++;
                            num21--;
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            num13 = 0;
            num14 = 1000;
            num15 = 0;
            Main.statusText = Lang.gen[58].Value + " 75%";
            while (num15 < num16)
            {
                num13++;
                int num22 = WorldGen.genRand.Next(dMinX, dMaxX);
                int num23 = WorldGen.genRand.Next((int)Main.worldSurface + 25, dMaxY);
                int num24 = num23;
                if (Main.tile[num22, num23].WallType == wallType && !Main.tile[num22, num23].HasTile)
                {
                    int num25 = 1;
                    if (WorldGen.genRand.NextBool(2))
                        num25 = -1;

                    for (; num22 > 5 && num22 < Main.maxTilesX - 5 && !Main.tile[num22, num23].HasTile; num22 += num25)
                    {
                    }

                    if (Main.tile[num22, num23 - 1].HasTile && Main.tile[num22, num23 + 1].HasTile && Main.tile[num22, num23 - 1].TileType != crackedType && !Main.tile[num22 - num25, num23 - 1].HasTile && !Main.tile[num22 - num25, num23 + 1].HasTile)
                    {
                        num15++;
                        int num26 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile[num22, num23 - 1].HasTile && Main.tile[num22, num23 - 1].TileType != crackedType && Main.tile[num22 + num25, num23].HasTile && Main.tile[num22, num23].HasTile && !Main.tile[num22 - num25, num23].HasTile && num26 > 0)
                        {
                            Main.tile[num22, num23].TileType = TileID.Spikes;
                            if (!Main.tile[num22 - num25, num23 - 1].HasTile && !Main.tile[num22 - num25, num23 + 1].HasTile)
                            {
                                Main.tile[num22 - num25, num23].TileType = TileID.Spikes;
                                Main.tile[num22 - num25, num23].Clear(TileDataType.Slope);
                                Main.tile[num22 - num25 * 2, num23].TileType = TileID.Spikes;
                                Main.tile[num22 - num25 * 2, num23].Clear(TileDataType.Slope);
                            }

                            num23--;
                            num26--;
                        }

                        num26 = WorldGen.genRand.Next(5, 13);
                        num23 = num24 + 1;
                        while (Main.tile[num22, num23 + 1].HasTile && Main.tile[num22, num23 + 1].TileType != crackedType && Main.tile[num22 + num25, num23].HasTile && Main.tile[num22, num23].HasTile && !Main.tile[num22 - num25, num23].HasTile && num26 > 0)
                        {
                            Main.tile[num22, num23].TileType = TileID.Spikes;
                            if (!Main.tile[num22 - num25, num23 - 1].HasTile && !Main.tile[num22 - num25, num23 + 1].HasTile)
                            {
                                Main.tile[num22 - num25, num23].TileType = TileID.Spikes;
                                Main.tile[num22 - num25, num23].Clear(TileDataType.Slope);
                                Main.tile[num22 - num25 * 2, num23].TileType = TileID.Spikes;
                                Main.tile[num22 - num25 * 2, num23].Clear(TileDataType.Slope);
                            }

                            num23++;
                            num26--;
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            Main.statusText = Lang.gen[58].Value + " 80%";
            for (int num27 = 0; num27 < numDDoors; num27++)
            {
                int num28 = DDoorX[num27] - 10;
                int num29 = DDoorX[num27] + 10;
                int num30 = 100;
                int num31 = 0;
                int num32 = 0;
                int num33 = 0;
                for (int num34 = num28; num34 < num29; num34++)
                {
                    bool flag = true;
                    int num35 = DDoorY[num27];
                    while (num35 > 10 && !Main.tile[num34, num35].HasTile)
                    {
                        num35--;
                    }

                    if (!tileTypes.Contains(Main.tile[num34, num35].TileType))
                        flag = false;

                    num32 = num35;
                    for (num35 = DDoorY[num27]; !Main.tile[num34, num35].HasTile; num35++)
                    {
                    }

                    if (!tileTypes.Contains(Main.tile[num34, num35].TileType))
                        flag = false;

                    num33 = num35;
                    if (num33 - num32 < 3)
                        continue;

                    int num36 = num34 - 20;
                    int num37 = num34 + 20;
                    int num38 = num33 - 10;
                    int num39 = num33 + 10;
                    for (int num40 = num36; num40 < num37; num40++)
                    {
                        for (int num41 = num38; num41 < num39; num41++)
                        {
                            if (Main.tile[num40, num41].HasTile && Main.tile[num40, num41].TileType == TileID.ClosedDoor)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag)
                    {
                        for (int num42 = num33 - 3; num42 < num33; num42++)
                        {
                            for (int num43 = num34 - 3; num43 <= num34 + 3; num43++)
                            {
                                if (Main.tile[num43, num42].HasTile)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (flag && num33 - num32 < 20)
                    {
                        bool flag2 = false;
                        if (DDoorPos[num27] == 0 && num33 - num32 < num30)
                            flag2 = true;

                        if (DDoorPos[num27] == -1 && num34 > num31)
                            flag2 = true;

                        if (DDoorPos[num27] == 1 && (num34 < num31 || num31 == 0))
                            flag2 = true;

                        if (flag2)
                        {
                            num31 = num34;
                            num30 = num33 - num32;
                        }
                    }
                }

                if (num30 >= 20)
                    continue;

                int num44 = num31;
                int num45 = DDoorY[num27];
                int num46 = num45;
                for (; !Main.tile[num44, num45].HasTile; num45++)
                {
                    WorldUtils.ClearTile(num44, num45);
                }

                while (!Main.tile[num44, num46].HasTile)
                {
                    num46--;
                }

                num45--;
                num46++;
                for (int num47 = num46; num47 < num45 - 2; num47++)
                {
                    Main.tile[num44, num47].Clear(TileDataType.Slope);
                    Main.tile[num44, num47].TileType = tileType;
                    if (Main.tile[num44 - 1, num47].TileType == tileType)
                    {
                        Main.tile[num44 - 1, num47].ClearEverything();
                        Main.tile[num44 - 1, num47].WallType = (ushort)wallType;
                    }

                    if (Main.tile[num44 - 2, num47].TileType == tileType)
                    {
                        Main.tile[num44 - 2, num47].ClearEverything();
                        Main.tile[num44 - 2, num47].WallType = (ushort)wallType;
                    }

                    if (Main.tile[num44 + 1, num47].TileType == tileType)
                    {
                        Main.tile[num44 + 1, num47].ClearEverything();
                        Main.tile[num44 + 1, num47].WallType = (ushort)wallType;
                    }

                    if (Main.tile[num44 + 2, num47].TileType == tileType)
                    {
                        Main.tile[num44 + 2, num47].ClearEverything();
                        Main.tile[num44 + 2, num47].WallType = (ushort)wallType;
                    }
                }

                int style = 13;
                if (WorldGen.genRand.NextBool(3))
                {
                    switch (wallType)
                    {
                        case 7:
                            style = 16;
                            break;
                        case 8:
                            style = 17;
                            break;
                        case 9:
                            style = 18;
                            break;
                    }
                }

                WorldGen.PlaceTile(num44, num45, 10, mute: true, forced: false, -1, style);
                num44--;
                int num48 = num45 - 3;
                while (!Main.tile[num44, num48].HasTile)
                {
                    num48--;
                }

                if (num45 - num48 < num45 - num46 + 5 && tileTypes.Contains(Main.tile[num44, num48].TileType))
                {
                    for (int num49 = num45 - 4 - WorldGen.genRand.Next(3); num49 > num48; num49--)
                    {
                        Main.tile[num44, num49].Clear(TileDataType.Slope);
                        Main.tile[num44, num49].TileType = tileType;
                        if (Main.tile[num44 - 1, num49].TileType == tileType)
                        {
                            Main.tile[num44 - 1, num49].ClearEverything();
                            Main.tile[num44 - 1, num49].WallType = (ushort)wallType;
                        }

                        if (Main.tile[num44 - 2, num49].TileType == tileType)
                        {
                            Main.tile[num44 - 2, num49].ClearEverything();
                            Main.tile[num44 - 2, num49].WallType = (ushort)wallType;
                        }
                    }
                }

                num44 += 2;
                num48 = num45 - 3;
                while (!Main.tile[num44, num48].HasTile)
                {
                    num48--;
                }

                if (num45 - num48 < num45 - num46 + 5 && tileTypes.Contains(Main.tile[num44, num48].TileType))
                {
                    for (int num50 = num45 - 4 - WorldGen.genRand.Next(3); num50 > num48; num50--)
                    {
                        Main.tile[num44, num50].Clear(TileDataType.Slope);
                        Main.tile[num44, num50].TileType = tileType;
                        if (Main.tile[num44 + 1, num50].TileType == tileType)
                        {
                            Main.tile[num44 + 1, num50].ClearEverything();
                            Main.tile[num44 + 1, num50].WallType = (ushort)wallType;
                        }

                        if (Main.tile[num44 + 2, num50].TileType == tileType)
                        {
                            Main.tile[num44 + 2, num50].ClearEverything();
                            Main.tile[num44 + 2, num50].WallType = (ushort)wallType;
                        }
                    }
                }

                num45++;
                num44--;
                for (int num51 = num45 - 8; num51 < num45; num51++)
                {
                    if (Main.tile[num44 + 2, num51].TileType == tileType)
                    {
                        Main.tile[num44 + 2, num51].ClearEverything();
                        Main.tile[num44 + 2, num51].WallType = (ushort)wallType;
                    }

                    if (Main.tile[num44 + 3, num51].TileType == tileType)
                    {
                        Main.tile[num44 + 3, num51].ClearEverything();
                        Main.tile[num44 + 3, num51].WallType = (ushort)wallType;
                    }

                    if (Main.tile[num44 - 2, num51].TileType == tileType)
                    {
                        Main.tile[num44 - 2, num51].ClearEverything();
                        Main.tile[num44 - 2, num51].WallType = (ushort)wallType;
                    }

                    if (Main.tile[num44 - 3, num51].TileType == tileType)
                    {
                        Main.tile[num44 - 3, num51].ClearEverything();
                        Main.tile[num44 - 3, num51].WallType = (ushort)wallType;
                    }
                }

                Main.tile[num44 - 1, num45].TileType = tileType;
                Main.tile[num44 - 1, num45].Clear(TileDataType.Slope);
                Main.tile[num44 + 1, num45].TileType = tileType;
                Main.tile[num44 + 1, num45].Clear(TileDataType.Slope);
            }

            for (int num52 = 0; num52 < 5; num52++)
            {
                for (int num53 = 0; num53 < 3; num53++)
                {
                    int num54 = WorldGen.genRand.Next(40, 240);
                    int num55 = WorldGen.genRand.Next(dMinX, dMaxX);
                    int num56 = WorldGen.genRand.Next(dMinY, dMaxY);
                    for (int num57 = num55 - num54; num57 < num55 + num54; num57++)
                    {
                        for (int num58 = num56 - num54; num58 < num56 + num54; num58++)
                        {
                            if ((double)num58 > Main.worldSurface)
                            {
                                double num59 = Math.Abs(num55 - num57);
                                double num60 = Math.Abs(num56 - num58);
                                if (Math.Sqrt(num59 * num59 + num60 * num60) < (double)num54 * 0.4 && wallTypes.Contains(Main.tile[num57, num58].WallType))
                                    Spread.WallDungeon(num57, num58, roomWalls[num53]);
                            }
                        }
                    }
                }
            }

            Main.statusText = Lang.gen[58].Value + " 85%";
            for (int num61 = 0; num61 < numDungeonPlatforms; num61++)
            {
                int num62 = dungeonPlatformX[num61];
                int num63 = dungeonPlatformY[num61];
                int num64 = Main.maxTilesX;
                int num65 = 10;
                if ((double)num63 < Main.worldSurface + 50.0)
                    num65 = 20;

                for (int num66 = num63 - 5; num66 <= num63 + 5; num66++)
                {
                    int num67 = num62;
                    int num68 = num62;
                    bool flag3 = false;
                    if (Main.tile[num67, num66].HasTile)
                    {
                        flag3 = true;
                    }
                    else
                    {
                        while (!Main.tile[num67, num66].HasTile)
                        {
                            num67--;
                            if (!tileTypes.Contains(Main.tile[num67, num66].TileType) || num67 == 0)
                            {
                                flag3 = true;
                                break;
                            }
                        }

                        while (!Main.tile[num68, num66].HasTile)
                        {
                            num68++;
                            if (!tileTypes.Contains(Main.tile[num68, num66].TileType) || num68 == Main.maxTilesX - 1)
                            {
                                flag3 = true;
                                break;
                            }
                        }
                    }

                    if (flag3 || num68 - num67 > num65)
                        continue;

                    bool flag4 = true;
                    int num69 = num62 - num65 / 2 - 2;
                    int num70 = num62 + num65 / 2 + 2;
                    int num71 = num66 - 5;
                    int num72 = num66 + 5;
                    for (int num73 = num69; num73 <= num70; num73++)
                    {
                        for (int num74 = num71; num74 <= num72; num74++)
                        {
                            if (Main.tile[num73, num74].HasTile && Main.tile[num73, num74].TileType == TileID.Platforms)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                    }

                    for (int num75 = num66 + 3; num75 >= num66 - 5; num75--)
                    {
                        if (Main.tile[num62, num75].HasTile)
                        {
                            flag4 = false;
                            break;
                        }
                    }

                    if (flag4)
                    {
                        num64 = num66;
                        break;
                    }
                }

                if (num64 <= num63 - 10 || num64 >= num63 + 10)
                    continue;

                int num76 = num62;
                int num77 = num64;
                int num78 = num62 + 1;
                while (!Main.tile[num76, num77].HasTile)
                {
                    Main.tile[num76, num77].TileType = TileID.Platforms;
                    Main.tile[num76, num77].Clear(TileDataType.Slope);
                    switch (wallType)
                    {
                        case 7:
                            Main.tile[num76, num77].TileFrameY = 108;
                            break;
                        case 8:
                            Main.tile[num76, num77].TileFrameY = 144;
                            break;
                        default:
                            Main.tile[num76, num77].TileFrameY = 126;
                            break;
                    }

                    WorldGen.TileFrame(num76, num77);
                    num76--;
                }

                for (; !Main.tile[num78, num77].HasTile; num78++)
                {
                    Main.tile[num78, num77].TileType = TileID.Platforms;
                    Main.tile[num78, num77].Clear(TileDataType.Slope);
                    switch (wallType)
                    {
                        case 7:
                            Main.tile[num78, num77].TileFrameY = 108;
                            break;
                        case 8:
                            Main.tile[num78, num77].TileFrameY = 144;
                            break;
                        default:
                            Main.tile[num78, num77].TileFrameY = 126;
                            break;
                    }

                    WorldGen.TileFrame(num78, num77);
                }
            }

            int[] array2 = new int[3] {
                WorldGen.genRand.Next(9, 13),
                WorldGen.genRand.Next(9, 13),
                0
            };

            while (array2[1] == array2[0])
            {
                array2[1] = WorldGen.genRand.Next(9, 13);
            }

            array2[2] = WorldGen.genRand.Next(9, 13);
            while (array2[2] == array2[0] || array2[2] == array2[1])
            {
                array2[2] = WorldGen.genRand.Next(9, 13);
            }

            Main.statusText = Lang.gen[58].Value + " 90%";
            num13 = 0;
            num14 = 1000;
            num15 = 0;
            while (num15 < Main.maxTilesX / 20)
            {
                num13++;
                int num83 = WorldGen.genRand.Next(dMinX, dMaxX);
                int num84 = WorldGen.genRand.Next(dMinY, dMaxY);
                bool flag6 = true;
                if (wallTypes.AsEnumerable().Contains(Main.tile[num83, num84].WallType) && !Main.tile[num83, num84].HasTile)
                {
                    int num85 = 1;
                    if (WorldGen.genRand.NextBool(2))
                        num85 = -1;

                    while (flag6 && !Main.tile[num83, num84].HasTile)
                    {
                        num83 -= num85;
                        if (num83 < 5 || num83 > Main.maxTilesX - 5)
                            flag6 = false;
                        else if (Main.tile[num83, num84].HasTile && !tileTypes.Contains(Main.tile[num83, num84].TileType))
                            flag6 = false;
                    }

                    if (flag6 && Main.tile[num83, num84].HasTile && tileTypes.Contains(Main.tile[num83, num84].TileType) && Main.tile[num83, num84 - 1].HasTile && tileTypes.Contains(Main.tile[num83, num84 - 1].TileType) && Main.tile[num83, num84 + 1].HasTile && tileTypes.Contains(Main.tile[num83, num84 + 1].TileType))
                    {
                        num83 += num85;
                        for (int num86 = num83 - 3; num86 <= num83 + 3; num86++)
                        {
                            for (int num87 = num84 - 3; num87 <= num84 + 3; num87++)
                            {
                                if (Main.tile[num86, num87].HasTile && Main.tile[num86, num87].TileType == TileID.Platforms)
                                {
                                    flag6 = false;
                                    break;
                                }
                            }
                        }

                        if (flag6 && (!Main.tile[num83, num84 - 1].HasTile & !Main.tile[num83, num84 - 2].HasTile & !Main.tile[num83, num84 - 3].HasTile))
                        {
                            int num88 = num83;
                            int num89 = num83;
                            for (; num88 > dMinX && num88 < dMaxX && !Main.tile[num88, num84].HasTile && !Main.tile[num88, num84 - 1].HasTile && !Main.tile[num88, num84 + 1].HasTile; num88 += num85)
                            {
                            }

                            num88 = Math.Abs(num83 - num88);
                            bool flag7 = false;
                            if (WorldGen.genRand.NextBool(2))
                                flag7 = true;

                            if (num88 > 5)
                            {
                                for (int num90 = WorldGen.genRand.Next(1, 4); num90 > 0; num90--)
                                {
                                    Main.tile[num83, num84].Clear(TileDataType.Slope);
                                    Main.tile[num83, num84].TileType = TileID.Platforms;
                                    if (Main.tile[num83, num84].WallType == roomWalls[0])
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[0]);
                                    else if (Main.tile[num83, num84].WallType == roomWalls[1])
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[1]);
                                    else
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[2]);

                                    WorldGen.TileFrame(num83, num84);
                                    if (flag7)
                                    {
                                        WorldGen.PlaceTile(num83, num84 - 1, 50, mute: true);
                                        if (WorldGen.genRand.NextBool(50) && (double)num84 > (Main.worldSurface + Main.rockLayer) / 2.0 && Main.tile[num83, num84 - 1].TileType == 50)
                                            Main.tile[num83, num84 - 1].TileFrameX = 90;
                                    }

                                    num83 += num85;
                                }

                                num13 = 0;
                                num15++;
                                if (!flag7 && WorldGen.genRand.NextBool(2))
                                {
                                    num83 = num89;
                                    num84--;
                                    int num91 = 0;
                                    if (WorldGen.genRand.NextBool(4))
                                        num91 = 1;

                                    switch (num91)
                                    {
                                        case 0:
                                            num91 = 13;
                                            break;
                                        case 1:
                                            num91 = 49;
                                            break;
                                    }

                                    WorldGen.PlaceTile(num83, num84, num91, mute: true);
                                    if (Main.tile[num83, num84].TileType == TileID.Bottles)
                                    {
                                        if (WorldGen.genRand.NextBool(2))
                                            Main.tile[num83, num84].TileFrameX = 18;
                                        else
                                            Main.tile[num83, num84].TileFrameX = 36;
                                    }
                                }
                            }
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            Main.statusText = Lang.gen[58].Value + " 95%";
            int num92 = 1;
            for (int num93 = 0; num93 < numDRooms; num93++)
            {
                int num94 = 0;
                while (num94 < 1000)
                {
                    int num95 = (int)((double)dRoomSize[num93] * 0.4);
                    int i3 = dRoomX[num93] + WorldGen.genRand.Next(-num95, num95 + 1);
                    int num96 = dRoomY[num93] + WorldGen.genRand.Next(-num95, num95 + 1);
                    int num97 = 0;
                    int style3 = 2;
                    if (num92 == 1)
                        num92++;

                    switch (num92)
                    {
                        case 2:
                            num97 = 155;
                            break;
                        case 3:
                            num97 = 156;
                            break;
                        case 4:
                            num97 = ((!WorldGen.remixWorldGen) ? 157 : 2623);
                            break;
                        case 5:
                            num97 = 163;
                            break;
                        case 6:
                            num97 = 113;
                            break;
                        case 7:
                            num97 = 3317;
                            break;
                        case 8:
                            num97 = 327;
                            style3 = 0;
                            break;
                        default:
                            num97 = 164;
                            num92 = 0;
                            break;
                    }

                    if ((double)num96 < Main.worldSurface + 50.0)
                    {
                        num97 = 327;
                        style3 = 0;
                    }

                    if (num97 == 0 && WorldGen.genRand.NextBool(2))
                    {
                        num94 = 1000;
                        continue;
                    }

                    if (WorldGen.AddBuriedChest(i3, num96, num97, notNearOtherChests: false, style3, trySlope: false, 0))
                    {
                        num94 += 1000;
                        num92++;
                    }

                    num94++;
                }
            }

            dMinX -= 25;
            dMaxX += 25;
            dMinY -= 25;
            dMaxY += 25;
            if (dMinX < 0)
                dMinX = 0;

            if (dMaxX > Main.maxTilesX)
                dMaxX = Main.maxTilesX;

            if (dMinY < 0)
                dMinY = 0;

            if (dMaxY > Main.maxTilesY)
                dMaxY = Main.maxTilesY;

            num13 = 0;
            num14 = 1000;
            num15 = 0;
            MakeDungeon_Lights(tileType, ref num13, num14, ref num15, roomWalls);
            num13 = 0;
            num14 = 1000;
            num15 = 0;
            //MakeDungeon_Traps(ref num13, num14, ref num15);
            double count = MakeDungeon_GroundFurniture(wallType);
            //count = MakeDungeon_Pictures(array, count);
            count = MakeDungeon_Banners(roomWalls, count);
        }

        private void MakeDungeon_Lights(ushort tileType, ref int failCount, int failMax, ref int numAdd, int[] roomWall)
        {
            int[] array = new int[3] {
            WorldGen.genRand.Next(7),
            WorldGen.genRand.Next(7),
            0
        };

            while (array[1] == array[0])
            {
                array[1] = WorldGen.genRand.Next(7);
            }

            array[2] = WorldGen.genRand.Next(7);
            while (array[2] == array[0] || array[2] == array[1])
            {
                array[2] = WorldGen.genRand.Next(7);
            }

            while (numAdd < Main.maxTilesX / 150)
            {
                failCount++;
                int num = WorldGen.genRand.Next(dMinX, dMaxX);
                int num2 = WorldGen.genRand.Next(dMinY, dMaxY);
                if (wallTypes.AsEnumerable().Contains(Main.tile[num, num2].WallType))
                {
                    for (int num3 = num2; num3 > dMinY; num3--)
                    {
                        if (Main.tile[num, num3 - 1].HasTile && Main.tile[num, num3 - 1].TileType == tileType)
                        {
                            bool flag = false;
                            for (int i = num - 15; i < num + 15; i++)
                            {
                                for (int j = num3 - 15; j < num3 + 15; j++)
                                {
                                    if (i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY && (Main.tile[i, j].TileType == 42 || Main.tile[i, j].TileType == 34))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }

                            if (Main.tile[num - 1, num3].HasTile || Main.tile[num + 1, num3].HasTile || Main.tile[num - 1, num3 + 1].HasTile || Main.tile[num + 1, num3 + 1].HasTile || Main.tile[num, num3 + 2].HasTile)
                                flag = true;

                            if (flag)
                                break;

                            bool flag2 = false;
                            if (!flag2 && WorldGen.genRand.NextBool(7))
                            {
                                int style = 27;
                                switch (roomWall[0])
                                {
                                    case 7:
                                        style = 27;
                                        break;
                                    case 8:
                                        style = 28;
                                        break;
                                    case 9:
                                        style = 29;
                                        break;
                                }

                                bool flag3 = false;
                                for (int k = 0; k < 15; k++)
                                {
                                    if (WorldGen.SolidTile(num, num3 + k))
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }

                                if (!flag3)
                                    WorldGen.PlaceChand(num, num3, 34, style);

                                if (Main.tile[num, num3].TileType == 34)
                                {
                                    flag2 = true;
                                    failCount = 0;
                                    numAdd++;
                                    for (int l = 0; l < 1000; l++)
                                    {
                                        int num4 = num + WorldGen.genRand.Next(-12, 13);
                                        int num5 = num3 + WorldGen.genRand.Next(3, 21);
                                        if (Main.tile[num4, num5].HasTile || Main.tile[num4, num5 + 1].HasTile || !tileTypes.Contains(Main.tile[num4 - 1, num5].TileType) || !tileTypes.Contains(Main.tile[num4 + 1, num5].TileType) || !Collision.CanHit(new Point(num4 * 16, num5 * 16), 16, 16, new Point(num * 16, num3 * 16 + 1), 16, 16))
                                            continue;

                                        if (((WorldGen.SolidTile(num4 - 1, num5) && Main.tile[num4 - 1, num5].TileType != 10) || (WorldGen.SolidTile(num4 + 1, num5) && Main.tile[num4 + 1, num5].TileType != 10) || WorldGen.SolidTile(num4, num5 + 1)) && wallTypes.Contains(Main.tile[num4, num5].WallType) && (tileTypes.Contains(Main.tile[num4 - 1, num5].TileType) || tileTypes.Contains(Main.tile[num4 + 1, num5].TileType)))
                                            WorldGen.PlaceTile(num4, num5, 136, mute: true);

                                        if (!Main.tile[num4, num5].HasTile)
                                            continue;

                                        while (num4 != num || num5 != num3)
                                        {
                                            Tile tile = Main.tile[num4, num5];
                                            tile.RedWire = true;
                                            if (num4 > num)
                                                num4--;

                                            if (num4 < num)
                                                num4++;

                                            tile.RedWire = true;
                                            if (num5 > num3)
                                                num5--;

                                            if (num5 < num3)
                                                num5++;

                                            tile.RedWire = true;
                                        }

                                        if (WorldGen.genRand.Next(3) > 0)
                                        {
                                            Main.tile[num, num3].TileFrameX = 18;
                                            Main.tile[num, num3 + 1].TileFrameX = 18;
                                        }

                                        break;
                                    }
                                }
                            }

                            if (flag2)
                                break;

                            int style2 = array[0];
                            if (Main.tile[num, num3].WallType == roomWall[1])
                                style2 = array[1];

                            if (Main.tile[num, num3].WallType == roomWall[2])
                                style2 = array[2];

                            WorldGen.Place1x2Top(num, num3, 42, style2);
                            if (Main.tile[num, num3].TileType != 42)
                                break;

                            flag2 = true;
                            failCount = 0;
                            numAdd++;
                            for (int m = 0; m < 1000; m++)
                            {
                                int num6 = num + WorldGen.genRand.Next(-12, 13);
                                int num7 = num3 + WorldGen.genRand.Next(3, 21);
                                if (Main.tile[num6, num7].HasTile || Main.tile[num6, num7 + 1].HasTile || Main.tile[num6 - 1, num7].TileType == 48 || Main.tile[num6 + 1, num7].TileType == 48 || !Collision.CanHit(new Point(num6 * 16, num7 * 16), 16, 16, new Point(num * 16, num3 * 16 + 1), 16, 16))
                                    continue;

                                if ((WorldGen.SolidTile(num6 - 1, num7) && Main.tile[num6 - 1, num7].TileType != 10) || (WorldGen.SolidTile(num6 + 1, num7) && Main.tile[num6 + 1, num7].TileType != 10) || WorldGen.SolidTile(num6, num7 + 1))
                                    WorldGen.PlaceTile(num6, num7, 136, mute: true);

                                if (!Main.tile[num6, num7].HasTile)
                                    continue;

                                while (num6 != num || num7 != num3)
                                {
                                    Tile tile = Main.tile[num6, num7];
                                    tile.RedWire = true;
                                    if (num6 > num)
                                        num6--;

                                    if (num6 < num)
                                        num6++;

                                    tile = Main.tile[num6, num7];
                                    tile.RedWire = true;

                                    if (num7 > num3)
                                        num7--;

                                    if (num7 < num3)
                                        num7++;

                                    tile = Main.tile[num6, num7];
                                    tile.RedWire = true;
                                }

                                if (WorldGen.genRand.Next(3) > 0)
                                {
                                    Main.tile[num, num3].TileFrameX = 18;
                                    Main.tile[num, num3 + 1].TileFrameX = 18;
                                }

                                break;
                            }

                            break;
                        }
                    }
                }

                if (failCount > failMax)
                {
                    numAdd++;
                    failCount = 0;
                }
            }
        }

        private double MakeDungeon_Banners(int[] roomWall, double count)
        {
            count = 840000.0 / (double)Main.maxTilesX;
            for (int i = 0; (double)i < count; i++)
            {
                int num = WorldGen.genRand.Next(dMinX, dMaxX);
                int num2 = WorldGen.genRand.Next(dMinY, dMaxY);
                while (!wallTypes.Contains(Main.tile[num, num2].WallType) || Main.tile[num, num2].HasTile)
                {
                    num = WorldGen.genRand.Next(dMinX, dMaxX);
                    num2 = WorldGen.genRand.Next(dMinY, dMaxY);
                }

                while (!WorldGen.SolidTile(num, num2) && num2 > 10)
                {
                    num2--;
                }

                num2++;
                if (!wallTypes.AsEnumerable().Contains(Main.tile[num, num2].WallType) || Main.tile[num, num2 - 1].TileType == 48 || Main.tile[num, num2].HasTile || Main.tile[num, num2 + 1].HasTile || Main.tile[num, num2 + 2].HasTile || Main.tile[num, num2 + 3].HasTile)
                    continue;

                bool flag = true;
                for (int j = num - 1; j <= num + 1; j++)
                {
                    for (int k = num2; k <= num2 + 3; k++)
                    {
                        if (Main.tile[j, k].HasTile && (Main.tile[j, k].TileType == TileID.ClosedDoor || Main.tile[j, k].TileType == TileID.OpenDoor || Main.tile[j, k].TileType == TileID.Banners))
                            flag = false;
                    }
                }

                if (flag)
                {
                    int num3 = 10;
                    if (Main.tile[num, num2].WallType == roomWall[1])
                        num3 = 12;

                    if (Main.tile[num, num2].WallType == roomWall[2])
                        num3 = 14;

                    num3 += WorldGen.genRand.Next(2);
                    WorldGen.PlaceTile(num, num2, 91, mute: true, forced: false, -1, num3);
                }
            }

            return count;
        }

        private double MakeDungeon_GroundFurniture(int wallType)
        {
            double num = (double)(2000 * Main.maxTilesX) / 4200.0;
            int num2 = 1 + (int)((double)Main.maxTilesX / 4200.0);
            int num3 = 1 + (int)((double)Main.maxTilesX / 4200.0);
            for (int i = 0; (double)i < num; i++)
            {
                if (num2 > 0 || num3 > 0)
                    i--;

                int num4 = WorldGen.genRand.Next(dMinX, dMaxX);
                int j = WorldGen.genRand.Next((int)Main.worldSurface + 10, dMaxY);
                while (!wallTypes.Contains(Main.tile[num4, j].WallType) || Main.tile[num4, j].HasTile)
                {
                    num4 = WorldGen.genRand.Next(dMinX, dMaxX);
                    j = WorldGen.genRand.Next((int)Main.worldSurface + 10, dMaxY);
                }

                if (!wallTypes.AsEnumerable().Contains(Main.tile[num4, j].WallType) || Main.tile[num4, j].HasTile)
                    continue;

                for (; !WorldGen.SolidTile(num4, j) && j < Main.UnderworldLayer; j++)
                {
                }

                j--;
                int num5 = num4;
                int k = num4;
                while (!Main.tile[num5, j].HasTile && WorldGen.SolidTile(num5, j + 1))
                {
                    num5--;
                }

                num5++;
                for (; !Main.tile[k, j].HasTile && WorldGen.SolidTile(k, j + 1); k++)
                {
                }

                k--;
                int num6 = k - num5;
                int num7 = (k + num5) / 2;
                if (Main.tile[num7, j].HasTile || !wallTypes.AsEnumerable().Contains(Main.tile[num7, j].WallType) || !WorldGen.SolidTile(num7, j + 1) || Main.tile[num7, j + 1].TileType == 48)
                    continue;

                int style = 13;
                int style2 = 10;
                int style3 = 11;
                int num8 = 1;
                int num9 = 46;
                int style4 = 1;
                int num10 = 5;
                int num11 = 11;
                int num12 = 5;
                int num13 = 6;
                int num14 = 21;
                int num15 = 22;
                int num16 = 24;
                int num17 = 30;
                switch (wallType)
                {
                    case 8:
                        style = 14;
                        style2 = 11;
                        style3 = 12;
                        num8 = 2;
                        num9 = 47;
                        style4 = 2;
                        num10 = 6;
                        num11 = 12;
                        num12 = 6;
                        num13 = 7;
                        num14 = 22;
                        num15 = 23;
                        num16 = 25;
                        num17 = 31;
                        break;
                    case 9:
                        style = 15;
                        style2 = 12;
                        style3 = 13;
                        num8 = 3;
                        num9 = 48;
                        style4 = 3;
                        num10 = 7;
                        num11 = 13;
                        num12 = 7;
                        num13 = 8;
                        num14 = 23;
                        num15 = 24;
                        num16 = 26;
                        num17 = 32;
                        break;
                }

                if (Main.tile[num7, j].WallType >= 94 && Main.tile[num7, j].WallType <= 105)
                {
                    style = 17;
                    style2 = 14;
                    style3 = 15;
                    num8 = -1;
                    num9 = -1;
                    style4 = 5;
                    num10 = -1;
                    num11 = -1;
                    num12 = -1;
                    num13 = -1;
                    num14 = -1;
                    num15 = -1;
                    num16 = -1;
                    num17 = -1;
                }

                int num18 = WorldGen.genRand.Next(13);
                if ((num18 == 10 || num18 == 11 || num18 == 12) && !WorldGen.genRand.NextBool(4))
                    num18 = WorldGen.genRand.Next(13);

                while ((num18 == 2 && num9 == -1) || (num18 == 5 && num10 == -1) || (num18 == 6 && num11 == -1) || (num18 == 7 && num12 == -1) || (num18 == 8 && num13 == -1) || (num18 == 9 && num14 == -1) || (num18 == 10 && num15 == -1) || (num18 == 11 && num16 == -1) || (num18 == 12 && num17 == -1))
                {
                    num18 = WorldGen.genRand.Next(13);
                }

                int num19 = 0;
                int num20 = 0;
                if (num18 == 0)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 1)
                {
                    num19 = 4;
                    num20 = 3;
                }

                if (num18 == 2)
                {
                    num19 = 3;
                    num20 = 5;
                }

                if (num18 == 3)
                {
                    num19 = 4;
                    num20 = 6;
                }

                if (num18 == 4)
                {
                    num19 = 3;
                    num20 = 3;
                }

                if (num18 == 5)
                {
                    num19 = 5;
                    num20 = 3;
                }

                if (num18 == 6)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 7)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 8)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 9)
                {
                    num19 = 5;
                    num20 = 3;
                }

                if (num18 == 10)
                {
                    num19 = 2;
                    num20 = 4;
                }

                if (num18 == 11)
                {
                    num19 = 3;
                    num20 = 3;
                }

                if (num18 == 12)
                {
                    num19 = 2;
                    num20 = 5;
                }

                for (int l = num7 - num19; l <= num7 + num19; l++)
                {
                    for (int m = j - num20; m <= j; m++)
                    {
                        if (Main.tile[l, m].HasTile)
                        {
                            num18 = -1;
                            break;
                        }
                    }
                }

                if ((double)num6 < (double)num19 * 1.75)
                    num18 = -1;

                if (num2 > 0 || num3 > 0)
                {
                    if (num2 > 0)
                    {
                        WorldGen.PlaceTile(num7, j, 355, mute: true);
                        if (Main.tile[num7, j].TileType == 355)
                            num2--;
                    }
                    else if (num3 > 0)
                    {
                        WorldGen.PlaceTile(num7, j, 354, mute: true);
                        if (Main.tile[num7, j].TileType == 354)
                            num3--;
                    }

                    continue;
                }

                switch (num18)
                {
                    case 0:
                        {
                            WorldGen.PlaceTile(num7, j, 14, mute: true, forced: false, -1, style2);
                            if (Main.tile[num7, j].HasTile)
                            {
                                if (!Main.tile[num7 - 2, j].HasTile)
                                {
                                    WorldGen.PlaceTile(num7 - 2, j, 15, mute: true, forced: false, -1, style);
                                    if (Main.tile[num7 - 2, j].HasTile)
                                    {
                                        Main.tile[num7 - 2, j].TileFrameX += 18;
                                        Main.tile[num7 - 2, j - 1].TileFrameX += 18;
                                    }
                                }

                                if (!Main.tile[num7 + 2, j].HasTile)
                                    WorldGen.PlaceTile(num7 + 2, j, 15, mute: true, forced: false, -1, style);
                            }

                            for (int num22 = num7 - 1; num22 <= num7 + 1; num22++)
                            {
                                if (WorldGen.genRand.NextBool(2) && !Main.tile[num22, j - 2].HasTile)
                                {
                                    int num23 = WorldGen.genRand.Next(5);
                                    if (num8 != -1 && num23 <= 1 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].TileType])
                                        WorldGen.PlaceTile(num22, j - 2, 33, mute: true, forced: false, -1, num8);

                                    if (num23 == 2 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].TileType])
                                        WorldGen.PlaceTile(num22, j - 2, 49, mute: true);

                                    if (num23 == 3)
                                        WorldGen.PlaceTile(num22, j - 2, 50, mute: true);

                                    if (num23 == 4)
                                        WorldGen.PlaceTile(num22, j - 2, 103, mute: true);
                                }
                            }

                            break;
                        }
                    case 1:
                        {
                            WorldGen.PlaceTile(num7, j, 18, mute: true, forced: false, -1, style3);
                            if (!Main.tile[num7, j].HasTile)
                                break;

                            if (WorldGen.genRand.NextBool(2))
                            {
                                if (!Main.tile[num7 - 1, j].HasTile)
                                {
                                    WorldGen.PlaceTile(num7 - 1, j, 15, mute: true, forced: false, -1, style);
                                    if (Main.tile[num7 - 1, j].HasTile)
                                    {
                                        Main.tile[num7 - 1, j].TileFrameX += 18;
                                        Main.tile[num7 - 1, j - 1].TileFrameX += 18;
                                    }
                                }
                            }
                            else if (!Main.tile[num7 + 2, j].HasTile)
                            {
                                WorldGen.PlaceTile(num7 + 2, j, 15, mute: true, forced: false, -1, style);
                            }

                            for (int n = num7; n <= num7 + 1; n++)
                            {
                                if (WorldGen.genRand.NextBool(2) && !Main.tile[n, j - 1].HasTile)
                                {
                                    int num21 = WorldGen.genRand.Next(5);
                                    if (num8 != -1 && num21 <= 1 && !Main.tileLighted[Main.tile[n - 1, j - 1].TileType])
                                        WorldGen.PlaceTile(n, j - 1, 33, mute: true, forced: false, -1, num8);

                                    if (num21 == 2 && !Main.tileLighted[Main.tile[n - 1, j - 1].TileType])
                                        WorldGen.PlaceTile(n, j - 1, 49, mute: true);

                                    if (num21 == 3)
                                        WorldGen.PlaceTile(n, j - 1, 50, mute: true);

                                    if (num21 == 4)
                                        WorldGen.PlaceTile(n, j - 1, 103, mute: true);
                                }
                            }

                            break;
                        }
                    case 2:
                        WorldGen.PlaceTile(num7, j, 105, mute: true, forced: false, -1, num9);
                        break;
                    case 3:
                        WorldGen.PlaceTile(num7, j, 101, mute: true, forced: false, -1, style4);
                        break;
                    case 4:
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(num7, j, 15, mute: true, forced: false, -1, style);
                            Main.tile[num7, j].TileFrameX += 18;
                            Main.tile[num7, j - 1].TileFrameX += 18;
                        }
                        else
                        {
                            WorldGen.PlaceTile(num7, j, 15, mute: true, forced: false, -1, style);
                        }
                        break;
                    case 5:
                        if (WorldGen.genRand.NextBool(2))
                            WorldGen.Place4x2(num7, j, 79, 1, num10);
                        else
                            WorldGen.Place4x2(num7, j, 79, -1, num10);
                        break;
                    case 6:
                        WorldGen.PlaceTile(num7, j, 87, mute: true, forced: false, -1, num11);
                        break;
                    case 7:
                        WorldGen.PlaceTile(num7, j, 88, mute: true, forced: false, -1, num12);
                        break;
                    case 8:
                        WorldGen.PlaceTile(num7, j, 89, mute: true, forced: false, -1, num13);
                        break;
                    case 9:
                        if (WorldGen.genRand.NextBool(2))
                            WorldGen.Place4x2(num7, j, 90, 1, num14);
                        else
                            WorldGen.Place4x2(num7, j, 90, -1, num14);
                        break;
                    case 10:
                        WorldGen.PlaceTile(num7, j, 93, mute: true, forced: false, -1, num16);
                        break;
                    case 11:
                        WorldGen.PlaceTile(num7, j, 100, mute: true, forced: false, -1, num15);
                        break;
                    case 12:
                        WorldGen.PlaceTile(num7, j, 104, mute: true, forced: false, -1, num17);
                        break;
                }
            }

            return num;
        }

        public void DungeonRoom(int i, int j, ushort tileType, int wallType)
        {
            double num = genRand.Next(15, 30);
            Vector2D vector2D = default(Vector2D);
            vector2D.X = (double)genRand.Next(-10, 11) * 0.1;
            vector2D.Y = (double)genRand.Next(-10, 11) * 0.1;
            Vector2D vector2D2 = default(Vector2D);
            vector2D2.X = i;
            vector2D2.Y = (double)j - num / 2.0;
            int num2 = genRand.Next(10, 20);
            double num3 = vector2D2.X;
            double num4 = vector2D2.X;
            double num5 = vector2D2.Y;
            double num6 = vector2D2.Y;
            while (num2 > 0)
            {
                num2--;
                int num7 = (int)(vector2D2.X - num * 0.8 - 5.0);
                int num8 = (int)(vector2D2.X + num * 0.8 + 5.0);
                int num9 = (int)(vector2D2.Y - num * 0.8 - 5.0);
                int num10 = (int)(vector2D2.Y + num * 0.8 + 5.0);
                if (num7 < 0)
                    num7 = 0;

                if (num8 > Main.maxTilesX)
                    num8 = Main.maxTilesX;

                if (num9 < 0)
                    num9 = 0;

                if (num10 > Main.maxTilesY)
                    num10 = Main.maxTilesY;

                for (int k = num7; k < num8; k++)
                {
                    for (int l = num9; l < num10; l++)
                    {
                        if (k < dMinX)
                            dMinX = k;

                        if (k > dMaxX)
                            dMaxX = k;

                        if (l > dMaxY)
                            dMaxY = l;

                        Main.tile[k, l].LiquidAmount = 0;
                        if (!wallTypes.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].Clear(TileDataType.Slope);
                            Main.tile[k, l].TileType = tileType;
                        }
                    }
                }

                for (int m = num7 + 1; m < num8 - 1; m++)
                {
                    for (int n = num9 + 1; n < num10 - 1; n++)
                    {
                        Main.tile[m, n].WallType =  (ushort)wallType;
                    }
                }

                num7 = (int)(vector2D2.X - num * 0.5);
                num8 = (int)(vector2D2.X + num * 0.5);
                num9 = (int)(vector2D2.Y - num * 0.5);
                num10 = (int)(vector2D2.Y + num * 0.5);
                if (num7 < 0)
                    num7 = 0;

                if (num8 > Main.maxTilesX)
                    num8 = Main.maxTilesX;

                if (num9 < 0)
                    num9 = 0;

                if (num10 > Main.maxTilesY)
                    num10 = Main.maxTilesY;

                if ((double)num7 < num3)
                    num3 = num7;

                if ((double)num8 > num4)
                    num4 = num8;

                if ((double)num9 < num5)
                    num5 = num9;

                if ((double)num10 > num6)
                    num6 = num10;

                for (int num11 = num7; num11 < num8; num11++)
                {
                    for (int num12 = num9; num12 < num10; num12++)
                    {
                        WorldUtils.ClearTile(num11, num12);
                        Main.tile[num11, num12].WallType =  (ushort)wallType;
                    }
                }

                vector2D2 += vector2D;
                vector2D.X += (double)genRand.Next(-10, 11) * 0.05;
                vector2D.Y += (double)genRand.Next(-10, 11) * 0.05;
                if (vector2D.X > 1.0)
                    vector2D.X = 1.0;

                if (vector2D.X < -1.0)
                    vector2D.X = -1.0;

                if (vector2D.Y > 1.0)
                    vector2D.Y = 1.0;

                if (vector2D.Y < -1.0)
                    vector2D.Y = -1.0;
            }

            dRoomX[numDRooms] = (int)vector2D2.X;
            dRoomY[numDRooms] = (int)vector2D2.Y;
            dRoomSize[numDRooms] = (int)num;
            dRoomL[numDRooms] = (int)num3;
            dRoomR[numDRooms] = (int)num4;
            dRoomT[numDRooms] = (int)num5;
            dRoomB[numDRooms] = (int)num6;
            //dRoomTreasure[numDRooms] = false;

            if(numDRooms < 99) 
                numDRooms++;
        }

        public void DungeonEnt(int i, int j, ushort tileType, int wallType)
        {
            int num = 60;
            for (int k = i - num; k < i + num; k++)
            {
                for (int l = j - num; l < j + num; l++)
                {
                    if (InWorld(k, l))
                    {
                        Tile tile = Main.tile[k, l];
                        tile.LiquidAmount= 0;
                        tile.LiquidType = LiquidID.Lava;
                        tile.Clear(TileDataType.Slope);
                    }
                }
            }

            double dxStrength = dxStrength1;
            double dyStrength = dyStrength1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = (double)j - dyStrength / 2.0;
            dMinY = (int)vector2D.Y;
            int num2 = 1;
            if (i > Main.maxTilesX / 2)
                num2 = -1;

            if (drunkWorldGen || getGoodWorldGen)
                num2 *= -1;

            int num3 = (int)(vector2D.X - dxStrength * 0.6 - (double)genRand.Next(2, 5));
            int num4 = (int)(vector2D.X + dxStrength * 0.6 + (double)genRand.Next(2, 5));
            int num5 = (int)(vector2D.Y - dyStrength * 0.6 - (double)genRand.Next(2, 5));
            int num6 = (int)(vector2D.Y + dyStrength * 0.6 + (double)genRand.Next(8, 16));
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int m = num3; m < num4; m++)
            {
                for (int n = num5; n < num6; n++)
                {
                    Main.tile[m, n].LiquidAmount= 0;
                    if (Main.tile[m, n].WallType != wallType)
                    {
                        Main.tile[m, n].WallType =  0;
                        if (m > num3 + 1 && m < num4 - 2 && n > num5 + 1 && n < num6 - 2)
                            Main.tile[m, n].WallType =  (ushort)wallType;

                        Main.tile[m, n].TileType = tileType;
                        Main.tile[m, n].Clear(TileDataType.Slope);
                    }
                }
            }

            int num7 = num3;
            int num8 = num3 + 5 + genRand.Next(4);
            int num9 = num5 - 3 - genRand.Next(3);
            int num10 = num5;
            for (int num11 = num7; num11 < num8; num11++)
            {
                for (int num12 = num9; num12 < num10; num12++)
                {
                    Main.tile[num11, num12].LiquidAmount= 0;
                    if (Main.tile[num11, num12].WallType != wallType)
                    {
                        Main.tile[num11, num12].TileType = tileType;
                        Main.tile[num11, num12].Clear(TileDataType.Slope);
                    }
                }
            }

            num7 = num4 - 5 - genRand.Next(4);
            num8 = num4;
            num9 = num5 - 3 - genRand.Next(3);
            num10 = num5;
            for (int num13 = num7; num13 < num8; num13++)
            {
                for (int num14 = num9; num14 < num10; num14++)
                {
                    Main.tile[num13, num14].LiquidAmount= 0;
                    if (Main.tile[num13, num14].WallType != wallType)
                    {
                        Main.tile[num13, num14].TileType = tileType;
                        Main.tile[num13, num14].Clear(TileDataType.Slope);
                    }
                }
            }

            int num15 = 1 + genRand.Next(2);
            int num16 = 2 + genRand.Next(4);
            int num17 = 0;
            for (int num18 = num3; num18 < num4; num18++)
            {
                for (int num19 = num5 - num15; num19 < num5; num19++)
                {
                    Main.tile[num18, num19].LiquidAmount= 0;
                    if (Main.tile[num18, num19].WallType != wallType)
                    {
                        Main.tile[num18, num19].TileType = tileType;
                        Main.tile[num18, num19].Clear(TileDataType.Slope);
                    }
                }

                num17++;
                if (num17 >= num16)
                {
                    num18 += num16;
                    num17 = 0;
                }
            }

            for (int num20 = num3; num20 < num4; num20++)
            {
                for (int num21 = num6; (double)num21 < Main.worldSurface; num21++)
                {
                    Main.tile[num20, num21].LiquidAmount = 0;
                    if (!wallTypes.Contains(Main.tile[num20, num21].WallType))
                    {
                        Main.tile[num20, num21].TileType = tileType;
                    }

                    if (num20 > num3 && num20 < num4 - 1)
                        Main.tile[num20, num21].WallType = (ushort)wallType;

                    Main.tile[num20, num21].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.6);
            num4 = (int)(vector2D.X + dxStrength * 0.6);
            num5 = (int)(vector2D.Y - dyStrength * 0.6);
            num6 = (int)(vector2D.Y + dyStrength * 0.6);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num22 = num3; num22 < num4; num22++)
            {
                for (int num23 = num5; num23 < num6; num23++)
                {
                    Main.tile[num22, num23].LiquidAmount= 0;
                    Main.tile[num22, num23].WallType =  (ushort)wallType;
                    Main.tile[num22, num23].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.6 - 1.0);
            num4 = (int)(vector2D.X + dxStrength * 0.6 + 1.0);
            num5 = (int)(vector2D.Y - dyStrength * 0.6 - 1.0);
            num6 = (int)(vector2D.Y + dyStrength * 0.6 + 1.0);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            if (drunkWorldGen)
                num3 -= 4;

            for (int num24 = num3; num24 < num4; num24++)
            {
                for (int num25 = num5; num25 < num6; num25++)
                {
                    Main.tile[num24, num25].LiquidAmount= 0;
                    Main.tile[num24, num25].WallType =  (ushort)wallType;
                    Main.tile[num24, num25].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num5 = (int)(vector2D.Y - dyStrength * 0.5);
            num6 = (int)(vector2D.Y + dyStrength * 0.5);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num26 = num3; num26 < num4; num26++)
            {
                for (int num27 = num5; num27 < num6; num27++)
                {
                    Main.tile[num26, num27].LiquidAmount= 0;
                    Main.tile[num26, num27].WallType =  (ushort)wallType;
                }
            }

            int num28 = (int)vector2D.X;
            int num29 = num6;
            for (int num30 = 0; num30 < 20; num30++)
            {
                num28 = (int)vector2D.X - num30;
                if (!Main.tile[num28, num29].HasTile && wallTypes.Contains(Main.tile[num28, num29].WallType))
                {
                    dungeonPlatformX[numDungeonPlatforms] = num28;
                    dungeonPlatformY[numDungeonPlatforms] = num29;
                    if (numDungeonPlatforms < 499)
                        numDungeonPlatforms++;
                    break;
                }

                num28 = (int)vector2D.X + num30;
                if (!Main.tile[num28, num29].HasTile && wallTypes.Contains(Main.tile[num28, num29].WallType))
                {
                    dungeonPlatformX[numDungeonPlatforms] = num28;
                    dungeonPlatformY[numDungeonPlatforms] = num29;
                    if (numDungeonPlatforms < 499)
                        numDungeonPlatforms++;
                    break;
                }
            }

            vector2D.X += dxStrength * 0.6 * (double)num2;
            vector2D.Y += dyStrength * 0.5;
            dxStrength = dxStrength2;
            dyStrength = dyStrength2;
            vector2D.X += dxStrength * 0.55 * (double)num2;
            vector2D.Y -= dyStrength * 0.5;
            num3 = (int)(vector2D.X - dxStrength * 0.6 - (double)genRand.Next(1, 3));
            num4 = (int)(vector2D.X + dxStrength * 0.6 + (double)genRand.Next(1, 3));
            num5 = (int)(vector2D.Y - dyStrength * 0.6 - (double)genRand.Next(1, 3));
            num6 = (int)(vector2D.Y + dyStrength * 0.6 + (double)genRand.Next(6, 16));
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num31 = num3; num31 < num4; num31++)
            {
                for (int num32 = num5; num32 < num6; num32++)
                {
                    Main.tile[num31, num32].LiquidAmount= 0;
                    if (Main.tile[num31, num32].WallType == wallType)
                        continue;

                    bool flag = true;
                    if (num2 < 0)
                    {
                        if ((double)num31 < vector2D.X - dxStrength * 0.5)
                            flag = false;
                    }
                    else if ((double)num31 > vector2D.X + dxStrength * 0.5 - 1.0)
                    {
                        flag = false;
                    }

                    if (flag)
                    {
                        Main.tile[num31, num32].WallType =  0;
                        Main.tile[num31, num32].TileType = tileType;
                        Main.tile[num31, num32].Clear(TileDataType.Slope);
                    }
                }
            }

            for (int num33 = num3; num33 < num4; num33++)
            {
                for (int num34 = num6; (double)num34 < Main.worldSurface; num34++)
                {
                    Main.tile[num33, num34].LiquidAmount= 0;
                    if (!wallTypes.Contains(Main.tile[num33, num34].WallType))
                    {
                        Main.tile[num33, num34].TileType = tileType;
                    }

                    Main.tile[num33, num34].WallType =  (ushort)wallType;
                    Main.tile[num33, num34].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num7 = num3;
            if (num2 < 0)
                num7++;

            num8 = num7 + 5 + genRand.Next(4);
            num9 = num5 - 3 - genRand.Next(3);
            num10 = num5;
            for (int num35 = num7; num35 < num8; num35++)
            {
                for (int num36 = num9; num36 < num10; num36++)
                {
                    Main.tile[num35, num36].LiquidAmount= 0;
                    if (Main.tile[num35, num36].WallType != wallType)
                    {
                        Main.tile[num35, num36].TileType = tileType;
                        Main.tile[num35, num36].Clear(TileDataType.Slope);
                    }
                }
            }

            num7 = num4 - 5 - genRand.Next(4);
            num8 = num4;
            num9 = num5 - 3 - genRand.Next(3);
            num10 = num5;
            for (int num37 = num7; num37 < num8; num37++)
            {
                for (int num38 = num9; num38 < num10; num38++)
                {
                    Main.tile[num37, num38].LiquidAmount= 0;
                    if (Main.tile[num37, num38].WallType != wallType)
                    {
                        Main.tile[num37, num38].TileType = tileType;
                        Main.tile[num37, num38].Clear(TileDataType.Slope);
                    }
                }
            }

            num15 = 1 + genRand.Next(2);
            num16 = 2 + genRand.Next(4);
            num17 = 0;
            if (num2 < 0)
                num4++;

            for (int num39 = num3 + 1; num39 < num4 - 1; num39++)
            {
                for (int num40 = num5 - num15; num40 < num5; num40++)
                {
                    Main.tile[num39, num40].LiquidAmount= 0;
                    if (Main.tile[num39, num40].WallType != wallType)
                    {
                        Main.tile[num39, num40].TileType = tileType;
                        Main.tile[num39, num40].Clear(TileDataType.Slope);
                    }
                }

                num17++;
                if (num17 >= num16)
                {
                    num39 += num16;
                    num17 = 0;
                }
            }

            if (!drunkWorldGen)
            {
                num3 = (int)(vector2D.X - dxStrength * 0.6);
                num4 = (int)(vector2D.X + dxStrength * 0.6);
                num5 = (int)(vector2D.Y - dyStrength * 0.6);
                num6 = (int)(vector2D.Y + dyStrength * 0.6);
                if (num3 < 0)
                    num3 = 0;

                if (num4 > Main.maxTilesX)
                    num4 = Main.maxTilesX;

                if (num5 < 0)
                    num5 = 0;

                if (num6 > Main.maxTilesY)
                    num6 = Main.maxTilesY;

                for (int num41 = num3; num41 < num4; num41++)
                {
                    for (int num42 = num5; num42 < num6; num42++)
                    {
                        Main.tile[num41, num42].LiquidAmount= 0;
                        Main.tile[num41, num42].WallType =  0;
                    }
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num5 = (int)(vector2D.Y - dyStrength * 0.5);
            num6 = (int)(vector2D.Y + dyStrength * 0.5);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num43 = num3; num43 < num4; num43++)
            {
                for (int num44 = num5; num44 < num6; num44++)
                {
                    Main.tile[num43, num44].LiquidAmount= 0;
                    Main.tile[num43, num44].WallType =  0;
                }
            }

            Main.dungeonX = (int)vector2D.X;
            Main.dungeonY = num6;
            int num45 = NPC.NewNPC(new EntitySource_WorldGen(), Main.dungeonX * 16 + 8, Main.dungeonY * 16, 37);
            Main.npc[num45].homeless = false;
            Main.npc[num45].homeTileX = Main.dungeonX;
            Main.npc[num45].homeTileY = Main.dungeonY;

            if (!drunkWorldGen)
            {
                int num47 = 100;
                if (num2 == 1)
                {
                    int num48 = 0;
                    for (int num49 = num4; num49 < num4 + num47; num49++)
                    {
                        num48++;
                        for (int num50 = num6 + num48; num50 < num6 + num47; num50++)
                        {
                            Main.tile[num49, num50].LiquidAmount= 0;
                            Main.tile[num49, num50 - 1].LiquidAmount= 0;
                            Main.tile[num49, num50 - 2].LiquidAmount= 0;
                            Main.tile[num49, num50 - 3].LiquidAmount= 0;
                            if (!wallTypes.Contains(Main.tile[num49, num50].WallType) && Main.tile[num49, num50].WallType != 3 && Main.tile[num49, num50].WallType != 83)
                            {
                                Main.tile[num49, num50].TileType = tileType;
                                Main.tile[num49, num50].Clear(TileDataType.Slope);
                            }
                        }
                    }
                }
                else
                {
                    int num51 = 0;
                    for (int num52 = num3; num52 > num3 - num47; num52--)
                    {
                        num51++;
                        for (int num53 = num6 + num51; num53 < num6 + num47; num53++)
                        {
                            Main.tile[num52, num53].LiquidAmount= 0;
                            Main.tile[num52, num53 - 1].LiquidAmount= 0;
                            Main.tile[num52, num53 - 2].LiquidAmount= 0;
                            Main.tile[num52, num53 - 3].LiquidAmount= 0;
                            if (!wallTypes.Contains(Main.tile[num52, num53].WallType) && Main.tile[num52, num53].WallType != 3 && Main.tile[num52, num53].WallType != 83)
                            {
                                Main.tile[num52, num53].TileType = tileType;
                                Main.tile[num52, num53].Clear(TileDataType.Slope);
                            }
                        }
                    }
                }
            }

            num15 = 1 + genRand.Next(2);
            num16 = 2 + genRand.Next(4);
            num17 = 0;
            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            if (drunkWorldGen)
            {
                if (num2 == 1)
                {
                    num4--;
                    num3--;
                }
                else
                {
                    num3++;
                    num4++;
                }
            }
            else
            {
                num3 += 2;
                num4 -= 2;
            }

            for (int num54 = num3; num54 < num4; num54++)
            {
                for (int num55 = num5; num55 < num6 + 1; num55++)
                {
                    PlaceWall(num54, num55, wallType, mute: true);
                }

                if (!drunkWorldGen)
                {
                    num17++;
                    if (num17 >= num16)
                    {
                        num54 += num16 * 2;
                        num17 = 0;
                    }
                }
            }

            if (drunkWorldGen)
            {
                num3 = (int)(vector2D.X - dxStrength * 0.5);
                num4 = (int)(vector2D.X + dxStrength * 0.5);
                if (num2 == 1)
                    num3 = num4 - 3;
                else
                    num4 = num3 + 3;

                for (int num56 = num3; num56 < num4; num56++)
                {
                    for (int num57 = num5; num57 < num6 + 1; num57++)
                    {
                        Main.tile[num56, num57].TileType = tileType;
                        Main.tile[num56, num57].Clear(TileDataType.Slope);
                    }
                }
            }

            vector2D.X -= dxStrength * 0.6 * (double)num2;
            vector2D.Y += dyStrength * 0.5;
            dxStrength = 15.0;
            dyStrength = 3.0;
            vector2D.Y -= dyStrength * 0.5;
            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num5 = (int)(vector2D.Y - dyStrength * 0.5);
            num6 = (int)(vector2D.Y + dyStrength * 0.5);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num58 = num3; num58 < num4; num58++)
            {
                for (int num59 = num5; num59 < num6; num59++)
                {
                    WorldUtils.ClearTile(num58, num59);
                }
            }

            if (num2 < 0)
                vector2D.X -= 1.0;

            PlaceTile((int)vector2D.X, (int)vector2D.Y + 1, 10, mute: true, forced: false, -1, 13);
        }

        public void DungeonHalls(int i, int j, ushort tileType, int wallType, bool forceX = false)
        {
            Vector2D zero = Vector2D.Zero;
            double num = genRand.Next(4, 6);
            double num2 = num;
            Vector2D zero2 = Vector2D.Zero;
            Vector2D zero3 = Vector2D.Zero;
            int num3 = 1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = j;
            int num4 = genRand.Next(35, 80);
            bool flag = false;
            if (genRand.NextBool(6))
                flag = true;

            if (forceX)
            {
                num4 += 20;
                lastDungeonHall = Vector2D.Zero;
            }
            else if (genRand.NextBool(5))
            {
                num *= 2.0;
                num4 /= 2;
            }

            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = true;
            bool flag5 = false;
            while (!flag2)
            {
                flag5 = false;
                if (flag4 && !forceX)
                {
                    bool flag6 = true;
                    bool flag7 = true;
                    bool flag8 = true;
                    bool flag9 = true;
                    int num5 = num4;
                    bool flag10 = false;
                    for (int num6 = j; num6 > j - num5; num6--)
                    {
                        if (Main.tile[i, num6].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag6 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int k = j; k < j + num5; k++)
                    {
                        if (Main.tile[i, k].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag7 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int num7 = i; num7 > i - num5; num7--)
                    {
                        if (Main.tile[num7, j].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag8 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int l = i; l < i + num5; l++)
                    {
                        if (Main.tile[l, j].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag9 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    if (!flag8 && !flag9 && !flag6 && !flag7)
                    {
                        num3 = ((!genRand.NextBool(2)) ? 1 : (-1));
                        if (genRand.NextBool(2))
                            flag5 = true;
                    }
                    else
                    {
                        int num8 = genRand.Next(4);
                        do
                        {
                            num8 = genRand.Next(4);
                        } while (!(num8 == 0 && flag6) && !(num8 == 1 && flag7) && !(num8 == 2 && flag8) && !(num8 == 3 && flag9));

                        switch (num8)
                        {
                            case 0:
                                num3 = -1;
                                break;
                            case 1:
                                num3 = 1;
                                break;
                            default:
                                flag5 = true;
                                num3 = ((num8 != 2) ? 1 : (-1));
                                break;
                        }
                    }
                }
                else
                {
                    num3 = ((!genRand.NextBool(2)) ? 1 : (-1));
                    if (genRand.NextBool(2))
                        flag5 = true;
                }

                flag4 = false;
                if (forceX)
                    flag5 = true;

                if (flag5)
                {
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero3.Y = 0.0;
                    zero3.X = -num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (genRand.NextBool(3))
                    {
                        if (genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else
                {
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    zero3.X = 0.0;
                    zero3.Y = -num3;
                    if (!genRand.NextBool(3))
                    {
                        flag3 = true;
                        if (genRand.NextBool(2))
                            zero.X = (double)genRand.Next(10, 20) * 0.1;
                        else
                            zero.X = (double)(-genRand.Next(10, 20)) * 0.1;
                    }
                    else if (genRand.NextBool(2))
                    {
                        if (genRand.NextBool(2))
                            zero.X = (double)genRand.Next(20, 40) * 0.01;
                        else
                            zero.X = (double)(-genRand.Next(20, 40)) * 0.01;
                    }
                    else
                    {
                        num4 /= 2;
                    }
                }

                if (lastDungeonHall != zero3)
                    flag2 = true;
            }

            int num9 = 0;
            bool flag11 = vector2D.Y < Main.rockLayer + 100.0;
            if (remixWorldGen)
                flag11 = vector2D.Y < Main.worldSurface + 100.0;

            if (!forceX)
            {
                if (vector2D.X > (double)(lastMaxTilesX - 200))
                {
                    num3 = -1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (genRand.NextBool(3))
                    {
                        if (genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.X < 200.0)
                {
                    num3 = 1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (genRand.NextBool(3))
                    {
                        if (genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.Y > (double)(lastMaxTilesY - 300))
                {
                    num3 = -1;
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    if (genRand.NextBool(2))
                    {
                        if (genRand.NextBool(2))
                            zero.X = (double)genRand.Next(20, 50) * 0.01;
                        else
                            zero.X = (double)(-genRand.Next(20, 50)) * 0.01;
                    }
                }
                else if (flag11)
                {
                    num3 = 1;
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    if (!genRand.NextBool(3))
                    {
                        flag3 = true;
                        if (genRand.NextBool(2))
                            zero.X = (double)genRand.Next(10, 20) * 0.1;
                        else
                            zero.X = (double)(-genRand.Next(10, 20)) * 0.1;
                    }
                    else if (genRand.NextBool(2))
                    {
                        if (genRand.NextBool(2))
                            zero.X = (double)genRand.Next(20, 50) * 0.01;
                        else
                            zero.X = (double)genRand.Next(20, 50) * 0.01;
                    }
                }
                else if (vector2D.X < (double)(Main.maxTilesX / 2) && vector2D.X > (double)Main.maxTilesX * 0.25)
                {
                    num3 = -1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (genRand.NextBool(3))
                    {
                        if (genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.X > (double)(Main.maxTilesX / 2) && vector2D.X < (double)Main.maxTilesX * 0.75)
                {
                    num3 = 1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (genRand.NextBool(3))
                    {
                        if (genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
            }

            if (zero2.Y == 0.0)
            {
                DDoorX[numDDoors] = (int)vector2D.X;
                DDoorY[numDDoors] = (int)vector2D.Y;
                DDoorPos[numDDoors] = 0;
                if (numDDoors < 499)
                    numDDoors++;
            }
            else
            {
                dungeonPlatformX[numDungeonPlatforms] = (int)vector2D.X;
                dungeonPlatformY[numDungeonPlatforms] = (int)vector2D.Y;
                if (numDungeonPlatforms < 499)
                    numDungeonPlatforms++;
            }

            lastDungeonHall = zero2;
            if (Math.Abs(zero.X) > Math.Abs(zero.Y) && !genRand.NextBool(3))
                num = (int)(num2 * ((double)genRand.Next(110, 150) * 0.01));

            while (num4 > 0)
            {
                num9++;
                if (zero2.X > 0.0 && vector2D.X > (double)(Main.maxTilesX - 100))
                    num4 = 0;
                else if (zero2.X < 0.0 && vector2D.X < 100.0)
                    num4 = 0;
                else if (zero2.Y > 0.0 && vector2D.Y > (double)(Main.maxTilesY - 100))
                    num4 = 0;
                else if (remixWorldGen && zero2.Y < 0.0 && vector2D.Y < (Main.rockLayer + Main.worldSurface) / 2.0)
                    num4 = 0;
                else if (!remixWorldGen && zero2.Y < 0.0 && vector2D.Y < Main.rockLayer + 50.0)
                    num4 = 0;

                num4--;
                int num10 = (int)(vector2D.X - num - 4.0 - (double)genRand.Next(6));
                int num11 = (int)(vector2D.X + num + 4.0 + (double)genRand.Next(6));
                int num12 = (int)(vector2D.Y - num - 4.0 - (double)genRand.Next(6));
                int num13 = (int)(vector2D.Y + num + 4.0 + (double)genRand.Next(6));
                if (num10 < 0)
                    num10 = 0;

                if (num11 > Main.maxTilesX)
                    num11 = Main.maxTilesX;

                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesY)
                    num13 = Main.maxTilesY;

                for (int m = num10; m < num11; m++)
                {
                    for (int n = num12; n < num13; n++)
                    {
                        if (m < dMinX)
                            dMinX = m;

                        if (m > dMaxX)
                            dMaxX = m;

                        if (n > dMaxY)
                            dMaxY = n;

                        Main.tile[m, n].LiquidAmount= 0;
                        if (!wallTypes.Contains(Main.tile[m, n].WallType))
                        {
                            Main.tile[m, n].TileType = tileType;
                            Main.tile[m, n].Clear(TileDataType.Slope);
                        }
                    }
                }

                for (int num14 = num10 + 1; num14 < num11 - 1; num14++)
                {
                    for (int num15 = num12 + 1; num15 < num13 - 1; num15++)
                    {
                        Main.tile[num14, num15].WallType =  (ushort)wallType;
                    }
                }

                int num16 = 0;
                if (zero.Y == 0.0 && genRand.NextBool((int)num + 1))
                    num16 = genRand.Next(1, 3);
                else if (zero.X == 0.0 && genRand.NextBool((int)num - 1))
                    num16 = genRand.Next(1, 3);
                else if (genRand.NextBool((int)num * 3))
                    num16 = genRand.Next(1, 3);

                num10 = (int)(vector2D.X - num * 0.5 - (double)num16);
                num11 = (int)(vector2D.X + num * 0.5 + (double)num16);
                num12 = (int)(vector2D.Y - num * 0.5 - (double)num16);
                num13 = (int)(vector2D.Y + num * 0.5 + (double)num16);
                if (num10 < 0)
                    num10 = 0;

                if (num11 > Main.maxTilesX)
                    num11 = Main.maxTilesX;

                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesY)
                    num13 = Main.maxTilesY;

                for (int num17 = num10; num17 < num11; num17++)
                {
                    for (int num18 = num12; num18 < num13; num18++)
                    {
                        Main.tile[num17, num18].Clear(TileDataType.Slope);
                        if (flag)
                        {
                            if (Main.tile[num17, num18].HasTile || Main.tile[num17, num18].WallType != wallType)
                            {
                                Main.tile[num17, num18].TileType = tileType;
                            }
                        }
                        else
                        {
                            WorldUtils.ClearTile(num17, num18);
                        }

                        Main.tile[num17, num18].Clear(TileDataType.Slope);
                        Main.tile[num17, num18].WallType =  (ushort)wallType;
                    }
                }

                vector2D += zero;
                if (flag3 && num9 > genRand.Next(10, 20))
                {
                    num9 = 0;
                    zero.X *= -1.0;
                }
            }

            dungeonX = (int)vector2D.X;
            dungeonY = (int)vector2D.Y;
            if (zero2.Y == 0.0)
            {
                DDoorX[numDDoors] = (int)vector2D.X;
                DDoorY[numDDoors] = (int)vector2D.Y;
                DDoorPos[numDDoors] = 0;
                if (numDDoors < 499)
                    numDDoors++;
            }
            else
            {
                dungeonPlatformX[numDungeonPlatforms] = (int)vector2D.X;
                dungeonPlatformY[numDungeonPlatforms] = (int)vector2D.Y;
                if (numDungeonPlatforms < 499)
                    numDungeonPlatforms++;
            }
        }

        public void DungeonStairs(int i, int j, ushort tileType, int wallType)
        {
            Vector2D zero = Vector2D.Zero;
            double num = genRand.Next(5, 9);
            int num2 = 1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = j;
            int num3 = genRand.Next(10, 30);
            num2 = ((i <= dEntranceX) ? 1 : (-1));
            if (i > Main.maxTilesX - 400)
                num2 = -1;
            else if (i < 400)
                num2 = 1;

            zero.Y = -1.0;
            zero.X = num2;
            if (!genRand.NextBool(3))
                zero.X *= 1.0 + (double)genRand.Next(0, 200) * 0.01;
            else if (genRand.NextBool(3))
                zero.X *= (double)genRand.Next(50, 76) * 0.01;
            else if (genRand.NextBool(6))
                zero.Y *= 2.0;

            if (dungeonX < Main.maxTilesX / 2 && zero.X < 0.0 && zero.X < 0.5)
                zero.X = -0.5;

            if (dungeonX > Main.maxTilesX / 2 && zero.X > 0.0 && zero.X > 0.5)
                zero.X = -0.5;

            if (drunkWorldGen)
            {
                num2 *= -1;
                zero.X *= -1.0;
            }

            while (num3 > 0)
            {
                num3--;
                int num4 = (int)(vector2D.X - num - 4.0 - (double)genRand.Next(6));
                int num5 = (int)(vector2D.X + num + 4.0 + (double)genRand.Next(6));
                int num6 = (int)(vector2D.Y - num - 4.0);
                int num7 = (int)(vector2D.Y + num + 4.0 + (double)genRand.Next(6));
                if (num4 < 0)
                    num4 = 0;

                if (num5 > Main.maxTilesX)
                    num5 = Main.maxTilesX;

                if (num6 < 0)
                    num6 = 0;

                if (num7 > Main.maxTilesY)
                    num7 = Main.maxTilesY;

                int num8 = 1;
                if (vector2D.X > (double)(Main.maxTilesX / 2))
                    num8 = -1;

                int num9 = (int)(vector2D.X + dxStrength1 * 0.6 * (double)num8 + dxStrength2 * (double)num8);
                int num10 = (int)(dyStrength2 * 0.5);
                if (vector2D.Y < Main.worldSurface - 5.0 && Main.tile[num9, (int)(vector2D.Y - num - 6.0 + (double)num10)].WallType == 0 && Main.tile[num9, (int)(vector2D.Y - num - 7.0 + (double)num10)].WallType == 0 && Main.tile[num9, (int)(vector2D.Y - num - 8.0 + (double)num10)].WallType == 0)
                {
                    dSurface = true;
                    TileRunner(num9, (int)(vector2D.Y - num - 6.0 + (double)num10), genRand.Next(25, 35), genRand.Next(10, 20), -1, addTile: false, 0.0, -1.0);
                }

                for (int k = num4; k < num5; k++)
                {
                    for (int l = num6; l < num7; l++)
                    {
                        Main.tile[k, l].LiquidAmount= 0;
                        if (!wallTypes.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType =  0;
                            Main.tile[k, l].TileType = tileType;
                        }
                    }
                }

                for (int m = num4 + 1; m < num5 - 1; m++)
                {
                    for (int n = num6 + 1; n < num7 - 1; n++)
                    {
                        Main.tile[m, n].WallType =  (ushort)wallType;
                    }
                }

                int num11 = 0;
                if (genRand.NextBool((int)num))
                    num11 = genRand.Next(1, 3);

                num4 = (int)(vector2D.X - num * 0.5 - (double)num11);
                num5 = (int)(vector2D.X + num * 0.5 + (double)num11);
                num6 = (int)(vector2D.Y - num * 0.5 - (double)num11);
                num7 = (int)(vector2D.Y + num * 0.5 + (double)num11);
                if (num4 < 0)
                    num4 = 0;

                if (num5 > Main.maxTilesX)
                    num5 = Main.maxTilesX;

                if (num6 < 0)
                    num6 = 0;

                if (num7 > Main.maxTilesY)
                    num7 = Main.maxTilesY;

                for (int num12 = num4; num12 < num5; num12++)
                {
                    for (int num13 = num6; num13 < num7; num13++)
                    {
                        PlaceWall(num12, num13, wallType, mute: true);
                    }
                }

                if (dSurface)
                    num3 = 0;

                vector2D += zero;
                if (vector2D.Y < Main.worldSurface)
                    zero.Y *= 0.98;
            }

            dungeonX = (int)vector2D.X;
            dungeonY = (int)vector2D.Y;
        }
    }
}
