using System;
using Terraria.ModLoader;

namespace Macrocosm.Common.Subworlds;

public abstract class MultiSubworld : MacrocosmSubworld
{
    public override string Name => base.Name + InstanceIndex;
    public int InstanceIndex { get; private set; } = 0;
    protected abstract int InstanceCount { get; }

    public override void Load()
    {
        base.Load();

        // Index 0 manages the loading of all the other instances
        if (InstanceIndex <= 0)
        {
            MultiSubworld[] subworlds = new MultiSubworld[InstanceCount];
            subworlds[0] = this;

            for (int i = 1; i < InstanceCount; i++)
            {
                if (Activator.CreateInstance(GetType()) is MultiSubworld instance)
                {
                    instance.InstanceIndex = i;
                    Mod.AddContent(instance);
                    subworlds[i] = instance;
                }
            }
        }
    }

    public override void Unload()
    {
        base.Unload();
    }
}
