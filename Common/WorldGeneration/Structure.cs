using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using StructureHelper;
using Terraria;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    public abstract class Structure
    {
        public virtual string StructureFile => this.GetNamespacePath().Replace("Macrocosm/", "");

        public bool IsMultistructure => StructureHelper.API.Legacy.LegacyGenerator.IsMultistructure(StructureFile, Macrocosm.Instance) ?? false;

        private Point16 size;
        /// <summary> Size of the structure's first variant </summary>
        public Point16 Size
        {
            get
            {
                if (size == default)
                {
                    Point16 dims = default;
                    if (IsMultistructure)
                        StructureHelper.API.Legacy.LegacyGenerator.GetMultistructureDimensions(StructureFile, Macrocosm.Instance, 0, ref dims);
                    else
                        StructureHelper.API.Legacy.LegacyGenerator.GetDimensions(StructureFile, Macrocosm.Instance, ref dims);
                    size = dims;
                }

                return size;
            }
        }

        public virtual bool PrePlace(Point16 origin) { return true; }
        public virtual void PostPlace(Point16 origin) { }

        /// <summary>
        /// Places the structure and adds it to the provided structure map
        /// </summary>
        /// <param name="origin"> Placement top left origin </param>
        /// <param name="structures"> The structure protection map. Leave null if overlap protection is not required </param>
        /// <param name="variant"> 
        /// <br> The structure variant, relevant if <see cref="IsMultistructure"/>. Throws an exception if <see cref="IsMultistructure"/> and the value is out of bounds. </br>
        /// <br> Leave null for random variant. Do note that <see cref="Size"/> reflects the size of the zero-index variant. </br>
        /// </param>
        /// <param name="ignoreNull"> Whether to ignore null (negative space) tiles/walls </param>
        /// <param name="genFlags"> Extra flags for altering generation behaviour with paints, slopes and TEs </param>
        /// <returns> Whether the placement was successfu l</returns>
        public bool Place(Point16 origin, StructureMap structures, int? variant = null, bool ignoreNull = false, GenFlags genFlags = GenFlags.None)
        {
            if (!WorldGen.InWorld(origin.X, origin.Y))
                return false;

            Rectangle area = new(origin.X, origin.Y, Size.X, Size.Y);
            if (structures != null && !structures.CanPlace(area))
                return false;

            if (!WorldGen.InWorld(origin.X + Size.X, origin.Y + Size.Y))
                return false;

            if (!PrePlace(origin))
                return false;

            bool success;
            if (IsMultistructure)
            {
                if (variant is null)
                    success = StructureHelper.API.Legacy.LegacyGenerator.GenerateMultistructureRandom(StructureFile, origin, Macrocosm.Instance, false, ignoreNull, genFlags);
                else
                    success = StructureHelper.API.Legacy.LegacyGenerator.GenerateMultistructureSpecific(StructureFile, origin, Macrocosm.Instance, variant.Value, false, ignoreNull, genFlags);
            }
            else
            {
                success = StructureHelper.API.Legacy.LegacyGenerator.GenerateStructure(StructureFile, origin, Macrocosm.Instance, false, ignoreNull, genFlags);
            }

            if (success)
            {
                structures?.AddProtectedStructure(area, padding: 50);
                PostPlace(origin);
            }

            return success;
        }
    }
}
