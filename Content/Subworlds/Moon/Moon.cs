using Macrocosm.Backgrounds.Moon;
using Macrocosm.Content.Subworlds.Moon.Generation;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.WorldBuilding;


namespace Macrocosm.Content.Subworlds.Moon {
    /// <summary>
    /// Moon terrain and crater generation by 4mbr0s3 2
    /// Why isn't anyone else working on this
    /// I have saved the day - Ryan
    /// </summary>
    public class Moon : Subworld {
        public override bool NormalUpdates => true;
        //public override int Width => 2000;
        public override int Width => 2200;
        public override int Height => 1200; 
        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            genGroundPass,
            new CraterPass("CraterPass", 1f),
            new BackgroundPass("BackgroundPass", 1f),
            new RegolithPass("RegolithPass", 5f),
            new OrePass("OrePass", 0.75f),
            new CavePass("CavePass", 1f, genGroundPass.RockLayerHigh, genGroundPass.RockLayerHigh),
            new ScuffedSmoothPass("ScuffedSmoothPass", 1f),
            new AmbientPass("AmbientPass", 0.2f),
            new FinishPass("FinishPass", 0.1f)
        };

        public const float TimeRate = 0.125f;

        private MoonSubworldLoadUI moonLoadUI;
        private GroundPass genGroundPass;

        public Moon() {
            this.moonLoadUI = new();
            this.genGroundPass = new GroundPass("GroundPass", 8f, Width, Height);
        }

        public override void OnEnter() {
            moonLoadUI.Setup(toEarth: false);
            SkyManager.Instance.Activate("Macrocosm:MoonSky");
            MoonSky.SpawnStarsOnMoon();
        }

        public override void OnExit() {
            moonLoadUI.Setup(toEarth: true);
            SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
            MoonSky.ClearStarsOnMoon();
        }

        public override void Load() {
            SubworldSystem.hideUnderworld = true;
            SubworldSystem.noReturn = true;
            Main.numClouds = 0;
            Main.raining = false;
        }

        public override void DrawSetup(GameTime gameTime) {
            PlayerInput.SetZoom_Unscaled();
            Main.instance.GraphicsDevice.Clear(Color.Black);
            DrawMenu(gameTime);
        }

        public override void DrawMenu(GameTime gameTime) {
            moonLoadUI.DrawSelf(Main.spriteBatch);
        }
    }
}
