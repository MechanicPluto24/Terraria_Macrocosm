using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    /// <summary> Global NPC for general NPC modifications (loot, spawn pools) </summary>
    public class SpawnGlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!SubworldSystem.AnyActive<Macrocosm>())
                return;

            bool peaceful = MacrocosmSubworld.Current.PeacefulWorld;
            for (int type = 0; type < NPCLoader.NPCCount; type++)
            {
                if (peaceful)
                    pool.Remove(type);
                else if (SubworldSystem.IsActive<Moon>() && !NPCSets.MoonNPC[type])
                    pool.Remove(type);
                //if (SubworldSystem.IsActive<Mars>() && !NPCSets.MarsNPCs[type])
                //  pool.Remove(type);
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!SubworldSystem.AnyActive<Macrocosm>())
                return;

            if (player.InModBiome<MoonUndergroundBiome>())
            {
                spawnRate = (int)(spawnRate * 0.75f);
                maxSpawns = (int)(maxSpawns * 1.5f);
            }
            /*
            else if (player.InModBiome<IrradiationBiome>())
            {
                //...
            }
            */
            else if (player.InModBiome<MoonNightBiome>())
            {
                spawnRate = (int)(spawnRate * 0.6f);
                maxSpawns = (int)(maxSpawns * 2f);
            }
            else if (player.InModBiome<MoonBiome>())
            {
                spawnRate = (int)(spawnRate * 1f);
                maxSpawns = (int)(maxSpawns * 1f);
            }

            if (RoomOxygenSystem.IsRoomPressurized(player.Center))
            {
                spawnRate = (int)(spawnRate * 0.1f);
                maxSpawns = (int)(maxSpawns * 0.6f);
            }
        }
    }
}