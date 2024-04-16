using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Global;
using Macrocosm.Content.Tiles.Special;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public class LaunchPadGlobalTile : GlobalTile
    {
        public override void Load()
        {
            On_Player.PlaceThing_Tiles += On_Player_PlaceThing_Tiles;

            On_WorldGen.KillTile_MakeTileDust += On_WorldGen_KillTile_MakeTileDust;
            On_WorldGen.KillTile_PlaySounds += On_WorldGen_KillTile_PlaySounds;
        }

        public override void Unload()
        {
            On_Player.PlaceThing_Tiles -= On_Player_PlaceThing_Tiles;
            On_WorldGen.KillTile_MakeTileDust -= On_WorldGen_KillTile_MakeTileDust;
            On_WorldGen.KillTile_PlaySounds -= On_WorldGen_KillTile_PlaySounds;
        }

        /// <summary> 
        /// Temporary workaround for <see href="https://github.com/tModLoader/tModLoader/issues/3509"/>
        /// Remove once resolved.
        /// </summary>
        private void On_Player_PlaceThing_Tiles(On_Player.orig_PlaceThing_Tiles orig, Player self)
        {
            if (TileLoader.CanPlace(Player.tileTargetX, Player.tileTargetY, self.inventory[self.selectedItem].createTile))
            {
                orig(self);  
            }
        }

        public override bool CanPlace(int i, int j, int type)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out _) && type > 0 && Main.tileSolid[type])
                return false;

            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                bool canKillMarker = launchPad.Inventory.IsEmpty && !launchPad.HasRocket;

                if (Main.LocalPlayer.CurrentItem().ModItem is IDevItem)
                    canKillMarker = true;

                if (Main.tile[i, j].TileType == ModContent.TileType<LaunchPadMarker>() && !canKillMarker)
                    fail = true;
            }

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                fail = true;
            }
        }

        private void On_WorldGen_KillTile_PlaySounds(On_WorldGen.orig_KillTile_PlaySounds orig, int i, int j, bool fail, Tile tileCache)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                if (Main.tile[i, j].TileType != ModContent.TileType<LaunchPadMarker>() || !launchPad.Inventory.IsEmpty)
                    return;
            }

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                return;
            }

            orig(i, j, fail, tileCache);
        }

        private int On_WorldGen_KillTile_MakeTileDust(On_WorldGen.orig_KillTile_MakeTileDust orig, int i, int j, Tile tileCache)
        {
            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j), out LaunchPad launchPad))
            {
                if (Main.tile[i, j].TileType != ModContent.TileType<LaunchPadMarker>() || !launchPad.Inventory.IsEmpty)
                    return 6000;
            }

            if (LaunchPadManager.TryGetLaunchPadAtTileCoordinates(MacrocosmSubworld.CurrentID, new(i, j - 1), out _))
            {
                return 6000;
            }

            return orig(i, j, tileCache);
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
