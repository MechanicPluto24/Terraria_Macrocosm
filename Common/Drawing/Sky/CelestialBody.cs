using Macrocosm.Common.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.UI.Chat;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing.Sky
{
    public enum SkyRotationMode
    {
        None,
        Day,
        Night,
        Any
    }

    public class CelestialBody
    {
        public float Width => bodyTexture.Width;
        public float Height => bodyTexture.Height;

        public bool HasAtmo => atmoTexture is not null;
        public bool HasOverlay = false;

        /// <summary> Position on the screen </summary>
        public Vector2 Position { get; set; }
        public float Scale { get => scale; set => scale = value; }
        public float Rotation { get => rotation; set => rotation = value; }
		public CelestialBody LightSource { get => lightSource; set => lightSource = value; }
		public float OverlayRotation { get => overlayRotation; set => overlayRotation = value; }
        public float OrbitRotation { get => orbitRotation % MathHelper.TwoPi - MathHelper.Pi; set => orbitRotation = value; }
        public float OrbitSpeed { get => orbitSpeed; set => orbitSpeed = value; }

        #region Private vars 

        private Texture2D bodyTexture;
        private Texture2D atmoTexture;

        private Vector2 averageOffset = default;
        private float parallaxSpeedX = 0f;
        private float parallaxSpeedY = 0f;
        private float scale;
        private float rotation;
        private Color color = Color.White;

        private SkyRotationMode rotationMode = SkyRotationMode.None;

        private Texture2D bodyOverlayTexture = null;
        private Texture2D atmoOverlayTexture = null;
        private CelestialBody lightSource = null;
        private float overlayRotation = 0f;
        private Color shadowColor = Color.White;

        private CelestialBody orbitParent = null;
        private List<CelestialBody> orbitChildren = new();

        private Vector2 orbitEllipse = default;
        private float orbitAngle = 0f;
        private float orbitRotation = 0f;
        private float orbitSpeed = 0f;

        private Vector2 ScreenCenter => new(Main.screenWidth / 2, Main.screenHeight / 2);

		#endregion

		public CelestialBody(Texture2D bodyTexture = null, Texture2D atmoTexture = null, float scale = 1f, float rotation = 0f)
        {
            this.bodyTexture = bodyTexture;
            Scale = scale;
            this.atmoTexture = atmoTexture;
            Rotation = rotation;
        }

        public void SetPosition(float x, float y) => Position = new Vector2(x, y);
        public void SetPositionRelative(CelestialBody reference, Vector2 offset) => Position = reference.Position + offset;
        public void SetPositionPolar(Vector2 origin, float radius, float theta) => Position = origin + MathUtils.PolarVector(radius, theta);
        public void SetPositionPolar(float radius, float theta) => Position = ScreenCenter + MathUtils.PolarVector(radius, theta);
        public void SetPositionPolar(CelestialBody referenceBody, float radius, float theta) => Position = referenceBody.Position + MathUtils.PolarVector(radius, theta);
        
		public void SetLightSouce(CelestialBody lightSource) => this.lightSource = lightSource;

		public void SetTextures(Texture2D bodyTexture = null, Texture2D atmoTexture = null, Texture2D bodyShadowTexture = null, Texture2D atmoShadowTexture = null)
        {
            this.bodyTexture = bodyTexture;
            this.atmoTexture = atmoTexture;
            this.bodyOverlayTexture = bodyShadowTexture;
            this.atmoOverlayTexture = atmoShadowTexture;
        }

        /// <summary>
        /// Configure parallax settings for the object while drawn in a world's sky 
        /// </summary>
        /// <param name="parallaxX"> Horizontal parallaxing speed  </param>
        /// <param name="parallaxY"> Vertical parallaxing speed </param>
        /// <param name="averageOffset"> The offset from the screen center when the player is in the middle of the world </param>
        public void SetParallax(float parallaxX = 0f, float parallaxY = 0f, Vector2 averageOffset = default)
        {
            parallaxSpeedX = parallaxX;
            parallaxSpeedY = parallaxY;
            this.averageOffset = averageOffset;
        }

        /// <summary>
        /// Configures rotation of the object on the sky 
        /// </summary>
        /// <param name="mode">
        /// None  - No rotation (default)
        /// Day   - Only visible during the day (rotation logic will still run)
        /// Night - Only visible during the night (rotation logic will still run)
        /// Any   - Cycle during both day and night  
        /// </param>
        public void SetupSkyRotation(SkyRotationMode mode) => rotationMode = mode;

		/// <summary>
		/// Configures drawing extra textures over the object
		/// </summary>
		/// <param name="lightSource"> The object the overlay will rotate away from (leave null for stationary) </param>
		/// <param name="bodyOverlayTexture"> The texture that draws over the object's body (null for none) </param>
		/// <param name="atmoOverlayTexture"> The texture that draws over the object's atmosphere (null for none) </param>
		public void SetupOverlays(Texture2D bodyOverlayTexture = null, Texture2D atmoOverlayTexture = null)
        {
            HasOverlay = true;
            this.bodyOverlayTexture = bodyOverlayTexture;
            this.atmoOverlayTexture = atmoOverlayTexture;
        }

        /// <summary>
        /// Makes this body orbit around another CelestialBody in a circular orbit
        /// </summary>
        /// <param name="orbitParent"> The parent CelestialBody this should orbit around </param>
        /// <param name="orbitRadius"> The radius of the orbit relative to the parent's center </param>
        /// <param name="orbitRotation"> Initial angle of that orbit (in radians) </param>
        /// <param name="orbitSpeed"> The speed at which this object is orbiting (in rad/tick) </param>
        public void SetOrbitParent(CelestialBody orbitParent, float orbitRadius, float orbitRotation, float orbitSpeed)
        {
            this.orbitParent = orbitParent;
            orbitEllipse = new Vector2(orbitRadius);
            orbitAngle = 0f;
            this.orbitRotation = orbitRotation;
            this.orbitSpeed = orbitSpeed;
            orbitParent.orbitChildren.Add(this);
        }

        /// <summary>
        /// Makes this body orbit around another CelestialBody in an elliptic orbit (WIP)
        /// </summary>
        /// <param name="orbitParent"> The parent CelestialBody this should orbit around </param>
        /// <param name="orbitEllipse"> The ellipse min and max at 0 angle </param>
        /// <param name="orbitAngle"> The tilt of the orbit (TODO) </param>
        /// <param name="orbitRotation"> Initial angle of that orbit (in radians </param>
        /// <param name="orbitSpeed">  The speed at which this object is orbiting (in rad/tick) (FIXME) </param>
        public void SetOrbitParent(CelestialBody orbitParent, Vector2 orbitEllipse, float orbitAngle, float orbitRotation, float orbitSpeed)
        {
            this.orbitParent = orbitParent;
            this.orbitEllipse = orbitEllipse;
            this.orbitAngle = orbitAngle;
            this.orbitRotation = orbitRotation;
            this.orbitSpeed = orbitSpeed;
            orbitParent.orbitChildren.Add(this);
        }

        /// <summary>
        /// Registers an orbiting child for this CelestialBody 
        /// </summary>
        /// <param name="orbitChild"> The child CelestialBody to add </param>
        /// <param name="orbitRadius"> The radius of the orbit relative to the parent's center </param>
        /// <param name="orbitRotation"> Initial angle of that orbit (in radians) </param>
        /// <param name="orbitSpeed"> The speed at which this object is orbiting (in rad/tick) </param>
        public void AddOrbitChild(CelestialBody orbitChild, float orbitRadius, float orbitRotation, float orbitSpeed)
            => orbitChild.SetOrbitParent(this, orbitRadius, orbitRotation, orbitSpeed);

        /// <summary>
        /// Registers an orbiting child for this CelestialBody 
        /// </summary>
        /// <param name="orbitChild"> The child CelestialBody to add </param>
        /// <param name="orbitEllipse"> The ellipse min and max at 0 angle </param>
        /// <param name="orbitAngle"> The tilt of the orbit (TODO) </param>
        /// <param name="orbitRotation"> Initial angle of that orbit (in radians </param>
        /// <param name="orbitSpeed">  The speed at which this object is orbiting (in rad/tick) (FIXME) </param>
        public void AddOrbitChild(CelestialBody orbitChild, Vector2 orbitEllipse, float orbitAngle, float orbitRotation, float orbitSpeed)
            => orbitChild.SetOrbitParent(this, orbitEllipse, orbitAngle, orbitRotation, orbitSpeed);

        /// <summary>
        /// Resets the orbit tree down from this CelestialBody 
        /// </summary>
        public void ClearOrbitChildren()
        {
            foreach (CelestialBody child in orbitChildren)
                child.ClearOrbitChildren();

            orbitChildren.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (parallaxSpeedX > 0f || parallaxSpeedY > 0f || averageOffset != default)
                Parallax(); // stationary parallaxing mode 
            else if (rotationMode != SkyRotationMode.None)
                Rotate(); // rotate even if not drawing, it affects the shadow rotation  
            else if (orbitParent is not null)
                Orbit();

            if (!ShouldDraw())
                return;

            SpriteBatchState state = spriteBatch.SaveState();
            spriteBatch.EndIfBeginCalled();

            Effect shader = null;
			if (lightSource is not null)
            {
				shader = ModContent.Request<Effect>("Macrocosm/Assets/Effects/CelestialBodyShading", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                shader.Parameters["uPos1"].SetValue(Position);
                shader.Parameters["uPos2"].SetValue(lightSource.Position);
                shader.Parameters["uIntensity"].SetValue(0.85f);
                shader.Parameters["uOffset"].SetValue(0.23f);
 			}

			DrawChildren(spriteBatch, state, inFront: false);

            // Draw atmosphere 
            if (atmoTexture is not null)
			{
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, shader, state);
                spriteBatch.Draw(atmoTexture, Position, null, Color.White, Rotation, atmoTexture.Size() / 2, Scale, default, 0f);
                spriteBatch.End();
            }

            // Draw main body 
            if (bodyTexture is not null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, shader, state);
                spriteBatch.Draw(bodyTexture, Position, null, Color.White, Rotation, bodyTexture.Size() / 2, Scale, default, 0f);
                spriteBatch.End();
            }

            DrawChildren(spriteBatch, state, inFront: true);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, shader, state);

            #region Draw overlay
            if (HasOverlay)
            {
                if (atmoOverlayTexture is not null && HasAtmo)
                    spriteBatch.Draw(atmoOverlayTexture, Position, null, Color.White, overlayRotation, atmoTexture.Size() / 2, Scale, default, 0f);
            
                if (bodyOverlayTexture is not null)
                    spriteBatch.Draw(bodyOverlayTexture, Position, null, Color.White, overlayRotation, atmoTexture.Size() / 2, Scale, default, 0f);
            }
			#endregion

			spriteBatch.End();

            spriteBatch.Restore(state);
        }

        private void DrawChildren(SpriteBatch spriteBatch, SpriteBatchState state, bool inFront)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state);
            foreach (CelestialBody child in orbitChildren)
            {
                if (child.OrbitRotation >= orbitAngle && !inFront || child.OrbitRotation < orbitAngle && inFront)
                    child.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        private void Parallax()
        {
            // surface layer dimensions in game coordinates 
            float worldWidth = Main.maxTilesX * 16f;
            float surfaceLayerHeight = (float)Main.worldSurface * 16f;

            // positions relative to the center origin of the surface layer 
            float playerPositionToCenterX = Main.LocalPlayer.position.X - worldWidth / 2;
            float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - surfaceLayerHeight / 2;

            Position = new Vector2(
                ScreenCenter.X - playerPositionToCenterX * parallaxSpeedX + averageOffset.X,
                ScreenCenter.Y - playerPositionToSurfaceCenterY * parallaxSpeedY + averageOffset.Y
            );
        }

        private void Rotate()
        {
            double duration;

            if (SubworldSystem.AnyActive<Macrocosm>())
                duration = Main.dayTime ? MacrocosmSubworld.Current().DayLenght : MacrocosmSubworld.Current().NightLenght;
            else
                duration = Main.dayTime ? Main.dayLength : Main.nightLength;

            double bgTop = -(Main.LocalPlayer.Center.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 200.0;

            int timeX = (int)(Main.time / duration * (Main.screenWidth + bodyTexture.Width * 2)) - bodyTexture.Width;
            double timeY = Main.time < duration / 2 ? //Gets the Y axis for the angle depending on the time
                Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) : //AM
                Math.Pow(1.0 - Main.time / duration * 2.0, 2.0); //PM

            Rotation = (float)(Main.time / duration) * 2f - 7.3f;
            Scale = (float)(1.2 - timeY * 0.4);

            float clouldAlphaMult = Math.Max(0f, 1f - Main.cloudAlpha * 1.5f);
            color = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));
            int angle = (int)(bgTop + timeY * 250.0 + 180.0);

            Position = new Vector2(timeX, angle + Main.sunModY); // TODO: add configurable vertical parallax 
        }

        private void Orbit()
        {
            float radius;

            if (orbitEllipse.X == orbitEllipse.Y)
            {
                radius = orbitEllipse.X;
                orbitRotation += orbitSpeed;
            }
            else
            {
                radius = (float)(orbitEllipse.X * orbitEllipse.Y / Math.Sqrt(orbitEllipse.X * orbitEllipse.X * Math.Pow(Math.Sin(OrbitRotation + orbitAngle), 2) + orbitEllipse.Y * orbitEllipse.Y * Math.Pow(Math.Cos(OrbitRotation + orbitAngle), 2)));

                float perigee = Math.Min(orbitEllipse.X, orbitEllipse.Y);
                float apogee = Math.Max(orbitEllipse.X, orbitEllipse.Y);

                float period = MathHelper.TwoPi / orbitSpeed; // period of a circle, not ellipse 
                float angularSpeed = MathHelper.Pi / (period * radius * radius) * (perigee + apogee) * (float)Math.Sqrt(perigee * apogee);
                orbitRotation += angularSpeed;
            }

            SetPositionPolar(orbitParent, radius, orbitRotation);
        }

        private bool ShouldDraw()
        {
            if (rotationMode == SkyRotationMode.Day)
                return Main.dayTime;
            else if (rotationMode == SkyRotationMode.Night)
                return !Main.dayTime;
            else
                return true;
        }

        public static float ScaleBrigthness(bool dayTime, double timeHigh, double timeLow, float minBrightness, float maxBrightness)
        {
            float fadeFactor = maxBrightness - minBrightness;

            if (dayTime == Main.dayTime)
            {
                if (Main.time <= timeLow)
                    return minBrightness + (1f - (float)(Main.time / timeLow)) * fadeFactor;
                else if (Main.time >= timeHigh)
                    return minBrightness + (float)((Main.time - timeHigh) / timeLow) * fadeFactor;
                else
                    return minBrightness;
            }
            else
                return maxBrightness;
        }

        // TODO: - Add variation based on current subworld's day/night lenghts 
        //		 - Remove magic numbers lol 
        /// <summary> Used for linear brightness scaling along an entire day/night cycle  </summary>
        public static float ScaleBrightnessNoonToMidnight(float minBrightness, float maxBrightness)
        {
            float brightness;
            double totalTime = Main.dayTime ? Main.time : Main.dayLength + Main.time;

            float diff = maxBrightness - minBrightness;

            if (totalTime <= 27000)
                brightness = minBrightness + maxBrightness * (diff * 0.4f + diff * 0.6f * ((float)totalTime / 27000));
            else if (totalTime >= 70200)
                brightness = diff * 0.4f * ((float)(totalTime - 70200) / 16200);
            else
                brightness = maxBrightness - (float)(totalTime - 27000) / 43200;

            return brightness;
        }
    }
}