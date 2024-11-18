using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    /// <summary> 
    /// General system, mainly for managing <see cref="MacrocosmSubworld"/>s, and sometimes as an entry point of other systems.
    /// <br/> Persistent world flags such as downed bosses should go in <see cref="WorldFlags"/>
    /// </summary>
    class MacrocosmWorld : ModSystem
    {
        /// <summary> Whether the dusk time boundary happened in this update tick </summary>
        public static bool IsDusk { get; set; } = false;

        /// <summary> Whether the dawn time boundary happened in this update tick </summary>
        public static bool IsDawn { get; set; } = false;

        public static int Seed => Main.ActiveWorldFileData.Seed;
        public static string SeedText => Main.ActiveWorldFileData.SeedText;
        public override void OnWorldLoad()
        {
            if (!SubworldSystem.AnyActive<Macrocosm>())
                Earth.WorldSize = WorldSize.Current;
        }

        private static void RandomTileUpdate()
        {
            MacrocosmSubworld current = MacrocosmSubworld.Current;
            if (current is null)
                return;

            double worldUpdateRate = WorldGen.GetWorldUpdateRate();
            if (worldUpdateRate == 0)
                return;

            double overgroundUpdateRate = 3E-05f * (float)worldUpdateRate;
            double undergroundUpdateRate = 1.5E-05f * (float)worldUpdateRate;
            //double gfbUpdateRate = 2.5E-05f * (float)worldUpdateRate;

            double overgroundTileUpdateNumber = Main.maxTilesX * Main.maxTilesY * overgroundUpdateRate;
            double undergroundTileUpdateNumber = Main.maxTilesX * Main.maxTilesY * undergroundUpdateRate;

            //int plantUpdateChance = (int)Terraria.Utils.Lerp(151, 151 * 2.8, Terraria.Utils.Clamp(Main.maxTilesX / 4200.0 - 1.0, 0.0, 1.0));

            for (int k = 0; k < overgroundTileUpdateNumber; k++)
            {
                //if (Main.rand.NextBool(plantUpdateChance * 100))
                //    WorldGen.PlantAlch();

                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);

                TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
                WallLoader.RandomUpdate(i, j, Main.tile[i, j].WallType);
                current.RandomTileUpdate(i, j, underground: false);
                TownNPCSystem.TrySpawningTownNPC(i, j);
            }

            for (int k = 0; k < undergroundTileUpdateNumber; k++)
            {
                int i = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);
                current.RandomTileUpdate(i, j, underground: true);
                TownNPCSystem.TrySpawningTownNPC(i, j);
            }
        }

        #region ModSystem update hooks
        public override void PreUpdateEntities()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateEntities();
        }

        public override void PreUpdatePlayers()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdatePlayers();
        }

        public override void PostUpdatePlayers()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdatePlayers();
        }

        public override void PreUpdateNPCs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateNPCs();
        }

        public override void PostUpdateNPCs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateNPCs();
        }

        public override void PreUpdateGores()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateGores();
        }

        public override void PostUpdateGores()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateGores();
        }

        public override void PreUpdateProjectiles()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateProjectiles();
        }

        public override void PostUpdateProjectiles()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateProjectiles();
        }

        public override void PreUpdateItems()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateItems();
        }

        public override void PostUpdateItems()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateItems();
        }

        public override void PreUpdateDusts()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateDusts();
        }

        public override void PostUpdateDusts()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateDusts();
        }

        public override void PreUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                MacrocosmSubworld.Current.PreUpdateTime();
            }
            else
            {
                // Set these flags on Earth
                IsDusk = Main.dayTime && Main.time >= Main.dayLength;
                IsDawn = !Main.dayTime && Main.time >= Main.nightLength;
            }
        }

        public override void PostUpdateTime()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateTime();
        }

        public override void PreUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateWorld();
        }

        public override void PostUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                TownNPCSystem.UpdateTownNPCSpawns();
                RandomTileUpdate();

                MacrocosmSubworld.Current.PostUpdateWorld();
            }
        }

        public override void PreUpdateInvasions()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PreUpdateInvasions();
        }

        public override void PostUpdateInvasions()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateInvasions();
        }

        public override void PostUpdateEverything()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                MacrocosmSubworld.Current.PostUpdateEverything();
        }

        #endregion

        // Send current main world GUID and ask player to travel to last known subworld  
        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.LastSubworldCheck);
                Guid guid = MacrocosmSubworld.MainWorldUniqueID;
                packet.Write(guid.ToString());
                packet.Send(remoteClient);
            }

            return false;
        }
    }
}
