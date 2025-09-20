using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

public class ConveyorPlayer : ModPlayer
{
    public override void PostUpdate()
    {
        if (Main.dedServ || Player.whoAmI != Main.myPlayer)
            return;

        if (!ConveyorSystem.ShouldDraw)
            return;

        Player player = Main.LocalPlayer;
        Item held = player?.HeldItem;
        if (held is not null && held.hammer > 0 && player.itemAnimation > 0 && player.ItemTimeIsZero && player.controlUseItem)
        {
            Point target = player.TargetCoords();
            if (WorldGen.InWorld(target.X, target.Y))
            {
                bool rotated = false;
                rotated |= ConveyorSystem.TryRotateDropper(target.X, target.Y);
                rotated |= ConveyorSystem.TryRotateHopper(target.X, target.Y);
                if (rotated)
                {
                    player.ApplyItemTime(held);
                }
            }
        }
    }
}
