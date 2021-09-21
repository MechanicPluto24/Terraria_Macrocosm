using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.World.Generation;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Subworlds.Moon
{
    /// <summary>
    /// Moon terrain and crater generation by 4mbr0s3 2
    /// Why isn't anyone else working on this
    /// I have saved the day - Ryan
    /// </summary>
    public class Moon : Subworld
    {
        public override int width => 2000;
        public override int height => 1200; // 200 tile padding for the hell-layer.

        public override ModWorld modWorld => null;

        public override bool saveSubworld => true;
        public override bool disablePlayerSaving => false;
        public override bool saveModData => true;
        public override List<GenPass> tasks => new MoonGen(this);
        public override UIState loadingUIState => new MoonSubworldLoadUI();

        public override void Load()
        {
            // One Terraria day = 86400
            SLWorld.drawUnderworldBackground = false;
            SLWorld.noReturn = true;
            Main.dayTime = true;
            Main.spawnTileX = 1000;
            for (int tileY = 0; tileY < Main.maxTilesY; tileY++)
            {
                if (Main.tile[1000, tileY].active())
                {
                    Main.spawnTileY = tileY;
                    break;
                }
            }
            Main.numClouds = 0;
        }
    }
}
