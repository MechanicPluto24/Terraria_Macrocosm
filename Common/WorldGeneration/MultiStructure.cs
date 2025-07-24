using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using StructureHelper;
using StructureHelper.Models;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration;

// Use only if you're sure the structures will share all overrides (e.g. all will use the same PrePlace)
public abstract class MultiStructure : ModType
{
    protected sealed override void Register()
    {
        ModTypeLookup<MultiStructure>.Register(this);
        data = StructureHelper.API.MultiStructureGenerator.GetMultiStructureData(Path, Mod);
    }
    public override void SetupContent() => SetStaticDefaults();
    protected MultiStructure() { }

    public static MultiStructure Get<T>() where T : MultiStructure => ModContent.GetInstance<T>();

    /// <inheritdoc cref="Place(Point16, StructureMap, int?, int, bool, GenFlags)"/>
    public static void Place<T>(Point16 origin, StructureMap structures, int? index = null, int padding = 0, bool ignoreNull = false, GenFlags genFlags = GenFlags.None) where T : MultiStructure
        => Get<T>().Place(origin, structures, index, padding, ignoreNull, genFlags);

    private MultiStructureData data;
    public virtual string Path => this.GetNamespacePath().Replace("Macrocosm/", "");
    public int Count => data.count;
    public Point16 Size(int index) => new(data.structures[index].width, data.structures[index].height);
    public virtual bool IsInBounds(Point16 origin, int index) => StructureHelper.API.MultiStructureGenerator.IsInBounds(data, index, origin);

    public virtual bool PrePlace(Point16 origin) { return true; }
    public virtual void PostPlace(Point16 origin) { }

    /// <summary>
    /// Places the structure and adds it to the provided structure map
    /// </summary>
    /// <param name="origin"> Placement top left origin </param>
    /// <param name="structures"> The structure protection map. Leave null if overlap protection is not required </param>
    /// <param name="index"> The specific structure index of the multistructure, leave null for random. </param>
    /// <param name="ignoreNull"> Whether to ignore null (negative space) tiles/walls </param>
    /// <param name="genFlags"> Extra flags for altering generation behaviour with paints, slopes and TEs </param>
    /// <returns> Whether the placement was successful</returns>
    public bool Place(Point16 origin, StructureMap structures, int? index = null, int padding = 0, bool ignoreNull = false, GenFlags genFlags = GenFlags.None)
    {
        if (!WorldGen.InWorld(origin.X, origin.Y))
            return false;

        int idx = index ?? WorldGen.genRand.Next(Count);
        StructureData selected = data.structures[idx];

        if (!IsInBounds(origin, idx))
            return false;

        Rectangle area = new(origin.X, origin.Y, selected.width, selected.height);
        if (structures != null && !structures.CanPlace(area, padding))
            return false;

        if (!PrePlace(origin))
            return false;

        StructureHelper.API.Generator.GenerateFromData(selected, origin, ignoreNull, genFlags);
        structures?.AddProtectedStructure(area, padding: padding);
        PostPlace(origin);

        return true;
    }
}
