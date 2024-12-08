using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Skies.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Map;
using Terraria.ModLoader;

namespace Macrocosm.Content.Skies.EarthOrbit
{
    public class EarthOrbitSky : CustomSky, ILoadable
    {
        private bool active;
        private float intensity;

        private readonly Stars stars;

        private readonly CelestialBody moon;
        private readonly CelestialBody sun;

        private readonly Asset<Texture2D> skyTexture;
        private readonly Asset<Texture2D> sunTexture;
        private readonly Asset<Texture2D> earthTexture;

        private const string Path = "Macrocosm/Content/Skies/EarthOrbit/";

        public EarthOrbitSky()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            skyTexture = ModContent.Request<Texture2D>(Path + "EarthOrbitSky", mode);
            sunTexture = ModContent.Request<Texture2D>(Path + "Sun", mode);
            earthTexture = ModContent.Request<Texture2D>(Path + "Earth", mode);

            stars = new();
            stars.SpawnStars(850, randomColor: true, baseScale: 0.8f, twinkleFactor: 0.05f);

            sun = new CelestialBody(sunTexture);
            moon = new CelestialBody(TextureAssets.Moon[Main.moonType]);

            sun.SetupSkyRotation(CelestialBody.SkyRotationMode.Day);
            moon.SetupSkyRotation(CelestialBody.SkyRotationMode.Night);
        }

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            SkyManager.Instance["Macrocosm:EarthOrbitSky"] = new EarthOrbitSky();
        }

        public void Unload() { }

        public override void Reset()
        {
        }

        public override bool IsActive() => active;

        public override void Activate(Vector2 position, params object[] args)
        {
            intensity = 0.002f;
            active = true;
        }

        public override void Deactivate(params object[] args)
        {
            intensity = 0f;
            stars.Clear();
            active = false;
        }

        public override float GetCloudAlpha() => 0f;
        public override Color OnTileColor(Color inColor) => GetLightColor();

        private static Color GetLightColor()
        {
            Color darkColor = new Color(35, 35, 35);
            Color moonLightColor = new Color(85, 85, 85) * Utility.GetMoonPhaseBrightness(Main.GetMoonPhase());

            if (Main.dayTime)
            {
                if (Main.time < MacrocosmSubworld.CurrentDayLength * 0.1)
                    return Color.Lerp(darkColor, Color.White, (float)(Main.time / (MacrocosmSubworld.CurrentDayLength * 0.1)));
                else if (Main.time > MacrocosmSubworld.CurrentDayLength * 0.9)
                    return Color.Lerp(darkColor, Color.White, (float)((MacrocosmSubworld.CurrentDayLength - Main.time) / (MacrocosmSubworld.CurrentDayLength - MacrocosmSubworld.CurrentDayLength * 0.9)));
                else
                    return Color.White;
            }
            else
            {
                if (Main.time < MacrocosmSubworld.CurrentNightLength * 0.2)
                    return Color.Lerp(darkColor, moonLightColor, (float)(Main.time / (MacrocosmSubworld.CurrentNightLength * 0.2)));
                else if (Main.time > MacrocosmSubworld.CurrentNightLength * 0.8)
                    return Color.Lerp(darkColor, moonLightColor, (float)((MacrocosmSubworld.CurrentNightLength - Main.time) / (MacrocosmSubworld.CurrentNightLength - MacrocosmSubworld.CurrentNightLength * 0.8)));
                else
                    return moonLightColor;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!SubworldSystem.IsActive<EarthOrbitSubworld>())
                active = false;

            intensity = active ? Math.Min(1f, intensity + 0.01f) : Math.Max(0f, intensity - 0.01f);

            sun.Color = new Color(255, 255, 255);
            float bgTopY = (float)(-(Main.screenPosition.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 50.0);
            stars.CommonOffset = new Vector2(0, bgTopY);

            moon.SetSourceRectangles(bodySourceRect: TextureAssets.Moon[Main.moonType].Frame(verticalFrames: 8, frameY: Main.moonPhase));
            moon.SetTextures(TextureAssets.Moon[Main.moonType]);
        }

        private SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SubworldSystem.IsActive<EarthOrbitSubworld>() && maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Main.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Draw(skyTexture.Value, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * intensity);

                stars.DrawAll(spriteBatch);
                sun.Draw(spriteBatch);
                moon.Draw(spriteBatch);

                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(BlendState.NonPremultiplied, state);

                float bgTopY = -(float)((Main.screenPosition.Y / Main.maxTilesY * 16.0) - earthTexture.Height() / 2);
                spriteBatch.Draw
                (
                    earthTexture.Value,
                    new System.Drawing.RectangleF(0, bgTopY, Main.screenWidth, Main.screenHeight),
                    GetLightColor()
                );

                spriteBatch.End();
                spriteBatch.Begin(state);
            }
        }
    }
}