using Macrocosm.Common.Storage;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Machines
{
    public abstract class MachineTE : ModTileEntity
    {
        public abstract MachineTile MachineTile { get; }

        public abstract bool Operating { get; }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == MachineTile.Type;
        }
    }
}
