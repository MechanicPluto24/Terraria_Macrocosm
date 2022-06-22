using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Graphics.Effects;

namespace Macrocosm.Content.Subworlds.Moon
{
    /// <summary>
    /// Moon terrain and crater generation by 4mbr0s3 2
    /// Why isn't anyone else working on this
    /// I have saved the day - Ryan
    /// </summary>
    public class Moon : Subworld
    {
        
        public override int Width => 2000;
        public override int Height => 1200; // 200 tile padding for the hell-layer.
        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            new MoonGen("LoadingMoon", 1f, this)
        };

        //public override UIState loadingUIState => new MoonSubworldLoadUI();
        //public override ModWorld modWorld => null;
        //public override bool saveModData => true;


        public override void OnEnter()
        {
            SkyManager.Instance.Activate("Macrocosm:MoonSky");
        }

        public override void OnExit()
        {
            SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
        }


        public override void Load()
        {
            // One Terraria day = 86400
            SubworldSystem.hideUnderworld = true;
            SubworldSystem.noReturn = true;
            Main.dayTime = true;
            Main.numClouds = 0;
        }
    }
}
