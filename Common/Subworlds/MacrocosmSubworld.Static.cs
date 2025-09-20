using Macrocosm.Common.Systems;
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
using System.Linq;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace Macrocosm.Common.Subworlds;


public partial class MacrocosmSubworld
{
    /// <summary>
    /// Get the current <c>MacrocosmSubworld</c> active instance. 
    /// <br/> Main world (Earth) and other mods' subworlds return null! 
    /// <br/> You should check for <see cref="SubworldSystem.AnyActive{T}"/> for <see cref="Macrocosm"/> before accessing this. 
    /// </summary>
    public static MacrocosmSubworld Current => SubworldSystem.AnyActive<Macrocosm>() ? (SubworldSystem.Current is MacrocosmSubworld macrocosmSubworld ? macrocosmSubworld : null) : null;

    /// <summary>
    /// Get the current active <see cref="Subworld"/> (not <see cref="MacrocosmSubworld"/>!) string ID, matching the subworld class name with the mod name prepended. 
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

    private static Subworld Cache => typeof(SubworldSystem).GetFieldValue<Subworld>("cache");
    public static string CacheID => Cache.FullName;
    public static WorldFileData MainWorldFileData => typeof(SubworldSystem).GetFieldValue<WorldFileData>("main");
    public static List<MacrocosmSubworld> MacrocosmSubworlds => Subworlds.Where(s => s is MacrocosmSubworld).Cast<MacrocosmSubworld>().ToList();
    public static List<Subworld> Subworlds => typeof(SubworldSystem).GetFieldValue<List<Subworld>>("subworlds");

    public static int CurrentIndex => SubworldSystem.AnyActive() ? SubworldSystem.GetIndex(CurrentID) : -1;

    public static Guid MainWorldUniqueID => SubworldSystem.AnyActive() ? typeof(SubworldSystem).GetFieldValue<WorldFileData>("main").UniqueId : Main.ActiveWorldFileData.UniqueId;

    public static double CurrentTimeRate => Current is not null ? Current.TimeRate : Earth.TimeRate;
    public static double CurrentWorldUpdateRate => Current is not null ? Current.WorldUpdateRate : Earth.TimeRate;
    public static double CurrentTileUpdateRate => Current is not null ? Current.TileUpdateRate : Earth.TimeRate;

    public static double GetDayLength() => Current is not null ? Current.DayLength : Earth.DayLength;

    public static double GetNightLength() => Current is not null ? Current.NightLength : Earth.NightLength;

    public static float GetGravityMultiplier(Vector2? position = null)
    {
        Vector2 worldPosition = position ?? Main.LocalPlayer.position;
        return Current?.GravityMultiplier(worldPosition) ?? Earth.GravityMultiplier;
    }

    public static float GetAmbientTemperature(Vector2? position = null)
    {
        Vector2 worldPosition = position ?? Main.LocalPlayer.position;
        return Current?.AmbientTemperature(worldPosition) ?? Earth.AmbientTemperature(worldPosition);
    }

    public static float GetAtmosphericDensity(Vector2? position = null, bool checkRooms = false)
    {
        if (Current is not null)
        {
            Vector2 worldPosition = position ?? Main.LocalPlayer.position;
            if (checkRooms && RoomOxygenSystem.CheckRoomOxygen(worldPosition))
                return Earth.AtmosphericDensity;

            return Current.AtmosphericDensity(worldPosition);
        }
        else
        {
            return Earth.AtmosphericDensity;
        }
    }

    /// <summary> The loading screen. </summary>
    public static LoadingScreen LoadingScreen { get; set; }
    public static void SetupLoadingScreen(Rocket rocket, string targetWorld, bool downwards = false)
    {
        string currentId = CurrentID;
        string targetId = targetWorld;

        string id = SanitizeID(rocket is not null ? currentId : targetId);

        LoadingScreen = id switch
        {
            nameof(Earth) => new EarthLoadingScreen(),
            nameof(Moon) => new MoonLoadingScreen(),
            _ => null,
        };

        switch (targetId)
        {
            case nameof(Moon):
                LoadingScreen?.SetProgressBar(new(
                    ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/ProgressBarMoon", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/ProgressBarMoon_Lower", AssetRequestMode.ImmediateLoad),
                    new Color(56, 10, 28), new Color(155, 38, 74), new Color(6, 53, 27), new Color(93, 228, 162)
                ));
                break;
        }

        if (rocket is not null)
            LoadingScreen?.SetRocket(rocket, downwards);
        else
            LoadingScreen?.ClearRocket();

        LoadingScreen?.Setup();
    }


}
