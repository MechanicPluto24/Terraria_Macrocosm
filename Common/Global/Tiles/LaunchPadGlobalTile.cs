using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Tiles.Special;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Tiles
{
    public class LaunchPadGlobalTile : GlobalTile
    {
        public override void Load()
        {
            On_WorldGen.SpawnFallingBlockProjectile += On_WorldGen_SpawnFallingBlockProjectile;
            On_Wiring.DeActive += On_Wiring_DeActive;
        }

        public override void Unload()
        {
            On_WorldGen.SpawnFallingBlockProjectile -= On_WorldGen_SpawnFallingBlockProjectile;
            On_Wiring.DeActive -= On_Wiring_DeActive;
        }

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
                    RocketManager.Rockets[launchPad.RocketID].Launch(targetWorld: MacrocosmSubworld.CurrentID);
            }
        }


        public override bool CanPlace(int i, int j, int type)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _) && type >= 0 && Main.tileSolid[type])
                return false;

            return true;
        }

        // Check if the tile is not on a launchpad or has a launchpad above it
        private bool CanTileBeAltered(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            // Check if the tile is directly on a launchpad. Only solid tiles can not be altered.
            // LaunchpadMarkers themselves do their own check in their class (can't be broken if there's a Rocket or items in the Assembly)
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _))
            {
                if (Main.tileSolid[tile.TileType])
                    return false;
            }

            // Tiles below the launchpad can't be altered at all
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                return false;
            }

            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!CanTileBeAltered(i, j))
                fail = true;
        }

        public override void NumDust(int i, int j, int type, bool fail, ref int num)
        {
            if(!CanTileBeAltered(i, j))
                num = 0;
        }

        public override bool KillSound(int i, int j, int type, bool fail)
        {
            if (!CanTileBeAltered(i, j))
                return false;

            return true;
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (!CanTileBeAltered(i, j))
                resetFrame = false;

            return true;
        }

        // Disable tile actuation for launch pads and tiles below them
        private void On_Wiring_DeActive(On_Wiring.orig_DeActive orig, int i, int j)
        {
            if (!CanTileBeAltered(i, j))
                return;

            orig(i, j);
        }

        // Disable sandfall for launch pads and tiles below them
        private bool On_WorldGen_SpawnFallingBlockProjectile(On_WorldGen.orig_SpawnFallingBlockProjectile orig, int i, int j, Tile tileCache, Tile tileTopCache, Tile tileBottomCache, int type)
        {
            if (!CanTileBeAltered(i, j))
                return false;

            return orig(i, j, tileCache, tileTopCache, tileBottomCache, type);
        }

        // Disable sloping for launch pads and tiles below them
        public override bool Slope(int i, int j, int type) => CanTileBeAltered(i, j);

        // Disable explosions for launch pads and tiles below them
        public override bool CanExplode(int i, int j, int type) => CanTileBeAltered(i, j);
    }
}
