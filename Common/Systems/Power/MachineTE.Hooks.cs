using Macrocosm.Common.Utils;
using Terraria;

namespace Macrocosm.Common.Systems.Power
{
    public partial class MachineTE
    {
        public override void Load()
        {
            On_Main.DrawWires += On_Main_DrawWires;
        }

        public override void Unload()
        {
            On_Main.DrawWires -= On_Main_DrawWires;
        }

        private void On_Main_DrawWires(On_Main.orig_DrawWires orig, Main self)
        {
            orig(self);

            if (Main.LocalPlayer.CurrentItem().mech)
            {
                foreach (var kvp in ByID)
                {
                    if (kvp.Value is MachineTE machine)
                    {
                        machine.DrawMachinePowerInfo(Main.spriteBatch, machine.Position.ToWorldCoordinates(), Lighting.GetColor(machine.Position.ToPoint()));
                    }
                }
            }
        }
    }
}
