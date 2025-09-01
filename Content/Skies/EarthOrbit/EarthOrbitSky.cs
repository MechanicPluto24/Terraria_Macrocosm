using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using static Macrocosm.Common.Drawing.Sky.CelestialBody;

namespace Macrocosm.Content.Skies.EarthOrbit
{
        // Only load visuals on the client.
    [Autoload(Side = ModSide.Client)]
    public class EarthOrbitSky : CustomSky, ILoadable
    {
        private bool active;
        private float intensity;

        private readonly Stars stars;

        private CelestialBodySphere earth3D;
        private readonly CelestialBodySprite moon;
        private readonly CelestialBodySprite sun;

        private readonly Asset<Texture2D> skyTexture;
        private readonly Asset<Texture2D> sunTexture;

        private static List<Asset<Texture2D>> earthBackgrounds;
        private static Asset<Texture2D> earthBackground;
        private static Asset<Texture2D> earthAtmo;

        private static Asset<Texture2D> earthMercator;
        private static Asset<Texture2D> earthMercatorOverlay;

        private const string Path = "Macrocosm/Content/Skies/EarthOrbit/";

        public EarthOrbitSky()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            skyTexture = ModContent.Request<Texture2D>(Path + "EarthOrbitSky", mode);
            sunTexture = ModContent.Request<Texture2D>(Path + "Sun", mode);
            earthBackgrounds = [
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Earth_Africa"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Earth_Asia"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Earth_Australia"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Earth_Europe"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Earth_NorthAmerica"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Earth_SouthAmerica")
            ];

            earthAtmo = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_Atmo");

            earthMercator = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_Mercator");
            earthMercatorOverlay = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_MercatorClouds");

            stars = new();

            sun = new(sunTexture);
            moon = new(TextureAssets.Moon[Main.moonType]);

                // Please tweak these values.
            earth3D = new(earthMercator, new(3000), projectionOverlayTexture: earthMercatorOverlay)
            { ConfigureSphereShader = ConfigureEarthSphereShader };

            earth3D.SetLighting(sun);

            earth3D.SetParallax(0f, 0f);
            earth3D.SetPosition(Main.screenWidth * .7f, Main.screenHeight * 1.85f);
        }

        public void Load(Mod mod) =>
            SkyManager.Instance["Macrocosm:EarthOrbitSky"] = new EarthOrbitSky();

        public void Unload() { }

        public override void Reset()
        {
        }

        public override bool IsActive() => active;

        public override void Activate(Vector2 position, params object[] args)
        {
            intensity = 0.002f;

            earthBackground = earthBackgrounds[Utility.RealTimeCycle(earthBackgrounds.Count, 1800)];

            Rectangle area = new(
                0,
                -Main.screenHeight,
                Main.screenWidth,
                Main.screenHeight * 2
            );

            stars.SpawnStars(450 * 5, area: area, randomColor: true, baseScale: 0.8f, twinkleFactor: 0.05f);
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
                double dayLength = MacrocosmSubworld.GetDayLength();
                if (Main.time < dayLength * 0.1)
                    return Color.Lerp(darkColor, Color.White, (float)(Main.time / (dayLength * 0.1)));
                else if (Main.time > dayLength * 0.9)
                    return Color.Lerp(darkColor, Color.White, (float)((dayLength - Main.time) / (dayLength - dayLength * 0.9)));
                else
                    return Color.White;
            }
            else
            {
                double nightLength = MacrocosmSubworld.GetNightLength();
                if (Main.time < nightLength * 0.2)
                    return Color.Lerp(darkColor, moonLightColor, (float)(Main.time / (nightLength * 0.2)));
                else if (Main.time > nightLength * 0.8)
                    return Color.Lerp(darkColor, moonLightColor, (float)((nightLength - Main.time) / (nightLength - nightLength * 0.8)));
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

            float bgTopY = (float)(((Main.screenPosition.Y - Main.screenHeight / 2)) / (Main.maxTilesY * 16.0) * 0.2f * Main.screenHeight) * 0.5f;
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

                RotateSunAndMoon();

                if (ClientConfig.Instance.Use3DCelestialBodies)
                    earth3D.Draw(spriteBatch);
                else
                {
                    state.SaveState(spriteBatch);

                    spriteBatch.End();
                    spriteBatch.Begin(BlendState.NonPremultiplied, SamplerState.PointClamp, state);

                    DrawEarth2D(spriteBatch);

                    spriteBatch.End();
                    spriteBatch.Begin(state);
                }
            }
        }

        private void ConfigureEarthSphereShader(
            CelestialBodySphere celestialBody,
            CelestialBody lightSource,
            out Vector3 lightPosition,
            out Vector4 lightColor,
            out Vector4 shadowColor,
            out Vector4 lightAtmosphereColor,
            out Vector4 shadowAtmosphereColor,
            out Matrix bodyRotation,
            out float radius)
        {
            float depth = 1600f;
            lightPosition = new Vector3(sun.Center, depth);

                // These are test/dummy values feel free to mess with these all you like.
            lightColor = Vector4.One;
            shadowColor = Color.Black.ToVector4();

            bodyRotation =
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateRotationY(MathHelper.Pi) *
                Matrix.CreateRotationZ(Main.GlobalTimeWrappedHourly * .2f);

            lightAtmosphereColor = Color.Blue.ToVector4();
            shadowAtmosphereColor = Color.DarkRed.ToVector4() * .6f;

                // Value from 0-1 representing the ratio between the atmosphere and actual body.
            radius = .96f;

            earth3D.SetParallax(0.01f, 0.12f, new Vector2(800f, 300f));
        }

        private static void DrawEarth2D(SpriteBatch spriteBatch)
        {
            float bgTopY = -(float)((Main.screenPosition.Y / Main.maxTilesY * 16.0) - earthBackground.Height() / 2);
            spriteBatch.Draw
            (
                earthBackground.Value,
                new System.Drawing.RectangleF(0, bgTopY, Main.screenWidth, Main.screenHeight),
                GetLightColor().WithAlpha(255)
            );
        }

        // CelestialBody.Rotate is NOT working for drawing under the surface
        private void RotateSunAndMoon()
        {
            float bgTopY = (float)(((Main.screenPosition.Y - Main.screenHeight / 2)) / (Main.maxTilesY * 16.0) * 0.2f * Main.screenHeight) * 0.5f;
            double duration = Main.dayTime ? MacrocosmSubworld.GetDayLength() : MacrocosmSubworld.GetNightLength();

            if (Main.dayTime)
            {
                double progress = Main.dayTime ? Main.time / duration : 1.0 - Main.time / duration;
                int timeX = (int)(progress * (Main.screenWidth + sun.Width * 2)) - (int)sun.Width;

                double timeY = Main.time < duration / 2
                    ? Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) // AM
                    : Math.Pow(1.0 - Main.time / duration * 2.0, 2.0);   // PM

                sun.Rotation = (float)(Main.time / duration) * 2f - 7.3f;
                sun.Scale = (float)(1.2 - timeY * 0.4);

                sun.Color = Color.White;

                int posY = Main.dayTime ? (int)(bgTopY + timeY * 250.0 + 180.0) : (int)(bgTopY - timeY * 250.0 + 665.0);

                sun.SetupSkyRotation(SkyRotationMode.None);
                sun.Center = new Vector2(timeX, posY);
                sun.Draw(Main.spriteBatch);
            }
            else
            {
                double progress = !Main.dayTime ? Main.time / duration : 1.0 - Main.time / duration;
                int timeX = (int)(progress * (Main.screenWidth + moon.Width * 2)) - (int)moon.Width;

                double timeY = Main.time < duration / 2
                    ? Math.Pow(1.0 - Main.time / duration * 2.0, 2.0)
                    : Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0);

                moon.Rotation = (float)(Main.time / duration) * 2f - 7.3f;
                moon.Scale = (float)(1.2 - timeY * 0.4);

                moon.Color = Color.White;
                int posY = !Main.dayTime ? (int)(bgTopY + timeY * 250.0 + 360.0) : (int)(bgTopY - timeY * 250.0 + 665.0);

                moon.SetupSkyRotation(SkyRotationMode.None);
                moon.Center = new Vector2(timeX, posY);
                moon.Draw(Main.spriteBatch);
            }
        }
    }
}