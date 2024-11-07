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
            On_WorldGen.KillTile_MakeTileDust += On_WorldGen_KillTile_MakeTileDust;
            On_WorldGen.KillTile_PlaySounds += On_WorldGen_KillTile_PlaySounds;
            On_WorldGen.SpawnFallingBlockProjectile += On_WorldGen_SpawnFallingBlockProjectile;
            On_Wiring.DeActive += On_Wiring_DeActive;
        }

        public override void Unload()
        {
            On_WorldGen.KillTile_MakeTileDust -= On_WorldGen_KillTile_MakeTileDust;
            On_WorldGen.KillTile_PlaySounds -= On_WorldGen_KillTile_PlaySounds;
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

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                bool canKillMarker = launchPad.Inventory.IsEmpty && !launchPad.HasRocket;

                if (ItemSets.DeveloperItem[Main.LocalPlayer.CurrentItem().type])
                    canKillMarker = true;

                if (Main.tile[i, j].TileType == ModContent.TileType<LaunchPadMarker>() && !canKillMarker)
                    fail = true;
            }

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                fail = true;
            }
        }

        // Check if the tile is not on a launchpad or has a launchpad above it
        private bool CanTileBeAltered(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                if (tile.TileType != ModContent.TileType<LaunchPadMarker>())
                    return false;
            }

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                return false;
            }

            return true;
        }

        // Disable tile actuation for launch pads and tiles below them
        private void On_Wiring_DeActive(On_Wiring.orig_DeActive orig, int i, int j)
        {
            if (CanTileBeAltered(i, j))
                orig(i, j);
        }

        // Disable sandfall for launch pads and tiles below them
        private bool On_WorldGen_SpawnFallingBlockProjectile(On_WorldGen.orig_SpawnFallingBlockProjectile orig, int i, int j, Tile tileCache, Tile tileTopCache, Tile tileBottomCache, int type)
        {
            if (CanTileBeAltered(i, j))
                return orig(i, j, tileCache, tileTopCache, tileBottomCache, type);

            return false;
        }

        // Disable sloping for launch pads and tiles below them
        public override bool Slope(int i, int j, int type) => CanTileBeAltered(i, j);

        // Disable explosions for launch pads and tiles below them
        public override bool CanExplode(int i, int j, int type) => CanTileBeAltered(i, j);

        // Disable tile break sounds for launch pads and tiles below them
        private void On_WorldGen_KillTile_PlaySounds(On_WorldGen.orig_KillTile_PlaySounds orig, int i, int j, bool fail, Tile tileCache)
        {
            if (CanTileBeAltered(i, j))
                orig(i, j, fail, tileCache);
        }

        // Disable tile dust for launch pads and tiles below them
        private int On_WorldGen_KillTile_MakeTileDust(On_WorldGen.orig_KillTile_MakeTileDust orig, int i, int j, Tile tileCache)
        {
            if (CanTileBeAltered(i, j))
                return orig(i, j, tileCache);

            return 6000;
        }

        /*
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _))
            {
                blockDamaged = false;
                return false;
            }

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                blockDamaged = false;
                return false;
            }

            return true;
        }
        */
    }
}
