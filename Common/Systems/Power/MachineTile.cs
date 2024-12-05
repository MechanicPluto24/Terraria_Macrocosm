using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Utils;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineTile : ModTile
    {
        /// <summary> The Tile's width </summary>
        public abstract short Width { get; }

        /// <summary> The Tile's height </summary>
        public abstract short Height { get; }

        /// <summary> The Machine TileEntity template instance associated with this multitile </summary>
        public abstract MachineTE MachineTE { get; }

        /// <summary> 
        /// Used to determine if the machine is powered on, by using the Tile frame.
        /// <br/> Typically, <see cref="MachineTE.PoweredOn"/> is determined by this return value.
        /// </summary>
        public virtual bool IsPoweredOnFrame(int i, int j) => false;

        /// <summary>
        /// Implement here toggling of the tile's frame (what happens when wires or switches on the tile are hit) 
        /// <br/> For initiating the toggle, please call <see cref="Toggle"/>
        /// </summary>
        public virtual void OnToggleStateFrame(int i, int j, bool skipWire = false) { }

        public void Toggle(int i, int j, bool automatic, bool skipWire = false)
        {
            if (Utility.TryGetTileEntityAs(i, j, out MachineTE machineTE))
            {
                machineTE.Toggle(automatic, skipWire);
            }
        }

        public override void HitWire(int i, int j)
        {
            if (Utility.TryGetTileEntityAs(i, j, out BatteryTE _))
                return;

            Toggle(i, j, automatic: false, skipWire: true);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) => MachineTE.Kill(i, j);
    }
}
