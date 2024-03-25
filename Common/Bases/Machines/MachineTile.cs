using System;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Machines
{
    public abstract class MachineTile : ModTile
    {
        public abstract short Width { get; }

        public abstract short Height { get; }

        public abstract MachineTE MachineTE { get; }

    }
}
