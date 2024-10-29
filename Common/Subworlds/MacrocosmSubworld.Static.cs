using Macrocosm.Common.Players;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace Macrocosm.Common.Subworlds
{

    public partial class MacrocosmSubworld
    {
        /// <summary> Get the current <c>MacrocosmSubworld</c> active instance. 
        /// Earth returns null! You should check for <see cref="SubworldSystem.AnyActive{Macrocosm}"/> for <see cref="Macrocosm"/> before accessing this. </summary>
        public static MacrocosmSubworld Current => SubworldSystem.AnyActive<Macrocosm>() ? SubworldSystem.Current as MacrocosmSubworld : null;

        /// <summary>
        /// Get the current active subworld string ID, matching the subworld class name with the mod name prepended. 
        /// Returns <see cref="Earth.ID"/> if none active.
        /// </summary>
        public static string CurrentID => SubworldSystem.AnyActive() ? SubworldSystem.Current.Mod.Name + "/" + SubworldSystem.Current.Name : Earth.ID;
        public static bool IsValidID(string id) => SubworldSystem.GetIndex(id) >= 0 || id is Earth.ID;

        public static string SanitizeID(string id) => SanitizeID(id, out _);
        public static string SanitizeID(string id, out string modName)
        {
            string[] split = id.Split("/");

            if (string.IsNullOrEmpty(id))
            {
                modName = "Macrocosm";
                return "";
            }
            else if (split.Length == 1)
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

        public static Dictionary<string, MacrocosmSubworld> Subworlds { get; } = [];

        public static int CurrentIndex => SubworldSystem.AnyActive() ? SubworldSystem.GetIndex(CurrentID) : -1;

        public static Guid MainWorldUniqueID => SubworldSystem.AnyActive() ? typeof(SubworldSystem).GetFieldValue<WorldFileData>("main").UniqueId : Main.ActiveWorldFileData.UniqueId;

        public static double CurrentTimeRate => Current is not null ? Current.TimeRate : Earth.TimeRate;
        public static double CurrentDayLength => Current is not null ? Current.DayLength : Earth.DayLength;
        public static double CurrentNightLength => Current is not null ? Current.NightLength : Earth.NightLength;
        public static float CurrentGravityMultiplier => Current is not null ? Current.GravityMultiplier : Earth.GravityMultiplier;
        public static float CurrentAtmosphericDensity => Current is not null ? Current.AtmosphericDensity : Earth.AtmosphericDensity;
        public static float GetCurrentAmbientTemperature() => Current is not null ? Current.GetAmbientTemperature() : Earth.GetAmbientTemperature();

        /// <summary> The loading screen. </summary>
        public static LoadingScreen LoadingScreen { get; set; }

        /// <summary> Travel to the specified subworld, using the specified rocket. </summary>
        /// <param name="targetWorldID"> The world to travel to, "Earth" for returning to the main world. </param>
        /// <param name="rocket"> The spacecraft used for travel, if applicable. Will display in the loading screen. </param>
        /// <param name="trigger"> Value set to the <see cref="MacrocosmPlayer.TriggeredSubworldTravel"/>. Normally true. </param>
        /// <returns> Whether world travel has been successful </returns>
        public static bool Travel(string targetWorldID, Rocket rocket = null, bool trigger = true)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (!trigger)
                    rocket = null;

                SetupLoadingScreen(rocket, targetWorldID);
                TitleCard.SetTargetWorld(targetWorldID);

                Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().TriggeredSubworldTravel = trigger;
                Main.LocalPlayer.GetModPlayer<SubworldTravelPlayer>().SetReturnSubworld(targetWorldID);

                if (targetWorldID == Earth.ID)
                {
                    SubworldSystem.Exit();
                    return true;
                }

                bool entered = SubworldSystem.Enter(targetWorldID);
                if (!entered)
                    WorldTravelFailure("Error: Failed entering target subworld: " + targetWorldID + ", staying on " + CurrentID);

                return entered;
            }
            else
            {
                return false;
            }
        }

        private static void SetupLoadingScreen(Rocket rocket, string targetWorld)
        {
            if (rocket is not null)
            {
                if (!SubworldSystem.AnyActive<Macrocosm>())
                {
                    LoadingScreen = new EarthLoadingScreen();
                }
                else
                {
                    LoadingScreen = SanitizeID(CurrentID) switch
                    {
                        nameof(Moon) => new MoonLoadingScreen(),
                        _ => null,
                    };
                }
            }
            else
            {
                LoadingScreen = SanitizeID(targetWorld) switch
                {
                    nameof(Earth) => new EarthLoadingScreen(),
                    nameof(Moon) => new MoonLoadingScreen(),
                    _ => null,
                };
            }

            switch (SanitizeID(targetWorld))
            {
                case "Moon":
                    LoadingScreen?.SetProgressBar(new(
                        ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/ProgressBarMoon", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/ProgressBarMoon_Lower", AssetRequestMode.ImmediateLoad),
                        new Color(56, 10, 28), new Color(155, 38, 74), new Color(6, 53, 27), new Color(93, 228, 162)
                    ));
                    break;
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

            public static bool SubworldSystem_CacheIsNull()
            {
                FieldInfo field = typeof(SubworldSystem).GetField("cache", BindingFlags.Static | BindingFlags.NonPublic);
                return field.GetValue(null) is null;
            }
        }
    }
}
