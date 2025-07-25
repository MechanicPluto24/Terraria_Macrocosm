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

namespace Macrocosm.Content.Skies.EarthOrbit;

public class EarthOrbitSky : CustomSky, ILoadable
{
    public bool Background3D { get; set; } = true;

    private bool active;
    private float intensity;

    private readonly Stars stars;

    private readonly CelestialBodySprite moon;
    private readonly CelestialBodySprite sun;

    private readonly Asset<Texture2D> skyTexture;
    private readonly Asset<Texture2D> sunTexture;

    private static List<Asset<Texture2D>> earthBackgrounds;
    private static Asset<Texture2D> earthBackground;

    private static Asset<Texture2D> earthMercator;
    private static Asset<Texture2D> earthMercatorClouds;
    private static Asset<Texture2D> earthAtmo;

    private Mesh earthMesh;
    private Mesh earthCloudMesh;

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

        earthMercator = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_Mercator");
        earthMercatorClouds = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_MercatorClouds");
        earthAtmo = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_Atmo");

        stars = new();

        sun = new(sunTexture);
        moon = new(TextureAssets.Moon[Main.moonType]);
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

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.NonPremultiplied, SamplerState.LinearClamp, state);

            if (Background3D)
                DrawEarth3D(spriteBatch);
            else
                DrawEarth2D(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }

    private void DrawEarth3D(SpriteBatch spriteBatch)
    {
        float x = -(float)((Main.screenPosition.X / Main.maxTilesX * 16.0) + 800);
        float y = -(float)((Main.screenPosition.Y / Main.maxTilesY * 16.0) - 50);

        float depthFactor = 16f;
        float radius = 1580 * depthFactor;
        Vector2 position = new(x, y);

        earthMesh ??= new Mesh();
        earthCloudMesh ??= new Mesh();

        earthMesh.CreateSphere(
            position: default,
            radius: radius,
            horizontalResolution: 100,
            verticalResolution: 100,
            depthFactor: depthFactor,
            rotation: new Vector3(
                x: 0,
                y: (float)Main.timeForVisualEffects / 6000 % MathHelper.TwoPi, // axial rotation
                z: MathHelper.ToRadians(23.44f)), // axial tilt
            projectionType: Mesh.SphereProjectionType.Mercator
        );

        earthCloudMesh.CreateSphere(
              position: default,
              radius: radius,
              horizontalResolution: 100,
              verticalResolution: 100,
              depthFactor: depthFactor,
              rotation: new Vector3(
                  x: 0,
                  y: (float)Main.timeForVisualEffects / 5500 % MathHelper.TwoPi, // axial rotation
                  z: MathHelper.ToRadians(23.44f)), // axial tilt
              projectionType: Mesh.SphereProjectionType.Mercator
        );

        Effect pixelate = Macrocosm.GetShader("Pixelate");
        pixelate.Parameters["uPixelCount"].SetValue(new Vector2(1024));
        Texture2D earth = earthMesh.DrawToRenderTarget(earthMercator.Value.ApplyEffects(pixelate), state.Matrix);
        Texture2D earthClouds = earthCloudMesh.DrawToRenderTarget(earthMercatorClouds.Value.ApplyEffects(pixelate), state.Matrix, blendState: BlendState.NonPremultiplied);

        Vector2 center = position + earth.Size() / 2f;
        Vector3 lightPosition;
        float depth = 1600f;
        lightPosition = new Vector3(sun.Center, depth);
        radius = 0.01f;

        Effect lighting = Macrocosm.GetShader("SphereLighting");
        lighting.Parameters["uLightSource"].SetValue(lightPosition);
        lighting.Parameters["uEntityPosition"].SetValue(center);
        lighting.Parameters["uTextureSize"].SetValue(earth.Size());
        lighting.Parameters["uEntitySize"].SetValue(earth.Size());
        lighting.Parameters["uRadius"].SetValue(radius);
        lighting.Parameters["uPixelSize"].SetValue(1);
        lighting.Parameters["uColor"].SetValue(Color.White.ToVector4());

        spriteBatch.Draw(earth.ApplyEffects(lighting), new Vector2(x, y), Color.White);
        spriteBatch.Draw(earthClouds.ApplyEffects(null), new Vector2(x, y), Color.White);
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

    // CelestialBody.Rotate is NOT working for drawing under the surface - What ?
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