using Macrocosm.Content.BuilderToggles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power;

public class MachinePowerOverlaySystem : ModSystem
{
    public override void PostDrawTiles()
    {
        if (!MachinePowerOverlayBuilderToggle.IsEnabled)
            return;

        const int padding = 64;
        Rectangle visibleWorldBounds = new(
            (int)Main.screenPosition.X - padding,
            (int)Main.screenPosition.Y - padding,
            Main.screenWidth + padding * 2,
            Main.screenHeight + padding * 2
        );

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        foreach (var kvp in TileEntity.ByID)
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

        Main.spriteBatch.End();
    }
}
