using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Common.Systems.Power;

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
            const int padding = 64;
            Rectangle visibleWorldBounds = new(
                (int)Main.screenPosition.X - padding,
                (int)Main.screenPosition.Y - padding,
                Main.screenWidth + padding * 2,
                Main.screenHeight + padding * 2
            );

            foreach (var kvp in ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    Rectangle machineWorldBounds = new(
                        machine.Position.X * 16,
                        machine.Position.Y * 16,
                        machine.MachineTile.Width * 16,
                        machine.MachineTile.Height * 16
                    );

                    if (!visibleWorldBounds.Intersects(machineWorldBounds))
                        continue;

                    machine.DrawMachinePowerInfo(Main.spriteBatch, machine.Position.ToWorldCoordinates(), Lighting.GetColor(machine.Position.ToPoint()));
                }
            }
        }
    }
}
