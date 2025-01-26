using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.DataStructures
{
    /// <summary>
    /// Data structure that lets you analyze Vanilla and Macrocosm SceneMetrics at any position, even on the server
    /// <br/> <b>Includes</b> Vanilla and Macrocosm biome-related (Tile counts) and proximity buff (Banners, Campfire, etc.) info 
    /// <br/> <b>Excludes</b> modded tile NearbyEffects, use <see cref="Hooks"/> if your tile has custom logic that can modify this <see cref="SceneData"/> instance
    /// <br/> <b>Excludes</b> visual data such as Monoliths or Music Boxes
    /// </summary>
    public class SceneData
    {
        /// <summary> Allows this tile to modify some of the properties of the SceneData instance </summary>
        public static Action<int, int, SceneData>[] Hooks { get; set; } = TileID.Sets.Factory.CreateCustomSet<Action<int, int, SceneData>>(null);

        private Vector2 scanCenterWorldCoordinates;

        public TileCounts Macrocosm => macrocosmTileCounts;
        private readonly TileCounts macrocosmTileCounts;

        public Vector2 Position => scanCenterWorldCoordinates;
        public Point TilePosition => scanCenterWorldCoordinates.ToTileCoordinates();

        public Rectangle ScanArea => WorldUtils.ClampToWorld(new Rectangle(TilePosition.X - Main.buffScanAreaWidth / 2, TilePosition.Y - Main.buffScanAreaHeight / 2, Main.buffScanAreaWidth, Main.buffScanAreaHeight));

        public bool ZoneUnderworldHeight => TilePosition.Y > Main.UnderworldLayer;
        public bool ZoneRockLayerHeight => TilePosition.Y <= Main.UnderworldLayer && TilePosition.Y > Main.rockLayer;
        public bool ZoneDirtLayerHeight => TilePosition.Y <= Main.rockLayer && TilePosition.Y > Main.worldSurface;
        public bool ZoneOverworldHeight => TilePosition.Y <= Main.worldSurface && TilePosition.Y > Main.worldSurface * 0.3499999940395355;
        public bool ZoneSkyHeight => TilePosition.Y <= Main.worldSurface * 0.3499999940395355;
        public bool ZoneBeach => WorldGen.oceanDepths(TilePosition.X, TilePosition.Y);
        public bool ZoneRain => Main.raining && TilePosition.Y <= Main.worldSurface;

        public bool ZoneShimmer => ShimmerTileCount >= ShimmerTileThreshold;
        public bool ZoneCorrupt => EvilTileCount >= CorruptionTileThreshold;
        public bool ZoneCrimson => BloodTileCount >= CrimsonTileThreshold;
        public bool ZoneHallow => HolyTileCount >= HallowTileThreshold;
        public bool ZoneJungle => JungleTileCount >= JungleTileThreshold && Position.Y / 16f < Main.UnderworldLayer;
        public bool ZoneSnow => SnowTileCount >= SnowTileThreshold;
        public bool ZoneDesert => SandTileCount >= DesertTileThreshold;
        public bool ZoneGlowshroom => MushroomTileCount >= MushroomTileThreshold;
        public bool ZoneMeteor => MeteorTileCount >= MeteorTileThreshold;
        public bool ZoneGraveyard => GraveyardTileCount >= GraveyardTileThreshold;

        public bool ZoneWaterCandle => WaterCandleCount > 0;
        public bool ZonePeaceCandle => PeaceCandleCount > 0;
        public bool ZoneShadowCandle => ShadowCandleCount > 0;

        public int ShimmerTileCount { get; set; }
        public int EvilTileCount { get; set; }
        public int BloodTileCount { get; set; }
        public int HolyTileCount { get; set; }
        public int HoneyBlockCount { get; set; }
        public int SandTileCount { get; set; }
        public int MushroomTileCount { get; set; }
        public int SnowTileCount { get; set; }
        public int DungeonTileCount { get; set; }
        public int JungleTileCount { get; set; }
        public int MeteorTileCount { get; set; }
        public int GraveyardTileCount { get; set; }

        public int WaterCandleCount { get; set; }
        public int PeaceCandleCount { get; set; }
        public int ShadowCandleCount { get; set; }

        public bool HasSunflower { get; set; }
        public bool HasGardenGnome { get; set; }
        public bool HasClock { get; set; }
        public bool HasCampfire { get; set; }
        public bool HasStarInBottle { get; set; }
        public bool HasHeartLantern { get; set; }
        public bool HasCatBast { get; set; }


        public const int ShimmerTileThreshold = 300;
        public const int CorruptionTileThreshold = 300;
        public const int CorruptionTileMax = 1000;
        public const int CrimsonTileThreshold = 300;
        public const int CrimsonTileMax = 1000;
        public const int HallowTileThreshold = 125;
        public const int HallowTileMax = 600;
        public const int JungleTileThreshold = 140;
        public const int JungleTileMax = 700;
        public const int SnowTileThreshold = 1500;
        public const int SnowTileMax = 6000;
        public const int DesertTileThreshold = 1500;
        public const int MushroomTileThreshold = 100;
        public const int MushroomTileMax = 160;
        public const int MeteorTileThreshold = 75;
        public const int GraveyardTileMax = 36;
        public const int GraveyardTileMin = 16;
        public const int GraveyardTileThreshold = 28;

        public bool[] NPCBannerBuff = new bool[290];
        public bool HasBanner;

        private readonly int[] tileCounts = new int[TileLoader.TileCount];
        private readonly int[] liquidCounts = new int[LiquidID.Count];

        public SceneData(Vector2 scanCenterWorldCoordinates)
        {
            this.scanCenterWorldCoordinates = scanCenterWorldCoordinates;
            macrocosmTileCounts = new();
            Scan();
        }
        public SceneData(Point TilePosition) : this(TilePosition.ToWorldCoordinates()) { }
        public SceneData(Point16 TilePosition) : this(TilePosition.ToPoint()) { }


        public int GetTileCount(ushort tileId) => tileCounts[tileId];
        public int GetLiquidCount(short liquidType) => liquidCounts[liquidType];

        public void Scan(Vector2 scanCenterWorldCoordinates)
        {
            this.scanCenterWorldCoordinates = scanCenterWorldCoordinates;
            Scan();
        }

        public void Scan()
        {
            Reset();

            for (int i = ScanArea.Left; i < ScanArea.Right; i++)
            {
                for (int j = ScanArea.Top; j < ScanArea.Bottom; j++)
                {
                    if (!ScanArea.Contains(i, j))
                        continue;

                    Tile tile = Main.tile[i, j];
                    if (tile == null)
                        continue;

                    if (!tile.HasTile)
                    {
                        if (tile.LiquidAmount > 0)
                            liquidCounts[tile.LiquidType]++;

                        continue;
                    }

                    if (!TileID.Sets.isDesertBiomeSand[tile.TileType] || !WorldGen.oceanDepths(i, j))
                        tileCounts[tile.TileType]++;

                    if (tile.TileType == TileID.Campfire && tile.TileFrameY < 36)
                        HasCampfire = true;

                    if (tile.TileType == TileID.WaterCandle && tile.TileFrameX < 18)
                        WaterCandleCount++;

                    if (tile.TileType == TileID.PeaceCandle && tile.TileFrameX < 18)
                        PeaceCandleCount++;

                    if (tile.TileType == TileID.ShadowCandle && tile.TileFrameX < 18)
                        ShadowCandleCount++;

                    if (tile.TileType == TileID.Fireplace && tile.TileFrameX < 54)
                        HasCampfire = true;

                    if (tile.TileType == TileID.CatBast && tile.TileFrameX < 72)
                        HasCatBast = true;

                    if (tile.TileType == TileID.HangingLanterns && tile.TileFrameY >= 324 && tile.TileFrameY <= 358)
                        HasHeartLantern = true;

                    if (tile.TileType == TileID.HangingLanterns && tile.TileFrameY >= 252 && tile.TileFrameY <= 286)
                        HasStarInBottle = true;

                    if (tile.TileType == TileID.Banners && (tile.TileFrameX >= 396 || tile.TileFrameY >= 54))
                    {
                        int bannerID = tile.TileFrameX / 18 - 21;
                        for (int k = tile.TileFrameY; k >= 54; k -= 54)
                        {
                            bannerID += 90;
                            bannerID += 21;
                        }

                        int bannerItemType = Item.BannerToItem(bannerID);
                        if (ItemID.Sets.BannerStrength.IndexInRange(bannerItemType) && ItemID.Sets.BannerStrength[bannerItemType].Enabled)
                        {
                            NPCBannerBuff[bannerID] = true;
                            HasBanner = true;
                        }
                    }

                    Hooks[tile.TileType]?.Invoke(i, j, this);
                }
            }

            if (tileCounts[TileID.Sunflower] > 0)
                HasSunflower = true;

            if (tileCounts[TileID.GardenGnome] > 0)
                HasGardenGnome = true;

            ShimmerTileCount = liquidCounts[LiquidID.Shimmer];
            HoneyBlockCount = tileCounts[TileID.HoneyBlock];

            MeteorTileCount = tileCounts[TileID.Meteorite];

            GraveyardTileCount = tileCounts[TileID.Tombstones];
            GraveyardTileCount -= tileCounts[TileID.Sunflower] / 2;

            // Loop through all tiles, skipping ones not onscreen, and add each to the biome tile counts from their respective sets
            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                int tileCount = tileCounts[i];

                if (tileCount == 0)
                    continue;

                HolyTileCount += tileCount * TileID.Sets.HallowBiome[i];
                SnowTileCount += tileCount * TileID.Sets.SnowBiome[i];
                MushroomTileCount += tileCount * TileID.Sets.MushroomBiome[i];
                SandTileCount += tileCount * TileID.Sets.SandBiome[i];
                DungeonTileCount += tileCount * TileID.Sets.DungeonBiome[i];

                int crimson, corrupt, jungle;

                // Handles if the world is using the remix seed or not, which slightly changes which blocks are counted
                if (!Main.remixWorld)
                {
                    corrupt = TileID.Sets.CorruptBiome[i];
                    crimson = TileID.Sets.CrimsonBiome[i];
                    jungle = TileID.Sets.JungleBiome[i];
                }
                else
                {
                    corrupt = TileID.Sets.RemixCorruptBiome[i];
                    crimson = TileID.Sets.RemixCrimsonBiome[i];
                    jungle = TileID.Sets.RemixJungleBiome[i];
                }

                EvilTileCount += tileCount * corrupt;
                BloodTileCount += tileCount * crimson;
                JungleTileCount += tileCount * jungle;
            }

            if (tileCounts[27] > 0)
                HasSunflower = true;

            if (GraveyardTileCount > GraveyardTileMin)
                HasSunflower = false;

            if (GraveyardTileCount < 0)
                GraveyardTileCount = 0;

            if (HolyTileCount < 0)
                HolyTileCount = 0;

            if (EvilTileCount < 0)
                EvilTileCount = 0;

            if (BloodTileCount < 0)
                BloodTileCount = 0;

            int holyTileCount = HolyTileCount;
            HolyTileCount -= EvilTileCount;
            HolyTileCount -= BloodTileCount;
            EvilTileCount -= holyTileCount;
            BloodTileCount -= holyTileCount;

            if (HolyTileCount < 0)
                HolyTileCount = 0;

            if (EvilTileCount < 0)
                EvilTileCount = 0;

            if (BloodTileCount < 0)
                BloodTileCount = 0;

            macrocosmTileCounts.TileCountsAvailable(tileCounts);

            GraveyardTileCount = macrocosmTileCounts.GetModifiedGraveyardTileCount(GraveyardTileCount);
        }

        private void Reset()
        {
            Array.Clear(tileCounts, 0, tileCounts.Length);
            Array.Clear(liquidCounts, 0, liquidCounts.Length);

            SandTileCount = 0;
            EvilTileCount = 0;
            BloodTileCount = 0;
            GraveyardTileCount = 0;
            MushroomTileCount = 0;
            SnowTileCount = 0;
            HolyTileCount = 0;
            MeteorTileCount = 0;
            JungleTileCount = 0;
            DungeonTileCount = 0;

            WaterCandleCount = 0;
            PeaceCandleCount = 0;
            ShadowCandleCount = 0;

            HasCampfire = false;
            HasSunflower = false;
            HasGardenGnome = false;
            HasStarInBottle = false;
            HasHeartLantern = false;
            HasClock = false;
            HasCatBast = false;

            Array.Clear(NPCBannerBuff, 0, NPCBannerBuff.Length);
            HasBanner = false;

            if (NPCBannerBuff.Length < NPCLoader.NPCCount)
                Array.Resize(ref NPCBannerBuff, NPCLoader.NPCCount);

            macrocosmTileCounts.ResetNearbyTileEffects();
        }
    }
}
