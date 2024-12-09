using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Flags;
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
    public abstract class MultiSubworld : MacrocosmSubworld
    {
        public static readonly Dictionary<string, List<MultiSubworld>> MultiSubworldsByParent = [];

        public static List<MultiSubworld> GetMultiSubworlds(string parentSubworld)
        {
            if(MultiSubworldsByParent.TryGetValue(parentSubworld, out var orbitSubworlds))
                return orbitSubworlds;

            return [];
        }

        public static bool IsMultiSubworld(string subworldId)
        {
            if (Subworlds.TryGetValue(subworldId, out var macrocosmSubworld) && macrocosmSubworld is MultiSubworld _)
                return true;

            return false;
        }

        public static string GetParentID(string subworldId)
        {
            if (Subworlds.TryGetValue(subworldId, out var macrocosmSubworld) && macrocosmSubworld is MultiSubworld orbitSubworld)
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
                MultiSubworld[] orbitSubworlds = new MultiSubworld[InstanceCount];
                orbitSubworlds[0] = this;

                for (int i = 1; i < InstanceCount; i++)
                {
                    if (Activator.CreateInstance(GetType()) is MultiSubworld instance)
                    {
                        instance.Index = i;
                        Mod.AddContent(instance);
                        orbitSubworlds[i] = instance;
                    }
                }

                MultiSubworldsByParent.Add(ParentSubworldID, orbitSubworlds.ToList());
            }
        }

        public override void Unload()
        {
            base.Unload();
        }
    }
}
