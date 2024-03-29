using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineTile : ModTile
    {
        public abstract short Width { get; }
        public abstract short Height { get; }

        public abstract MachineTE MachineTE { get; }

        public virtual bool IsOperating(int i, int j) => false;
        public virtual bool GetPowerState(int i, int j) => false;
        public virtual void TogglePowerState(int i, int j) { }
    }
}
