using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using StructureHelper;
using StructureHelper.Models;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    public abstract class Structure : ModType
    {
        protected sealed override void Register()
        {
            ModTypeLookup<Structure>.Register(this);
            data = StructureHelper.API.Generator.GetStructureData(Path, Mod);
        }
        public sealed override void SetupContent() => SetStaticDefaults();
        protected Structure() { }
        public static Structure Get<T>() where T : Structure => ModContent.GetInstance<T>();

        /// <inheritdoc cref="Place(Point16, StructureMap, int, bool, GenFlags)"/>
        public static void Place<T>(Point16 origin, StructureMap structures, int padding = 0, bool ignoreNull = false, GenFlags genFlags = GenFlags.None) where T : Structure
            => Get<T>().Place(origin, structures, padding, ignoreNull, genFlags);

        private StructureData data;
        public virtual string Path => this.GetNamespacePath().Replace("Macrocosm/", "");
        public virtual Point16 Size => new(data.width, data.height);

        public virtual bool IsInBounds(Point16 origin) => StructureHelper.API.Generator.IsInBounds(data, origin);

        public virtual bool PrePlace(Point16 origin) { return true; }
        public virtual void PostPlace(Point16 origin) { }

        /// <summary>
        /// Places the structure and adds it to the provided structure map
        /// </summary>
        /// <param name="origin"> Placement top left origin </param>
        /// <param name="structures"> The structure protection map. Leave null if overlap protection is not required </param>
        /// <param name="ignoreNull"> Whether to ignore null (negative space) tiles/walls </param>
        /// <param name="genFlags"> Extra flags for altering generation behaviour with paints, slopes and TEs </param>
        /// <returns> Whether the placement was successful</returns>
        public bool Place(Point16 origin, StructureMap structures, int padding = 0, bool ignoreNull = false, GenFlags genFlags = GenFlags.None)
        {
            if (!WorldGen.InWorld(origin.X, origin.Y))
                return false;

            if (!IsInBounds(origin))
                return false;

            Rectangle area = new(origin.X, origin.Y, Size.X, Size.Y);
            if (structures != null && !structures.CanPlace(area, padding))
                return false;

            if (!PrePlace(origin))
                return false;

            StructureHelper.API.Generator.GenerateFromData(data, origin, ignoreNull, genFlags);
            structures?.AddProtectedStructure(area, padding: padding);
            PostPlace(origin);

            return true;
        }
    }
}
