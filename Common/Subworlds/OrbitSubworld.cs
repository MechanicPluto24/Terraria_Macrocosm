using SubworldLibrary;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.Subworlds;

public abstract class OrbitSubworld : MultiSubworld
{
    public abstract string ParentSubworldID { get; }

    public static IEnumerable<OrbitSubworld> GetOrbitSubworlds(string parentSubworldId)
    {
        foreach (var subworld in MacrocosmSubworlds)
            if (subworld is OrbitSubworld orbitSubworld && orbitSubworld.ParentSubworldID == parentSubworldId)
                yield return orbitSubworld;
    }

    public static bool AnyActive() => IsOrbitSubworld(CurrentID);
    public static bool IsActive(string subworldId) => SubworldSystem.IsActive(subworldId) && IsOrbitSubworld(subworldId);
    public static bool IsOrbitSubworld(string subworldId)
    {
        if (MacrocosmSubworlds.FirstOrDefault(s => s.ID == subworldId) is OrbitSubworld _)
            return true;

        return false;
    }

    public static string GetParentID(string subworldId)
    {
        if (MacrocosmSubworlds.FirstOrDefault(s => s.ID == subworldId) is OrbitSubworld orbitSubworld)
            return orbitSubworld.ParentSubworldID;

        return subworldId;
    }

    public static bool IsUnlocked(string subworldId) => WorldData.GetSubworldData(subworldId).Unlocked;
    public static void Unlock(string subworldId)
    {
        var data = WorldData.GetSubworldData(subworldId);
        if (!data.Unlocked)
        {
            data.Unlocked = true;
            if (Main.netMode == Terraria.ID.NetmodeID.Server)
            {
                Utility.Chat($"Orbit unlocked: {subworldId}", Microsoft.Xna.Framework.Color.LightSkyBlue);
            }
        }
    }

    public static void UnlockForParent(string parentSubworldId)
    {
        foreach (var orbit in GetOrbitSubworlds(parentSubworldId))
        {
            var d = WorldData.GetSubworldData(orbit.ID);
            if (!d.Unlocked)
                d.Unlocked = true;
        }
    }
}
