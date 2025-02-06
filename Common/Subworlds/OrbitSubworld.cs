using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Subworlds
{
    public abstract class OrbitSubworld : MultiSubworld
    {
        public abstract string ParentSubworldID { get; }

        public static IEnumerable<OrbitSubworld> GetOrbitSubworlds(string parentSubworldId)
        {
            foreach(var subworld in MacrocosmSubworlds)
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
    }
}
