using Terraria.ModLoader;
using System.Collections.Generic;
using Macrocosm.NPCs.Unfriendly.Enemies;
using Terraria;
using System;

namespace Macrocosm
{
    /// <summary>
    /// Contains lists of enemies for each planet, some are uncomplete and some (for now) are completely un-filled
    /// </summary>
    public class EnemyCategorization
    {
        /// <summary>
        /// Enemies of our bastion of life, The Sun. See list to view said NPCs
        /// </summary>
        public static List<int> SunEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the dwarf planet Eris, see list to view said NPCs
        /// </summary>
        public static List<int> ErisEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of our very own satellite, The Moon. See list to view said NPCs
        /// </summary>
        public static List<int> MoonEnemies = new List<int>
        {
            ModContent.NPCType<Clavite>(),
            ModContent.NPCType<RegolithSlime>()
        };
        /// <summary>
        /// Enemies of the potential future red home-planet Mars, see list to view said NPCs
        /// </summary>
        public static List<int> MarsEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the acid rain hell that is Venus, see list to view said NPCs
        /// </summary>
        public static List<int> VenusEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the closest planet to the sun Mercury, see list to view said NPCs
        /// </summary>
        public static List<int> MercuryEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the make believe planet, Vulcan, see list to view said NPCs
        /// </summary>
        public static List<int> VulcanEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Mars' moon Phobos, see list to view said NPCs
        /// </summary>
        public static List<int> PhobosEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Mars' moon, Deimos, see list to view said NPCs
        /// </summary>
        public static List<int> DeimosEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the dwarf planet Ceres, see list to view said NPCs
        /// </summary>
        public static List<int> CeresEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the gas giant Jupiter, see list to view said NPCs
        /// </summary>
        public static List<int> JupiterEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Jupiter's volcanically active moon Io, see list to view said NPCs
        /// </summary>
        public static List<int> IoEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Jupiter's somewhat icy moon Europa, see list to view said NPCs
        /// </summary>
        public static List<int> EuropaEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the ringed gas giant Saturn, see list to view said NPCs
        /// </summary>
        public static List<int> SaturnEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Saturn's thick atmosphere-d moon and would-be planet Titan, see list to view said NPCs
        /// </summary>
        public static List<int> TitanEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the ice giant Uranus, see list to view said NPCs
        /// </summary>
        public static List<int> UranusEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Uranus' deathly cold planet Miranda, see list to view said NPCs
        /// </summary>
        public static List<int> MirandaEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the water & ice giant Neptune, see list to view said NPCs
        /// </summary>
        public static List<int> NeptuneEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of Neptune's biggest planet Triton, see list to view said NPCs
        /// </summary>
        public static List<int> TritonEnemies = new List<int>
        {
        };
        /// <summary>
        /// Enemies of the (nearly a planet) dwarf planet Pluto, see list to view said NPCs
        /// </summary>
        public static List<int> PlutoEnemies = new List<int>
        {
        };
    }
    public class RandomHelper
    {
        // Oh yeah >:)
        public static T PickRandom<T>(T[] input)
        {
            int rand = new Random().Next(0, input.Length);

            return input[rand];
        }
        private static List<int> chosenTs = new List<int>();
        public static List<T> PickRandom<T>(T[] input, int amount)
        {
            List<T> values = new List<T>();
            for (int i = 0; i < amount; i++)
            {
            ReRoll:
                int rand = new Random().Next(0, input.Length);

                if (!chosenTs.Contains(rand))
                {
                    chosenTs.Add(rand);
                    values.Add(input[rand]);
                }
                else
                    goto ReRoll;
            }
            chosenTs.Clear();
            return values;
        }
    }
}