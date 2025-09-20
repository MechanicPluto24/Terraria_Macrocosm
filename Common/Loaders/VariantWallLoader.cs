using Macrocosm.Common.Bases.Walls;
using System;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Macrocosm.Common.Loaders;

public class VariantWallLoader : ILoadable
{
    public void Load(Mod mod)
    {
        IOrderedEnumerable<Type> variantTypes = AssemblyManager.GetLoadableTypes(mod.Code)
            .Where(t => !t.IsAbstract && !t.ContainsGenericParameters)
            .Where(t => t.IsAssignableTo(typeof(VariantWall)))
            .Where(t => AutoloadAttribute.GetValue(t).NeedsAutoloading)
            .OrderBy(type => type.FullName, StringComparer.InvariantCulture);

        foreach (var type in variantTypes)
        {
            var instance = Activator.CreateInstance(type) as VariantWall;
            foreach (var variant in instance.SafetyVariants.Skip(1)) // First is autoloaded
            {
                mod.AddContent(instance.CreateVariant(variant));
            }
        }
    }

    public void Unload()
    {
    }
}
