using Macrocosm.Common.Utils;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

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

        public bool IsSingleTile => Width == 1 && Height == 1;

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
            if (TileEntity.TryGet(i, j, out MachineTE machineTE))
                machineTE.Toggle(automatic, skipWire);
        }

        public override void HitWire(int i, int j)
        {
            if (TileEntity.TryGet(i, j, out MachineTE machineTE) && machineTE.CanToggleWithWire)
                machineTE.Toggle(automatic: false, skipWire: true);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            MachineTE.KillTile_ClusterCheck(new(i, j));

            if (IsSingleTile && !effectOnly)
                MachineTE.Kill(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (!IsSingleTile)
                MachineTE.Kill(i, j);
        }

        // Runs for "block" tiles that function as machines, not multitiles
        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (IsSingleTile && TileObjectData.GetTileData(Main.tile[i, j]) is null)
                MachineTE.BlockPlacement(i, j);
        }

        // PlaceInWorld is NOT called on tile swap. 
        // As a temporary fix, tile swap is disabled entirely for machines.
        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced) => false;
    }
}
