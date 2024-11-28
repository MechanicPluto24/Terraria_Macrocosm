using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing.Sky
{
    public class CelestialBody
    {
        public enum SkyRotationMode
        {
            None,
            Day,
            Night,
            Any
        }

        /// <summary> The CelestialBody's width </summary>
        public float Width => Size.X;

        /// <summary> The CelestialBody's height </summary>
        public float Height => Size.Y;

        /// <summary> The CelestialBody's size </summary>
        public Vector2 Size { get; private set; }

        /// <summary> Center position on the screen </summary>
        public Vector2 Center { get; set; }

        public Vector2 Position => Center - Size / 2f;

        /// <summary> The CelestialBody's hitbox, defined by size and center position </summary>
        public Rectangle Hitbox => new((int)(Center.X - Width / 2f), (int)(Center.Y - Height / 2f), (int)(Width), (int)(Height));

        /// <summary> The CelestialBody's scale </summary>
        public float Scale
        {
            get => scale;
            set
            {
                scale = value;
                Size = defSize * scale;
            }
        }

        /// <summary> The CelestialBody's rotation </summary>
        public float Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        /// <summary> The draw color  </summary>
        public Color Color = Color.White;

        /// <summary> References another CelestialBody as a source of light </summary>
        public CelestialBody LightSource { get => lightSource; set => lightSource = value; }

        /// <summary> The rotation of the CelestialBody's orbit around another CelestialBody </summary>
        public float OrbitRotation
        {
            get => orbitRotation % MathHelper.TwoPi;
            set => orbitRotation = value;
        }

        /// <summary> The CelestialBody's orbit angular speed around another CelestialBody </summary>
        public float OrbitSpeed => orbitSpeed;

        public float OrbitAngle => orbitAngle;

        /// <summary> Whether the CelestialBody should update its position or not </summary>
        public bool ShouldUpdate { get; set; } = true;

        /// <summary> 
        /// Whether to reset the spritebatch or not. Set to false for batched drawing.
        /// If false, you MUST manually begin and end the spritebatch when drawing!
        /// </summary>
        public bool ResetSpritebatch { get; set; } = true;

        public delegate void FuncConfigureSphericalShader(CelestialBody celestialBody, CelestialBody lightSource, out Vector3 lightPosition, out float radius, out int pixelSize);
        public FuncConfigureSphericalShader ConfigureBackSphericalShader;
        public FuncConfigureSphericalShader ConfigureBodySphericalShader;
        public FuncConfigureSphericalShader ConfigureFrontSphericalShader;

        public delegate void FuncConfigureRadialShader(CelestialBody celestialBody, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution);
        public FuncConfigureRadialShader ConfigureBackRadialShader = null;
        public FuncConfigureRadialShader ConfigureBodyRadialShader = null;
        public FuncConfigureRadialShader ConfigureFrontRadialShader = null;

        public delegate Effect FuncOverrideShader();
        public FuncOverrideShader OverrideBackShader = null;
        public FuncOverrideShader OverrideBodyShader = null;
        public FuncOverrideShader OverrideFrontShader = null;

        public delegate void FuncOverrideDraw(CelestialBody celestialBody, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> texture, Effect shader);
        /// <summary> Override the way this CelestialBody's pre-body texture is drawn. You must begin and end the SpriteBatch </summary>
        public FuncOverrideDraw OverrideBackDraw = null;
        /// <summary> Override the way this CelestialBody is drawn. You must begin and end the SpriteBatch </summary>
        public FuncOverrideDraw OverrideBodyDraw = null;
        /// <summary> Override the way this CelestialBody's pre-body texture is drawn. You must begin and end the SpriteBatch </summary>
        public FuncOverrideDraw OverrideFrontDraw = null;

        #region Private vars 

        private static Asset<Effect> sphereLightingShader;
        private static Asset<Effect> radialLightingShader;

        private Asset<Texture2D> backTexture;
        private Asset<Texture2D> bodyTexture;
        private Asset<Texture2D> frontTexture;

        private Rectangle? backSourceRect;
        private Rectangle? bodySourceRect;
        private Rectangle? frontSourceRect;

        private Vector2 averageOffset = default;
        private float parallaxSpeedX = 0f;
        private float parallaxSpeedY = 0f;
        private Vector2 defSize;
        private float scale;
        private float rotation;

        private SkyRotationMode rotationMode = SkyRotationMode.None;

        private CelestialBody lightSource = null;

        private CelestialBody orbitParent = null;
        private readonly List<CelestialBody> orbitChildren = new();

        private Vector2 orbitEllipse = default;
        private float orbitAngle = 0f;
        private float orbitRotation = 0f;
        private float orbitSpeed = 0f;

        private SpriteBatchState state;

        #endregion

        public CelestialBody
        (
            Asset<Texture2D> bodyTexture = null,
            Asset<Texture2D> backTexture = null,
            Asset<Texture2D> frontTexture = null,
            float scale = 1f,
            float rotation = 0f,
            Vector2? size = null,
            Rectangle? backSourceRect = null,
            Rectangle? bodySourceRect = null,
            Rectangle? frontSourceRect = null
        )
        {
            this.bodyTexture = bodyTexture;
            this.backTexture = backTexture;
            this.frontTexture = frontTexture;

            this.backSourceRect = backSourceRect;
            this.bodySourceRect = bodySourceRect;
            this.frontSourceRect = frontSourceRect;

            if (size.HasValue)
            {
                defSize = size.Value;
            }
            else
            {
                defSize = new
                (
                    bodyTexture is null ? 1 : (bodySourceRect.HasValue ? bodySourceRect.Value.Width : bodyTexture.Width()),
                    bodyTexture is null ? 1 : (bodySourceRect.HasValue ? bodySourceRect.Value.Height : bodyTexture.Height())
                );
            }

            Scale = scale;
            Rotation = rotation;

            radialLightingShader = ModContent.Request<Effect>(Macrocosm.ShadersPath + "RadialLighting", AssetRequestMode.ImmediateLoad);
            sphereLightingShader = ModContent.Request<Effect>(Macrocosm.ShadersPath + "SphereLighting", AssetRequestMode.ImmediateLoad);
        }

        /// <summary> Set the position using absolut coordinates </summary>
        public void SetPosition(float x, float y) => Center = new Vector2(x, y);

        /// <summary> Set the position relative to another CelestialBody </summary>
        public void SetPositionRelative(CelestialBody reference, Vector2 offset) => Center = reference.Center + offset;

        /// <summary> Set the position using a reference point and polar coordinates </summary>
        public void SetPositionPolar(Vector2 origin, float radius, float theta) => Center = origin + Utility.PolarVector(radius, theta);

        /// <summary> Set the position using polar coordinates, with the screen center as origin </summary>
        public void SetPositionPolar(float radius, float theta) => Center = Utility.ScreenCenter + Utility.PolarVector(radius, theta);

        /// <summary> Set the position using polar coordinates, with the position of another CelestialBody as origin </summary>
        public void SetPositionPolar(CelestialBody referenceBody, float radius, float theta) => Center = referenceBody.Center + Utility.PolarVector(radius, theta);

        /// <summary> Set the lightsource CelestialBody of this CelestialBody </summary>
        public void SetLighting(CelestialBody lightSource)
        {
            this.lightSource = lightSource;
        }

        /// <summary> Set the composing textures of the CelestialBody </summary>
        public void SetTextures(Asset<Texture2D> bodyTexture = null, Asset<Texture2D> backTexture = null, Asset<Texture2D> frontTexture = null)
        {
            this.bodyTexture = bodyTexture;
            this.backTexture = backTexture;
            this.frontTexture = frontTexture;
        }

        public void SetCommonSourceRectangle(Rectangle? commonSourceRect = null) => SetSourceRectangles(commonSourceRect, commonSourceRect, commonSourceRect);

        public void SetSourceRectangles(Rectangle? backSourceRect = null, Rectangle? bodySourceRect = null, Rectangle? frontSourceRect = null)
        {
            this.backSourceRect = backSourceRect;
            this.bodySourceRect = bodySourceRect;
            this.frontSourceRect = frontSourceRect;
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

            if (!orbitParent.orbitChildren.Contains(this))
                orbitParent.orbitChildren.Add(this);
        }

        /// <summary>
        /// Makes this body orbit around another CelestialBody in an elliptic orbit (WIP)
        /// </summary>
        /// <param name="orbitParent"> The parent CelestialBody this should orbit around </param>
        /// <param name="orbitEllipse"> The ellipse min and max at 0 angle </param>
        /// <param name="orbitAngle"> The tilt of the orbit </param>
        /// <param name="orbitRotation"> Initial angle of that orbit (in radians </param>
        /// <param name="orbitSpeed">  The speed at which this object is orbiting (in rad/tick) (FIXME) </param>
        public void SetOrbitParent(CelestialBody orbitParent, Vector2 orbitEllipse, float orbitAngle, float orbitRotation, float orbitSpeed)
        {
            this.orbitParent = orbitParent;
            this.orbitEllipse = orbitEllipse;
            this.orbitAngle = orbitAngle;
            this.orbitRotation = orbitRotation;
            this.orbitSpeed = orbitSpeed;

            if (!orbitParent.orbitChildren.Contains(this))
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

        private void Update()
        {
            if (Main.gamePaused || !ShouldUpdate)
                return;

            if (parallaxSpeedX > 0f || parallaxSpeedY > 0f || averageOffset != default)
                Parallax(); // stationary parallaxing mode 
            else if (rotationMode != SkyRotationMode.None)
                Rotate(); // rotate even if not drawing, it affects the shadow rotation  
            else if (orbitParent is not null)
                Orbit();

        }

        /// <summary>
        /// Draw this CelestialBody
        /// </summary>
        /// <param name="spriteBatch"> The SpriteBatch </param>
        /// <param name="withChildren"> Whether to draw the CelestialBody with all its orbiting children </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Update();

            if (!ShouldDraw())
                return;

            bool beginCalled = true;
            if (ResetSpritebatch)
            {
                state.SaveState(spriteBatch);
                beginCalled = spriteBatch.BeginCalled();
                spriteBatch.EndIfBeginCalled();
            }

            DrawBack(spriteBatch);
            DrawBody(spriteBatch);
            DrawFront(spriteBatch);

            if (ResetSpritebatch && beginCalled)
                spriteBatch.Begin(state);
        }

        public void DrawChildren(SpriteBatch spriteBatch, Func<CelestialBody, bool> shouldDrawChild, bool recursive = false)
        {
            foreach (CelestialBody child in orbitChildren)
            {
                if (shouldDrawChild(child))
                {
                    child.Draw(spriteBatch);

                    if (recursive)
                        child.DrawChildren(spriteBatch, shouldDrawChild, recursive);
                }
            }
        }

        private void DrawBack(SpriteBatch spriteBatch)
        {
            Effect backShader = null;
            if (OverrideBackShader is not null)
            {
                backShader = OverrideBackShader();
            }
            else if (lightSource is not null)
            {
                if (ConfigureBackRadialShader is not null)
                {
                    backShader = radialLightingShader.Value;
                    float rotation = (Center - lightSource.Center).ToRotation();
                    Vector2 shadeResolution = backTexture.Size();
                    Vector4 sourceRect = backSourceRect.HasValue ? backSourceRect.Value.Normalize(backTexture.Size()) : new Vector4(0, 0, 1, 1);
                    ConfigureBackRadialShader(this, rotation, out float intensity, out Vector2 offset, out float radius, ref shadeResolution);
                    backShader.Parameters["uOffset"].SetValue(offset);
                    backShader.Parameters["uIntensity"].SetValue(intensity);
                    backShader.Parameters["uRadius"].SetValue(radius);
                    backShader.Parameters["uShadeResolution"].SetValue(shadeResolution);
                    backShader.Parameters["uSourceRect"].SetValue(sourceRect);
                    backShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }
                else if (ConfigureBackSphericalShader is not null)
                {
                    backShader = sphereLightingShader.Value;
                    ConfigureBackSphericalShader(this, lightSource, out Vector3 lightPosition, out float radius, out int pixelSize);
                    backShader.Parameters["uLightSource"].SetValue(lightPosition);
                    backShader.Parameters["uEntityPosition"].SetValue(Position);
                    backShader.Parameters["uTextureSize"].SetValue(backTexture.Size());
                    backShader.Parameters["uEntitySize"].SetValue(backTexture.Size());
                    backShader.Parameters["uRadius"].SetValue(radius);
                    backShader.Parameters["uPixelSize"].SetValue(pixelSize);
                    backShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }

            }

            if (backTexture is not null)
            {
                if (OverrideBackDraw is null)
                {
                    if (ResetSpritebatch) spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, default, state.RasterizerState, backShader, state.Matrix);
                    spriteBatch.Draw(backTexture.Value, Center, backSourceRect, Color, Rotation, backTexture.Size() / 2, Scale, default, 0f);
                    if (ResetSpritebatch) spriteBatch.End();
                }
                else OverrideBackDraw(this, spriteBatch, state, backTexture, backShader);
            }
        }

        private void DrawBody(SpriteBatch spriteBatch)
        {
            Effect bodyShader = null;
            if (OverrideBodyShader is not null)
            {
                bodyShader = OverrideBodyShader();
            }
            else if (lightSource is not null)
            {
                if (ConfigureBodyRadialShader is not null)
                {
                    bodyShader = radialLightingShader.Value;
                    float rotation = (Center - lightSource.Center).ToRotation();
                    Vector2 shadeResolution = bodyTexture.Size();
                    Vector4 sourceRect = bodySourceRect.HasValue ? bodySourceRect.Value.Normalize(bodyTexture.Size()) : new Vector4(0, 0, 1, 1);
                    ConfigureBodyRadialShader(this, rotation, out float intensity, out Vector2 offset, out float radius, ref shadeResolution);
                    bodyShader.Parameters["uOffset"].SetValue(offset);
                    bodyShader.Parameters["uIntensity"].SetValue(intensity);
                    bodyShader.Parameters["uRadius"].SetValue(radius);
                    bodyShader.Parameters["uShadeResolution"].SetValue(shadeResolution);
                    bodyShader.Parameters["uSourceRect"].SetValue(sourceRect);
                    bodyShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }
                else if (ConfigureBodySphericalShader is not null)
                {
                    bodyShader = sphereLightingShader.Value;
                    ConfigureBodySphericalShader(this, lightSource, out Vector3 lightPosition, out float radius, out int pixelSize);
                    bodyShader.Parameters["uLightSource"].SetValue(lightPosition);
                    bodyShader.Parameters["uEntityPosition"].SetValue(Position);
                    bodyShader.Parameters["uTextureSize"].SetValue(bodyTexture.Size());
                    bodyShader.Parameters["uEntitySize"].SetValue(bodyTexture.Size());
                    bodyShader.Parameters["uRadius"].SetValue(radius);
                    bodyShader.Parameters["uPixelSize"].SetValue(pixelSize);
                    bodyShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }
            }

            if (bodyTexture is not null)
            {
                if (OverrideBodyDraw is null)
                {
                    if (ResetSpritebatch) spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, state.RasterizerState, bodyShader, state.Matrix);
                    spriteBatch.Draw(bodyTexture.Value, Center, bodySourceRect, Color, Rotation, bodyTexture.Size() / 2, Scale, default, 0f);
                    if (ResetSpritebatch) spriteBatch.End();
                }
                else OverrideBodyDraw(this, spriteBatch, state, bodyTexture, bodyShader);
            }
        }

        private void DrawFront(SpriteBatch spriteBatch)
        {
            Effect frontShader = null;
            if (OverrideFrontShader is not null)
            {
                frontShader = OverrideFrontShader();
            }
            else if (lightSource is not null)
            {
                if (ConfigureFrontRadialShader is not null)
                {
                    frontShader = radialLightingShader.Value;
                    float rotation = (Center - lightSource.Center).ToRotation();
                    Vector2 shadeResolution = frontTexture.Size();
                    Vector4 sourceRect = frontSourceRect.HasValue ? frontSourceRect.Value.Normalize(frontTexture.Size()) : new Vector4(0, 0, 1, 1);
                    ConfigureFrontRadialShader(this, rotation, out float intensity, out Vector2 offset, out float radius, ref shadeResolution);
                    frontShader.Parameters["uOffset"].SetValue(offset);
                    frontShader.Parameters["uIntensity"].SetValue(intensity);
                    frontShader.Parameters["uRadius"].SetValue(radius);
                    frontShader.Parameters["uShadeResolution"].SetValue(shadeResolution);
                    frontShader.Parameters["uSourceRect"].SetValue(sourceRect);
                    frontShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }
                else if (ConfigureFrontSphericalShader is not null)
                {
                    frontShader = sphereLightingShader.Value;
                    ConfigureFrontSphericalShader(this, lightSource, out Vector3 lightPosition, out float radius, out int pixelSize);
                    frontShader.Parameters["uLightSource"].SetValue(lightPosition);
                    frontShader.Parameters["uEntityPosition"].SetValue(Position);
                    frontShader.Parameters["uTextureSize"].SetValue(frontTexture.Size());
                    frontShader.Parameters["uEntitySize"].SetValue(frontTexture.Size());
                    frontShader.Parameters["uRadius"].SetValue(radius);
                    frontShader.Parameters["uPixelSize"].SetValue(pixelSize);
                    frontShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }
            }

            if (frontTexture is not null)
            {
                if (OverrideFrontDraw is null)
                {
                    if (ResetSpritebatch) spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, default, state.RasterizerState, frontShader, state.Matrix);
                    spriteBatch.Draw(frontTexture.Value, Center, frontSourceRect, Color, Rotation, frontTexture.Size() / 2, Scale, default, 0f);
                    if (ResetSpritebatch) spriteBatch.End();
                }
                else OverrideFrontDraw(this, spriteBatch, state, frontTexture, frontShader);
            }
        }

        private void Parallax()
        {
            // surface layer dimensions in game coordinates 
            float worldWidth = Main.maxTilesX * 16f;
            float surfaceLayerHeight = (float)Main.worldSurface * 16f;

            // positions relative to the center origin of the surface layer 
            float screenCenterToWorldCenterX = Utility.ScreenCenterInWorld.X - worldWidth / 2;
            float screenPositionToWorldSurfaceCenterY = Utility.ScreenCenterInWorld.Y - surfaceLayerHeight / 2;

            Center = new Vector2(
                Utility.ScreenCenter.X - screenCenterToWorldCenterX * parallaxSpeedX + averageOffset.X,
                Utility.ScreenCenter.Y - screenPositionToWorldSurfaceCenterY * parallaxSpeedY + averageOffset.Y
            );
        }

        private void Rotate()
        {
            double duration;
            if (MacrocosmSubworld.Current is null)
                duration = Main.dayTime ? Main.dayLength : Main.nightLength;
            else
                duration = Main.dayTime ? MacrocosmSubworld.Current.DayLength : MacrocosmSubworld.Current.NightLength;

            double bgTopY = -(Main.screenPosition.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 200.0;

            double progress = Main.dayTime ? Main.time / duration : 1.0 - Main.time / duration;
            int timeX = (int)(progress * (Main.screenWidth + bodyTexture.Width() * 2)) - bodyTexture.Width();

            double timeY = Main.time < duration / 2 ? //Gets the Y axis for the angle depending on the time
                Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) : //AM
                Math.Pow(1.0 - Main.time / duration * 2.0, 2.0); //PM

            Rotation = (float)(Main.time / duration) * 2f - 7.3f;
            Scale = (float)(1.2 - timeY * 0.4);

            float clouldAlphaMult = Math.Max(0f, 1f - Main.cloudAlpha * 1.5f);
            Color = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));
            int posY = Main.dayTime ? (int)(bgTopY + timeY * 250.0 + 180.0) : (int)(bgTopY - timeY * 250.0 + 665.0);

            Center = new Vector2(timeX, posY); // TODO: add configurable vertical parallax 
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

                float minor = Math.Min(orbitEllipse.X, orbitEllipse.Y);
                float major = Math.Max(orbitEllipse.X, orbitEllipse.Y);

                float period = MathHelper.TwoPi / orbitSpeed; // period of a circle, not ellipse 
                float angularSpeed = MathHelper.Pi / (period * radius * radius) * (minor + major) * (float)Math.Sqrt(minor * major);
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
    }
}