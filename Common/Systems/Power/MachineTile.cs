using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineTile : ModTile
    {
        public abstract short Width { get; }
        public abstract short Height { get; }

        public abstract MachineTE MachineTE { get; }

        public virtual bool IsPoweredUpFrame(int i, int j) => false;
        public virtual bool IsOperatingFrame(int i, int j) => IsPoweredUpFrame(i, j);
        public virtual void TogglePowerStateFrame(int i, int j) { }
    }
}
