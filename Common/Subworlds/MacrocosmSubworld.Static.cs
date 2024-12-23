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
using System.Collections;
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
        public static string CurrentID => SubworldSystem.AnyActive() ? SubworldSystem.Current.FullName : Earth.ID;
        public static bool IsValidID(string id) => SubworldSystem.GetIndex(id) >= 0 || id is Earth.ID;

        public static string SanitizeID(string id) => SanitizeID(id, out _);
        public static string SanitizeID(string id, out string modName)
        {
            string[] split = id.Split("/");

            if (string.IsNullOrEmpty(id))
            {
                modName = nameof(Macrocosm);
                return "";
            }
            else if (split.Length == 1)
            {
                modName = nameof(Macrocosm);
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

        public static void SetupLoadingScreen(Rocket rocket, string targetWorld)
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

        public class Hacks
        {
            private static FieldInfo subworldSystem_current;
            private static FieldInfo subworldSystem_cache;

            private static MethodInfo subworldSystem_GetPacketHeader;
            private static FieldInfo subworldSystem_links;
            private static MethodInfo subserverLink_Send;

            public static void Initialize()
            {
                if (subworldSystem_current == null)
                {
                    throw new Exception("Failed to find SubworldSystem.current field.");
                }
            }

            public static Subworld SubworldSystem_GetCurrent()
            {
                subworldSystem_current ??= typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
                return (Subworld)subworldSystem_current.GetValue(null);
            }
            public static void SubworldSystem_NullCurrent()
            {
                subworldSystem_current ??= typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
                subworldSystem_current.SetValue(null, null);
            }

            public static void SubworldSystem_SetCurrent(Subworld value)
            {
                subworldSystem_current ??= typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
                subworldSystem_current.SetValue(null, value);
            }

            public static Subworld SubworldSystem_GetCache()
            {
                subworldSystem_cache ??= typeof(SubworldSystem).GetField("cache", BindingFlags.Static | BindingFlags.NonPublic);
                return (Subworld)subworldSystem_cache.GetValue(null);
            }

            public static void SubworldSystem_NullCache()
            {
                subworldSystem_cache ??= typeof(SubworldSystem).GetField("cache", BindingFlags.Static | BindingFlags.NonPublic);
                subworldSystem_cache.SetValue(null, null);
            }

            public static string SubworldSystem_CacheID() => SubworldSystem_GetCache().FullName;
            public static bool SubworldSystem_CacheIsNull() => SubworldSystem_GetCache() == null;


            /// <summary>
            /// Sends a packet from the specified mod directly to all subservers, except the specified one.
            /// </summary>
            /// <param name="mod">The mod sending the packet.</param>
            /// <param name="data">The data to send.</param>
            /// <param name="exceptSubserverIndex">The subserver ID to exclude.</param>
            public static void SubworldSystem_SendToAllSubserversExcept(Mod mod, byte[] data, int exceptSubserverIndex)
            {
                subworldSystem_GetPacketHeader ??= typeof(SubworldSystem).GetMethod("GetPacketHeader", BindingFlags.Static | BindingFlags.NonPublic);
                subworldSystem_links ??= typeof(SubworldSystem).GetField("links", BindingFlags.Static | BindingFlags.NonPublic);

                if (subworldSystem_links.GetValue(null) is not IDictionary links)
                    throw new Exception("Failed to retrieve SubworldSystem.links.");

                int headerLength = (ModNet.NetModCount < 256) ? 5 : 6;
                byte[] packetHeader = (byte[])subworldSystem_GetPacketHeader.Invoke(null, [data.Length + headerLength, mod.NetID]);
                Buffer.BlockCopy(data, 0, packetHeader, headerLength, data.Length);

                foreach (DictionaryEntry entry in links)
                {
                    int subserverID = (int)entry.Key;
                    if (subserverID != exceptSubserverIndex)
                    {
                        var subserverLink = entry.Value;
                        subserverLink_Send ??= subserverLink.GetType().GetMethod("Send", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        subserverLink_Send.Invoke(subserverLink, [packetHeader]);
                    }
                }
            }

        }
    }
}
