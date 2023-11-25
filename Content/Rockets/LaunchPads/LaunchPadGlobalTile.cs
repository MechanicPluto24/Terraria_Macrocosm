
using Macrocosm.Common.Subworlds;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public class LaunchPadGlobalTile : GlobalTile
    {
        public override void HitWire(int i, int j, int type)
        {
            if (LaunchPadManager.None(MacrocosmSubworld.CurrentID))
                return;

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                for (int tileX = launchPad.StartTile.X; tileX < launchPad.EndTile.X; tileX++)
                    for (int tileY = launchPad.StartTile.Y; tileY < launchPad.EndTile.Y; tileY++)
                        Wiring.SkipWire(tileX, tileY);

                if (launchPad.HasRocket)
                    RocketManager.Rockets[launchPad.RocketID].Launch(targetWorld: "None.. for now");
            }
        }
    }
}
