using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineTile : ModTile
    {
        public abstract short Width { get; }
        public abstract short Height { get; }

        public abstract MachineTE MachineTE { get; }

        public virtual bool IsPoweredOnFrame(int i, int j) => false;
        public virtual bool IsOperatingFrame(int i, int j) => IsPoweredOnFrame(i, j);
        public virtual void ToggleStateFrame(int i, int j) { }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            MachineTE.Kill(i, j);
        }
    }
}
