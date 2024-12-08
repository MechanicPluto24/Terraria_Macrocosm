using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.Subworlds
{
    public abstract class OrbitSubworld : MacrocosmSubworld
    {
        public static readonly Dictionary<string, List<OrbitSubworld>> OrbitSubworldsByParent = [];

        public static List<OrbitSubworld> GetOrbitSubworlds(string parentSubworld)
        {
            if(OrbitSubworldsByParent.TryGetValue(parentSubworld, out var orbitSubworlds))
                return orbitSubworlds;

            return [];
        }

        public bool Unlocked { get; set; } = true;
        public override string Name => base.Name + Index; 

        protected abstract int InstanceCount { get; }
        public abstract string ParentSubworldID { get; }
        public int Index { get; private set; }

        public static bool IsOrbitSubworld(string subworldId)
        {
            if (Subworlds.TryGetValue(subworldId, out var macrocosmSubworld) && macrocosmSubworld is OrbitSubworld _)
                return true;

            return false;
        }

        public static string GetParentID(string subworldId)
        {
            if (Subworlds.TryGetValue(subworldId, out var macrocosmSubworld) && macrocosmSubworld is OrbitSubworld orbitSubworld)
                return orbitSubworld.ParentSubworldID;

            return subworldId;
        }

        public override void Load()
        {
            base.Load();

            // Index 0 manages the loading of all the other instances
            if (Index <= 0)
            {
                OrbitSubworld[] orbitSubworlds = new OrbitSubworld[InstanceCount];
                orbitSubworlds[0] = this;

                for (int i = 1; i < InstanceCount; i++)
                {
                    if (Activator.CreateInstance(GetType()) is OrbitSubworld instance)
                    {
                        instance.Index = i;
                        Mod.AddContent(instance);
                        orbitSubworlds[i] = instance;
                    }
                }

                OrbitSubworldsByParent.Add(ParentSubworldID, orbitSubworlds.ToList());
            }
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override WorldSize GetSubworldSize(WorldSize earthWorldSize) => WorldSize.Small;

        public override void SaveExtraData(TagCompound data)
        {
            if(Unlocked) data[nameof(Unlocked)] = true;
        }

        public override void LoadExtraData(TagCompound data)
        {
            Unlocked = data.ContainsKey(nameof(Unlocked));
        }
    }
}
