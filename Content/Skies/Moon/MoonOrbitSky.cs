using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using static Macrocosm.Common.Drawing.Sky.CelestialBody;

namespace Macrocosm.Content.Skies.Moon;

public class MoonOrbitSky : CustomSky, ILoadable
{
    public bool Background3D { get; set; } = true;

    private bool active;
    private float intensity;

    private readonly Stars stars;

    private readonly CelestialBodySprite earth;
    private readonly CelestialBodySprite sun;

    private readonly Asset<Texture2D> skyTexture;

    private readonly Asset<Texture2D> sunTexture;

    private readonly Asset<Texture2D> earthBody;
    private readonly Asset<Texture2D> earthBodyDrunk;
    private readonly Asset<Texture2D> earthBodyFlat;
    private readonly Asset<Texture2D> earthAtmo;

    private static Asset<Texture2D> moonBackground;

    private static Asset<Texture2D> moonMercator;
    private Mesh moonMesh;

    private readonly Asset<Texture2D>[] nebulaTextures = new Asset<Texture2D>[Main.maxMoons];
    private readonly RawTexture[] nebulaRawTextures = new RawTexture[Main.maxMoons];

    private const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
    private const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten

    private bool shouldRefreshNebulaStars = true;
    private int lastMoonType = 0;

    private const string Path = "Macrocosm/Content/Skies/Moon/";

    public MoonOrbitSky()
    {
        //if (Main.dedServ)
        //   return;

        AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
        skyTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Skies/EarthOrbit/EarthOrbitSky", mode);

        sunTexture = ModContent.Request<Texture2D>(Path + "Sun", mode);
        earthBody = ModContent.Request<Texture2D>(Path + "Earth", mode);
        earthBodyDrunk = ModContent.Request<Texture2D>(Path + "EarthDrunk", mode);
        earthBodyFlat = ModContent.Request<Texture2D>(Path + "EarthFlat", mode);

        earthAtmo = ModContent.Request<Texture2D>(Path + "EarthAtmo", mode);

        moonBackground = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/2D/Luna", mode);
        moonMercator = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Luna_Mercator", mode);

        stars = new();

        sun = new(sunTexture);
        earth = new(earthBody, earthAtmo, scale: 0.9f);

        earth.SetLighting(sun);
        earth.ConfigureBackRadialShader = ConfigureEarthAtmoShader;
        earth.ConfigureBodySphericalShader = ConfigureEarthBodyShader;

        string[] nebulaNames =
        [
            "NebulaNormal",
            "NebulaYellow",
            "NebulaRinged",
            "NebulaMythril",
            "NebulaBlue",
            "NebulaGreen",
            "NebulaPink",
            "NebulaOrange",
            "NebulaPurple"
        ];

        for (int i = 0; i < Main.maxMoons; i++)
            nebulaTextures[i] = ModContent.Request<Texture2D>($"{Path}{nebulaNames[i]}", mode);

        for (int i = 0; i < Main.maxMoons; i++)
            nebulaRawTextures[i] = RawTexture.FromAsset(nebulaTextures[i]);
    }

    public void Load(Mod mod)
    {
        if (Main.dedServ)
            return;

        SkyManager.Instance["Macrocosm:MoonOrbitSky"] = new MoonOrbitSky();
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
        shouldRefreshNebulaStars = true;
    }

    public override float GetCloudAlpha() => 0f;

    public override Color OnTileColor(Color inColor) => GetLightColor();
    public static Color GetLightColor()
    {
        Color darkColor = new(35, 35, 35);
        Color earthshineBlue = Color.Lerp(new Color(39, 87, 155), darkColor, 0.6f);

        if (Main.dayTime)
        {
            if (Main.time < MacrocosmSubworld.GetDayLength() * 0.1)
                return Color.Lerp(darkColor, Color.White, (float)(Main.time / (MacrocosmSubworld.GetDayLength() * 0.1)));
            else if (Main.time > MacrocosmSubworld.GetDayLength() * 0.9)
                return Color.Lerp(darkColor, Color.White, (float)((MacrocosmSubworld.GetDayLength() - Main.time) / (MacrocosmSubworld.GetDayLength() - MacrocosmSubworld.GetDayLength() * 0.9)));
            else
                return Color.White;

        }
        else
        {
            if (Main.time < MacrocosmSubworld.GetNightLength() * 0.2)
                return Color.Lerp(darkColor, earthshineBlue, (float)(Main.time / (MacrocosmSubworld.GetNightLength() * 0.2)));
            else if (Main.time > MacrocosmSubworld.GetNightLength() * 0.8)
                return Color.Lerp(darkColor, earthshineBlue, (float)((MacrocosmSubworld.GetNightLength() - Main.time) / (MacrocosmSubworld.GetNightLength() - MacrocosmSubworld.GetNightLength() * 0.8)));
            else
                return earthshineBlue;
        }
    }

    private SpriteBatchState state;
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        if (SubworldSystem.IsActive<Subworlds.MoonOrbitSubworld>() && maxDepth >= float.MaxValue && minDepth < float.MaxValue)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(skyTexture.Value, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * intensity);

            float nebulaBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.2f, 0.3f);
            float nightStarBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0f, 0.8f);

            DrawMoonNebula(nebulaBrightness);

            UpdateNebulaStars();

            stars.DrawAll(spriteBatch);

            sun.Color = new Color((int)(255 * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity)), (int)(255 * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity)), (int)(255 * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity))) * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity);
            if (Main.dayTime)
                sun.Draw(spriteBatch);

            earth.Draw(spriteBatch);
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.NonPremultiplied, state);

            if (Background3D)
                DrawMoon3D(spriteBatch);
            else
                DrawMoon2D(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }

    private static void DrawMoon2D(SpriteBatch spriteBatch)
    {
        float bgTopY = -(float)((Main.screenPosition.Y / Main.maxTilesY * 16.0) - moonBackground.Height() / 2);
        spriteBatch.Draw
        (
            moonBackground.Value,
            new System.Drawing.RectangleF(0, bgTopY, Main.screenWidth, Main.screenHeight),
            GetLightColor().WithAlpha(255)
        );
    }

    private void DrawMoon3D(SpriteBatch spriteBatch)
    {
        float x = -(float)((Main.screenPosition.X / Main.maxTilesX * 16.0) - 350);
        float y = -(float)((Main.screenPosition.Y / Main.maxTilesY * 16.0) - 300);

        float depthFactor = 16f;
        float radius = 800 * depthFactor;
        Vector2 position = new(x, y);

        moonMesh ??= new Mesh();
        moonMesh.CreateSphere(
            position: default,
            radius: radius,
            horizontalResolution: 100,
            verticalResolution: 100,
            depthFactor: depthFactor,
            rotation: new Vector3(
                x: 0,
                y: (float)Main.timeForVisualEffects / 15000 % MathHelper.TwoPi, // axial rotation
                z: 0), // axial tilt
            projectionType: Mesh.SphereProjectionType.Mercator
        );

        Effect pixelate = Macrocosm.GetShader("Pixelate");
        pixelate.Parameters["uPixelCount"].SetValue(new Vector2(1024));
        Texture2D moon = moonMesh.DrawToRenderTarget(moonMercator.Value.ApplyEffects(pixelate), state.Matrix);

        Vector2 center = position + moon.Size() / 2f;
        Vector3 lightPosition;
        float depth = 1600f;
        lightPosition = new Vector3(sun.Center, depth);
        radius = 0.01f;

        Effect lighting = Macrocosm.GetShader("SphereLighting");
        lighting.Parameters["uLightSource"].SetValue(lightPosition);
        lighting.Parameters["uEntityPosition"].SetValue(center);
        lighting.Parameters["uTextureSize"].SetValue(moon.Size());
        lighting.Parameters["uEntitySize"].SetValue(moon.Size());
        lighting.Parameters["uRadius"].SetValue(radius);
        lighting.Parameters["uPixelSize"].SetValue(1);
        lighting.Parameters["uColor"].SetValue(Color.White.ToVector4());

        spriteBatch.Draw(moon.ApplyEffects(lighting), new Vector2(x, y), Color.White);
    }

    private void UpdateNebulaStars()
    {
        if (lastMoonType != Main.moonType)
            shouldRefreshNebulaStars = true;

        lastMoonType = Main.moonType;
        if (shouldRefreshNebulaStars)
        {
            stars.Clear();

            stars.SpawnStars(650, randomColor: true, baseScale: 0.8f, twinkleFactor: 0.05f);

            MacrocosmStar mars = stars.RandStar(); // :) 
            mars.Color = new Color(224, 137, 8);

            stars?.SpawnStars(colorMap: nebulaRawTextures[Main.moonType], 6000, null, baseScale: 0.6f, twinkleFactor: 0.05f);

            shouldRefreshNebulaStars = false;
        }
    }

    private void DrawMoonNebula(float brightness)
    {
        Texture2D nebula = nebulaTextures[Main.moonType].Value;
        Color nebulaColor = (Color.White * brightness).WithAlpha(0);
        float bgTopY = (float)(-(Main.screenPosition.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 50.0);
        Main.spriteBatch.Draw(nebula, new System.Drawing.RectangleF(0, bgTopY, Main.screenWidth, Main.screenHeight), nebulaColor);
    }

    public override void Update(GameTime gameTime)
    {
        if (!SubworldSystem.IsActive<Subworlds.MoonOrbitSubworld>())
            active = false;

        sun.Color = new Color(255, 255, 255) * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity);

        earth.Color = new Color(255, (int)(255 * (1f - (Subworlds.Moon.Instance.DemonSunVisualIntensity * 0.6f))), (int)(255 * (1f - (Subworlds.Moon.Instance.DemonSunVisualIntensity * 0.6f))));
        intensity = active ? Math.Min(1f, intensity + 0.01f) : Math.Max(0f, intensity - 0.01f);
        UpdateTextures();
        RotateSun();
        earth.SetupSkyRotation(SkyRotationMode.None);
        earth.Center = new Vector2((int)(0.3f * (Main.screenWidth + earth.Width * 2)) - (int)earth.Width, (int)(0.5f * (Main.screenHeight + earth.Height * 2)) - (int)earth.Height - 100);
        float bgTopY = (float)(((Main.screenPosition.Y - Main.screenHeight / 2)) / (Main.maxTilesY * 16.0) * 0.2f * Main.screenHeight) * 0.5f;
        stars.CommonOffset = new Vector2(0, bgTopY);
    }

    private void UpdateTextures()
    {
        if (Utility.IsAprilFools)
        {
            earth.SetLighting(null);
            earth.SetTextures(earthBodyFlat);
        }
        else
        {
            earth.SetLighting(sun);

            if (Main.drunkWorld)
                earth.SetTextures(earthBodyDrunk, earthAtmo);
            else
                earth.SetTextures(earthBody, earthAtmo);
        }

        if (Main.LocalPlayer.head == 12)
            sun.SetTextures(TextureAssets.Sun2);
        else
            sun.SetTextures(sunTexture);
    }

    private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
    {
        float brightness;

        float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

        if (Main.dayTime)
        {
            if (Main.time <= fadeOutTimeDawn)
                brightness = maxBrightnessDay + (1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor;
            else if (Main.time >= fadeInTimeDusk)
                brightness = maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor;
            else
                brightness = maxBrightnessDay;
        }
        else
        {
            brightness = maxBrightnessNigt;
        }

        return brightness;
    }

    private void ConfigureEarthBodyShader(CelestialBody celestialBody, CelestialBody lightSource, out Vector3 lightPosition, out float radius, out int pixelSize)
    {
        float distanceFactor;
        float depth;
        if (Main.dayTime)
        {
            distanceFactor = MathHelper.Clamp(Vector2.Distance(celestialBody.Center, lightSource.Center) / Math.Max(celestialBody.Width * 2, celestialBody.Height * 2), 0, 1);
            depth = MathHelper.Lerp(-60, 400, Utility.QuadraticEaseIn(distanceFactor));
            lightPosition = new Vector3(Utility.ClampOutsideCircle(lightSource.Center, celestialBody.Center, earth.Width / 2 * 2), depth);
            radius = 0.1f;
        }
        else
        {
            distanceFactor = MathHelper.Clamp(Vector2.Distance(celestialBody.Center, lightSource.Center) / Math.Max(celestialBody.Width * 2, celestialBody.Height * 2), 0, 1);
            depth = MathHelper.Lerp(400, 5000, 1f - Utility.QuadraticEaseOut(distanceFactor));
            lightPosition = new Vector3(Utility.ClampOutsideCircle(lightSource.Center, celestialBody.Center, earth.Width / 2 * 2), depth);
            radius = 0.01f;
        }

        pixelSize = 1;
    }

    private void ConfigureEarthAtmoShader(CelestialBody earth, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution)
    {
        Vector2 screenSize = Main.ScreenSize.ToVector2();
        float distance = Vector2.Distance(earth.Center / screenSize, earth.LightSource.Center / screenSize);
        float offsetRadius = MathHelper.Lerp(0.12f, 0.56f, 1 - distance);

        if (!Main.dayTime)
        {
            offsetRadius = MathHelper.Lerp(0.56f, 0.01f, 1 - distance);
        }
        else
        {
            if (distance < 0.1f)
            {
                float proximityFactor = 1 - distance / 0.1f;
                offsetRadius += 0.8f * proximityFactor;
            }
        }

        offset = Utility.PolarVector(offsetRadius, rotation) * 0.65f;
        intensity = 0.96f;
        shadeResolution /= 2;
        radius = 1f;
    }

    private void RotateSun()
    {
        float bgTopY = (float)(((Main.screenPosition.Y - Main.screenHeight / 2)) / (Main.maxTilesY * 16.0) * 0.2f * Main.screenHeight) * 0.5f;
        double duration = Main.dayTime ? MacrocosmSubworld.GetDayLength() : MacrocosmSubworld.GetNightLength();


        double progress = Main.dayTime ? Main.time / duration : 1.0 - Main.time / duration;
        int timeX = (int)(progress * (Main.screenWidth + sun.Width * 2)) - (int)sun.Width;

        double timeY = Main.time < duration / 2
            ? Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) // AM
            : Math.Pow(1.0 - Main.time / duration * 2.0, 2.0);   // PM

        sun.Rotation = (float)(Main.time / duration) * 2f - 7.3f;
        sun.Scale = (float)(1.2 - timeY * 0.4);

        sun.Color = Color.White;

        int posY = Main.dayTime ? (int)(bgTopY + timeY * 250.0 + 240.0) : (int)(bgTopY - timeY * 250.0 + 665.0);

        sun.SetupSkyRotation(SkyRotationMode.None);
        sun.Center = new Vector2(timeX, posY);
        sun.Draw(Main.spriteBatch);
    }
}