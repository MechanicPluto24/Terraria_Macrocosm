using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Macrocosm.Common.Drawing.Sky
{
    /// <summary>
    /// A base class for handling orbital logic of bodies as well as their children.
    /// </summary>
    public abstract class CelestialBody
    {
        public enum SkyRotationMode
        {
            /// <summary> No rotation (default) </summary>
            None,
            /// <summary> Only visible during the day (rotation logic will still run) </summary>
            Day,
            /// <summary> Only visible during the night (rotation logic will still run) </summary>
            Night,
            /// <summary> Cycle during both day and night </summary>
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

        public Vector2 Position => Center - Size * .5f;

        /// <summary> The CelestialBody's hitbox, defined by size and center position </summary>
        public Rectangle Hitbox => new((int)(Center.X - Width * .5f), (int)(Center.Y - Height * .5f), (int)Width, (int)Height);

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

        #region Private Fields

        private Vector2 averageOffset = default;
        private float parallaxSpeedX = 0f;
        private float parallaxSpeedY = 0f;
        protected Vector2 defSize;
        private float scale;
        private float rotation;

        protected SkyRotationMode rotationMode = SkyRotationMode.None;

        protected CelestialBody lightSource = null;

        protected CelestialBody orbitParent = null;
        protected readonly List<CelestialBody> orbitChildren = new();

        private Vector2 orbitEllipse = default;
        private float orbitAngle = 0f;
        private float orbitRotation = 0f;
        private float orbitSpeed = 0f;

        protected SpriteBatchState state;

        #endregion

        public CelestialBody
        (
            float scale = 1f,
            float rotation = 0f,
            Vector2 size = default
        )
        {
            defSize = size;
            Scale = scale;
            Rotation = rotation;
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
        public void SetLighting(CelestialBody lightSource) =>
            this.lightSource = lightSource;

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

            DrawSelf(spriteBatch);

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

        /// <summary>
        /// Allows for inheriting classes to provide their own drawcode.
        /// </summary>
        protected virtual void DrawSelf(SpriteBatch spriteBatch) { }

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
                duration = Main.dayTime ? MacrocosmSubworld.GetDayLength() : MacrocosmSubworld.GetNightLength();

            double bgTopY = -(Main.screenPosition.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 200.0;

            if (rotationMode == SkyRotationMode.Day)
            {
                double progress = Main.dayTime ? Main.time / duration : 1.0 - Main.time / duration;
                int timeX = (int)(progress * (Main.screenWidth + Size.X * 2)) - (int)Size.X;

                double timeY = Main.time < duration / 2
                    ? Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) // AM
                    : Math.Pow(1.0 - Main.time / duration * 2.0, 2.0);   // PM

                Rotation = (float)(Main.time / duration) * 2f - 7.3f;
                Scale = (float)(1.2 - timeY * 0.4);

                float cloudAlphaMult = Math.Max(0f, 1f - Main.cloudAlpha * 1.5f);
                Color = new Color((byte)(255f * cloudAlphaMult), (byte)(Color.White.G * cloudAlphaMult), (byte)(Color.White.B * cloudAlphaMult), (byte)(255f * cloudAlphaMult));
                int posY = Main.dayTime ? (int)(bgTopY + timeY * 250.0 + 180.0) : (int)(bgTopY - timeY * 250.0 + 665.0);

                Center = new Vector2(timeX, posY);
            }
            else if (rotationMode == SkyRotationMode.Night)
            {
                double progress = !Main.dayTime ? Main.time / duration : 1.0 - Main.time / duration;
                int timeX = (int)(progress * (Main.screenWidth + Size.X * 2)) - (int)Size.X;

                double timeY = Main.time < duration / 2
                    ? Math.Pow(1.0 - Main.time / duration * 2.0, 2.0)
                    : Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0);

                Rotation = (float)(Main.time / duration) * 2f - 7.3f;
                Scale = (float)(1.2 - timeY * 0.4);

                float cloudAlphaMult = Math.Max(0f, 1f - Main.cloudAlpha * 1.5f);
                Color = new Color((byte)(255f * cloudAlphaMult), (byte)(Color.White.G * cloudAlphaMult), (byte)(Color.White.B * cloudAlphaMult), (byte)(255f * cloudAlphaMult));
                int posY = !Main.dayTime ? (int)(bgTopY + timeY * 250.0 + 360.0) : (int)(bgTopY - timeY * 250.0 + 665.0);

                Center = new Vector2(timeX, posY);
            }
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