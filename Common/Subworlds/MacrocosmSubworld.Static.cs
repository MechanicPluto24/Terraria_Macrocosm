﻿using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Subworlds
{
    public enum MapColorType
    {
        SkyUpper,
        SkyLower,
        UndergroundUpper,
        UndergroundLower,
        CavernUpper,
        CavernLower,
        Underworld
    }

    public partial class MacrocosmSubworld
    {
        /// <summary> Get the current <c>MacrocosmSubworld</c> active instance. 
        /// Earth returns null! You should check for <see cref="SubworldSystem.AnyActive{Macrocosm}"/> for <see cref="Macrocosm"/> before accessing this. </summary>
        public static MacrocosmSubworld Current => SubworldSystem.AnyActive<Macrocosm>() ? SubworldSystem.Current as MacrocosmSubworld : null;

        /// <summary>
        /// Get the current active subworld string ID, matching the subworld class name with the mod name prepended. 
        /// Returns <c>"Macrocosm/Earth"</c> if none active.
        /// </summary>
        public static string CurrentID => SubworldSystem.AnyActive() ? SubworldSystem.Current.Mod.Name + "/" + SubworldSystem.Current.Name : "Macrocosm/Earth";
        public static bool IsValidID(string id) => SubworldSystem.GetIndex(id) >= 0 || id is "Macrocosm/Earth";
        public static string SanitizeID(string id, out string modName)
        {
            string[] split = id.Split("/");

            if (string.IsNullOrEmpty(id))
            {
                modName = "Macrocosm";
                return "";
            }
            else if(split.Length == 1)
            {
                modName = "Macrocosm";
                return split[0];
            }
            else
            {
                modName = split[0];
                return split[1];
            }   
        }

        public static int CurrentIndex => SubworldSystem.AnyActive() ? SubworldSystem.GetIndex(CurrentID) : -1;

        public static Guid MainWorldUniqueID => SubworldSystem.AnyActive() ? typeof(SubworldSystem).GetFieldValue<WorldFileData>("main").UniqueId : Main.ActiveWorldFileData.UniqueId;

        public static double CurrentTimeRate => Current is not null ? Current.TimeRate : Earth.TimeRate;
        public static double CurrentDayLength => Current is not null ? Current.DayLenght : Earth.DayLenght;
        public static double CurrentNightLength => Current is not null ? Current.NightLenght : Earth.NightLenght;
        public static float CurrentGravityMultiplier => Current is not null ? Current.GravityMultiplier : Earth.GravityMultiplier;
        public static float CurrentAtmosphericDensity => Current is not null ? Current.AtmosphericDensity : Earth.AtmosphericDensity;

        /// <summary> The loading screen. </summary>
        public static LoadingScreen LoadingScreen { get; set; }

        /// <summary> Travel to the specified subworld, using the specified rocket. </summary>
        /// <param name="targetWorld"> The world to travel to, "Earth" for returning to the main world. </param>
        /// <param name="rocket"> The spacecraft used for travel, if applicable. Will display in the loading screen. </param>
        /// <param name="trigger"> Value set to the <see cref="MacrocosmPlayer.TriggeredSubworldTravel"/>. Normally true. </param>
        /// <returns> Whether world travel has been successful </returns>
        public static bool Travel(string targetWorld, Rocket rocket = null, bool trigger = true)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (!trigger)
                    rocket = null;

                UpdateLoadingScreen(rocket, targetWorld);

                Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().TriggeredSubworldTravel = trigger;
                Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().SetReturnSubworld(targetWorld);

                if (targetWorld == "Macrocosm/Earth")
                {
                    SubworldSystem.Exit();
                    LoadingScreen?.SetTargetWorld(targetWorld);
                    TitleCard.SetTargetWorld(targetWorld);
                    return true;
                }

                bool entered = SubworldSystem.Enter(targetWorld);

                if (entered)
                {
                    LoadingScreen?.SetTargetWorld(targetWorld);
                    TitleCard.SetTargetWorld(targetWorld);
                }
                else
                {
                    WorldTravelFailure("Error: Failed entering target subworld: " + targetWorld + ", staying on " + CurrentID);
                }

                return entered;
            }
            else
            {
                return false;
            }
        }

        private static void UpdateLoadingScreen(Rocket rocket, string targetWorld)
        {
            if (rocket is not null)
            {
                if (!SubworldSystem.AnyActive<Macrocosm>())
                    LoadingScreen = new EarthLoadingScreen();
                else LoadingScreen = CurrentID switch
                {
                    "Macrocosm/Moon" => new MoonLoadingScreen(),
                    _ => null,
                };
            }
            else
            {
                LoadingScreen = targetWorld switch
                {
                    "Macrocosm/Earth" => new EarthLoadingScreen(),
                    "Macrocosm/Moon" => new MoonLoadingScreen(),
                    _ => null,
                };
            }

            if (rocket is not null)
                LoadingScreen?.SetRocket(rocket);
            else
                LoadingScreen?.ClearRocket();


            LoadingScreen?.Setup();
        }

        // Called if travel to the target subworld fails
        public static void WorldTravelFailure(string message)
        {
            Utility.Chat(message, Color.Red);
            Macrocosm.Instance.Logger.Error(message);
        }

        public class Hacks
        {
            /// <summary>
            /// Remove this once SubworldLibrary <see href="https://github.com/jjohnsnaill/SubworldLibrary/pull/35"/> is merged.
            /// </summary>
            public static void SubworldSystem_NullCache()
            {
                FieldInfo field = typeof(SubworldSystem).GetField("cache", BindingFlags.Static | BindingFlags.NonPublic);
                field.SetValue(null, null);
            }

            /// <summary> 
            /// Bypasses SubworldLibrary tag key existence check. 
            /// TODO: Replace with <see cref="SubworldSystem.CopyWorldData(string, object)"/> on next SubworldLibrary update.
            /// </summary>
            public static void SubworldSystem_CopyWorldData(string key, object data)
            {
                var copiedData = Utility.GetFieldValue<TagCompound>(typeof(SubworldSystem), "copiedData", flags: BindingFlags.Static | BindingFlags.NonPublic);

                if (data != null)
                    copiedData[key] = data;
            }

            public static void SubworldSystem_SendToAllSubserversExcept(Mod mod, int ignoreSubserver, byte[] data)
            {

            }
        }
    }
}
