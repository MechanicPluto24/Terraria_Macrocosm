using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineTE : ModTileEntity
    {
        public abstract MachineTile MachineTile { get; }

        public abstract bool Operating { get; }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == MachineTile.Type;
        }

        /// <summary> Things to happen before the first update tick. Only runs on SP and Server. </summary>
        public virtual void OnFirstUpdate() { }

        /// <summary> Update hook. Only runs on SP and Server </summary>
        public virtual void MachineUpdate() { }

        private bool ranFirstUpdate = false;
        public sealed override void Update()
        {
            if (!ranFirstUpdate)
            {
                OnFirstUpdate();
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
                ranFirstUpdate = true;
            }

            MachineUpdate();
        }
    }
}
