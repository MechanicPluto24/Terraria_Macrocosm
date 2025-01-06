using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

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


        public override string Name => base.Name + Index; 
        protected abstract int InstanceCount { get; }
        public abstract string ParentSubworldID { get; }
        public int Index { get; private set; }

        public override void Load()
        {
            base.Load();

            // Index 0 manages the loading of all the other instances
            if (Index <= 0)
            {
                OrbitSubworld[] subworlds = new OrbitSubworld[InstanceCount];
                subworlds[0] = this;

                for (int i = 1; i < InstanceCount; i++)
                {
                    if (Activator.CreateInstance(GetType()) is OrbitSubworld instance)
                    {
                        instance.Index = i;
                        Mod.AddContent(instance);
                        subworlds[i] = instance;
                    }
                }

                OrbitSubworldsByParent.Add(ParentSubworldID, subworlds.ToList());
            }
        }

        public override void Unload()
        {
            base.Unload();
        }
    }
}
