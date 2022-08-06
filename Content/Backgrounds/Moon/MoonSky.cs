using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Stars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Moon
{
	public class MoonSky : CustomSky, ILoadable, ITexturedImmediate
	{
		public bool Active;
		public float Intensity;

		private StarsDrawing starsDay;
		private StarsDrawing starsNight;

		private CelestialObject earth;
		private CelestialObject sun;

		Texture2D skyTexture;

		const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
		const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten
		public bool TexLoaded { get ; set; } = false;

		public void Load(Mod mod)
		{
			if (Main.dedServ)
				return;

			MoonSky moonSky = new MoonSky();

			Filters.Scene["Macrocosm:MoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.High);
			SkyManager.Instance["Macrocosm:MoonSky"] = moonSky;
		}

		public void Unload() { }


		public void LoadTextures()
		{
			if (TexLoaded)
				return;

			skyTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/MoonSky", AssetRequestMode.ImmediateLoad).Value;

			Texture2D sunTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/Sun_0", AssetRequestMode.ImmediateLoad).Value;
			Texture2D earthBody = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmoless", AssetRequestMode.ImmediateLoad).Value;
			Texture2D earthAtmo = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmo", AssetRequestMode.ImmediateLoad).Value;
			Texture2D earthBodyShadow = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthShadowMask", AssetRequestMode.ImmediateLoad).Value;
			Texture2D earthAtmoShadow = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmoShadowMask", AssetRequestMode.ImmediateLoad).Value;

			starsDay = new StarsDrawing();
			starsNight = new StarsDrawing();

			sun = new CelestialObject(sunTexture);
			earth = new CelestialObject(earthBody, earthAtmo, 0.9f);

			sun.SetSkyRotationMode(CelestialObject.SkyRotationMode.Day);

			earth.SetParallax(0.01f, 0.12f, new Vector2(0f, -200f));
			earth.SetupShadow(sun, earthBodyShadow, earthAtmoShadow);

			TexLoaded = true;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			LoadTextures();

			starsDay.SpawnStars(100, 130, 1.4f, 0.05f);
			starsNight.SpawnStars(600, 700, 0.8f, 0.05f);
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

				starsDay.Draw();
				starsNight.Draw(nightStarBrightness);

				sun.DrawSelf(spriteBatch);
				earth.DrawSelf(spriteBatch);	
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

			Color nebulaColor = Color.White * brightness;
			nebulaColor.A = 0;

			Main.spriteBatch.Draw(nebula, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), nebulaColor);
		}

		private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
		{
			float brightness;

			float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

			if (Main.dayTime)
			{
				if (Main.time <= fadeOutTimeDawn)
				{
					brightness = (maxBrightnessDay + ((1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor));
				}
				else if (Main.time >= fadeInTimeDusk)
				{
					brightness = (maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor);
				}
				else
				{
					brightness = maxBrightnessDay;
				}
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