using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace Macrocosm.Content.Backgrounds.Moon
{
    public class MoonSky : CustomSky, ILoadable
	{
		public bool Active;
		public float Intensity;

		private StarsDrawing starsDay;
		private StarsDrawing starsNight;

		private CelestialBody earth;
		private CelestialBody sun;

		private Texture2D skyTexture;

		private Texture2D sunTexture;  

		private Texture2D earthBody;  
		private Texture2D earthBodyDrunk;  
		private Texture2D earthBodyFlat;  

		private Texture2D earthAtmo; 
		private Texture2D earthBodyShadow; 
		private Texture2D earthAtmoShadow;  

		const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
		const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten

		public MoonSky()
		{
			skyTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/MoonSky", AssetRequestMode.ImmediateLoad).Value;

			sunTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/Sun", AssetRequestMode.ImmediateLoad).Value;
			earthBody = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/Earth", AssetRequestMode.ImmediateLoad).Value;
			earthBodyDrunk = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthDrunk", AssetRequestMode.ImmediateLoad).Value;
			earthBodyFlat = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthFlat", AssetRequestMode.ImmediateLoad).Value;
			
			earthAtmo = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmo", AssetRequestMode.ImmediateLoad).Value;
			earthBodyShadow = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthShadowMask", AssetRequestMode.ImmediateLoad).Value;
			earthAtmoShadow = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmoShadowMask", AssetRequestMode.ImmediateLoad).Value;

			starsDay = new StarsDrawing();
			starsNight = new StarsDrawing();

			sun = new CelestialBody(sunTexture);
			earth = new CelestialBody(earthBody, earthAtmo, 0.9f);

			sun.SetupSkyRotation(SkyRotationMode.Day);

			earth.SetParallax(0.01f, 0.12f, new Vector2(0f, -200f));
			//earth.SetupOverlays(earthBodyShadow, earthAtmoShadow);
			earth.SetLightSouce(sun);
		}

		public void Load(Mod mod)
		{
			if (Main.dedServ)
				return;

			MoonSky moonSky = new();
			Filters.Scene["Macrocosm:MoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.High);
			SkyManager.Instance["Macrocosm:MoonSky"] = moonSky;
		}

		public void Unload() { }

		public override void Activate(Vector2 position, params object[] args)
		{
			starsDay.SpawnStars(100, 130, 1.4f, 0.05f);
			starsNight.SpawnStars(600, 700, 0.8f, 0.05f);

			MacrocosmStar mars = starsDay.RandStar(); // :) 
			mars.OverrideColor(Color.OrangeRed * 0.7f);
			mars.scale *= 1.4f;

			Intensity = 0.002f;
			Active = true;
		}

		public override void Deactivate(params object[] args)
		{
			starsDay.Clear();
			starsNight.Clear();
			Active = false;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
			{
				spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * Intensity);

				spriteBatch.Draw(skyTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
					Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * Intensity));

				float nebulaBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.17f, 0.45f);
				float nightStarBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.1f, 0.8f);

				DrawMoonNebula(nebulaBrightness);

				starsDay.Draw(spriteBatch);
				starsNight.Draw(spriteBatch, nightStarBrightness);
				
				sun.Draw(spriteBatch);
				earth.Draw(spriteBatch);
			}
		}

		private static void DrawMoonNebula(float brightness)
		{
			Texture2D nebula = Main.moonType switch
			{
				1 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaYellow").Value,
				2 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaRinged").Value,
				3 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaMythril").Value,
				4 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaBlue").Value,
				5 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaGreen").Value,
				6 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaPink").Value,
				7 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaOrange").Value,
				8 => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaPurple").Value,
				_ => ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/NebulaNormal").Value,
			};
			
			Color nebulaColor = (Color.White * brightness).NewAlpha(0f);			
			
			Main.spriteBatch.Draw(nebula, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), nebulaColor);
		}

		private void SetEarthTextures()
		{
			if (AprilFools.CheckAprilFools())
				earth.SetTextures(earthBodyFlat);
			else if (Main.drunkWorld)
				earth.SetTextures(earthBodyDrunk, earthAtmo, earthBodyShadow, earthAtmoShadow);
			else
				earth.SetTextures(earthBody, earthAtmo, earthBodyShadow, earthAtmoShadow);
		}

		private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
		{
			float brightness;

			float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

			if (Main.dayTime)
			{
				if (Main.time <= fadeOutTimeDawn)
 					brightness = (maxBrightnessDay + ((1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor));
 				else if (Main.time >= fadeInTimeDusk)
 					brightness = (maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor);
 				else
 					brightness = maxBrightnessDay;
 			}
			else
			{
 				brightness = maxBrightnessNigt;
			}
 
			return brightness;
		}

		public override void Update(GameTime gameTime)
		{
			Intensity = Active ? Math.Min(1f, 0.01f + Intensity) : Math.Max(0f, Intensity - 0.01f);
			SetEarthTextures();
		}

		public override Color OnTileColor(Color inColor)
		{
			Vector4 value = inColor.ToVector4();
			return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.5f));
		}


		public override float GetCloudAlpha()
		{
			return 1f - Intensity;
		}
		public override void Reset()
		{
			starsDay.Clear();
			starsNight.Clear();
			Active = false;
		}

		public override bool IsActive()
		{
			return Active || Intensity > 0.001f;
		}
	}
}