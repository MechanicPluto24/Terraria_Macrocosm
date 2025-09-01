using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace Macrocosm.Common.Drawing.Sky
{
        // FIXME: Rename this class to something clearer, I cannot come up with anything that feels right.
        // Alternatively somehow bake this into the base of CelestialBody ? not sure.
    public class CelestialBodySprite : CelestialBody
    {
        public delegate void FuncConfigureSphericalShader(CelestialBodySprite celestialBody, CelestialBody lightSource, out Vector3 lightPosition, out float radius, out int pixelSize);
        public FuncConfigureSphericalShader ConfigureBackSphericalShader;
        public FuncConfigureSphericalShader ConfigureBodySphericalShader;
        public FuncConfigureSphericalShader ConfigureFrontSphericalShader;

        public delegate void FuncConfigureRadialShader(CelestialBodySprite celestialBody, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution);
        public FuncConfigureRadialShader ConfigureBackRadialShader = null;
        public FuncConfigureRadialShader ConfigureBodyRadialShader = null;
        public FuncConfigureRadialShader ConfigureFrontRadialShader = null;

        public delegate Effect FuncOverrideShader();
        public FuncOverrideShader OverrideBackShader = null;
        public FuncOverrideShader OverrideBodyShader = null;
        public FuncOverrideShader OverrideFrontShader = null;

        public delegate void FuncOverrideDraw(CelestialBodySprite celestialBody, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> texture, Effect shader);
        /// <summary> Override the way this CelestialBodySprite's pre-body texture is drawn. You must begin and end the SpriteBatch </summary>
        public FuncOverrideDraw OverrideBackDraw = null;
        /// <summary> Override the way this CelestialBodySprite is drawn. You must begin and end the SpriteBatch </summary>
        public FuncOverrideDraw OverrideBodyDraw = null;
        /// <summary> Override the way this CelestialBodySprite's pre-body texture is drawn. You must begin and end the SpriteBatch </summary>
        public FuncOverrideDraw OverrideFrontDraw = null;

        #region Private fields

        private Asset<Texture2D> backTexture;
        private Asset<Texture2D> bodyTexture;
        private Asset<Texture2D> frontTexture;

        private Rectangle? backSourceRect;
        private Rectangle? bodySourceRect;
        private Rectangle? frontSourceRect;

        #endregion

        public CelestialBodySprite
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
            : base(scale, rotation, size ?? Vector2.One)
        {
            this.bodyTexture = bodyTexture;
            this.backTexture = backTexture;
            this.frontTexture = frontTexture;

            this.backSourceRect = backSourceRect;
            this.bodySourceRect = bodySourceRect;
            this.frontSourceRect = frontSourceRect;

            if (size.HasValue)
                defSize = size.Value;
            else
            {
                defSize = new
                (
                    bodyTexture is null ? 1 : (bodySourceRect.HasValue ? bodySourceRect.Value.Width : bodyTexture.Width()),
                    bodyTexture is null ? 1 : (bodySourceRect.HasValue ? bodySourceRect.Value.Height : bodyTexture.Height())
                );
            }
        }

        /// <summary> Set the composing textures of the CelestialBodySprite </summary>
        public void SetTextures(Asset<Texture2D> bodyTexture = null, Asset<Texture2D> backTexture = null, Asset<Texture2D> frontTexture = null)
        {
            this.bodyTexture = bodyTexture;
            this.backTexture = backTexture;
            this.frontTexture = frontTexture;
        }

        public void SetCommonSourceRectangle(Rectangle? commonSourceRect = null) => 
            SetSourceRectangles(commonSourceRect, commonSourceRect, commonSourceRect);

        public void SetSourceRectangles(Rectangle? backSourceRect = null, Rectangle? bodySourceRect = null, Rectangle? frontSourceRect = null)
        {
            this.backSourceRect = backSourceRect;
            this.bodySourceRect = bodySourceRect;
            this.frontSourceRect = frontSourceRect;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawBack(spriteBatch);
            DrawBody(spriteBatch);
            DrawFront(spriteBatch);
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
                    backShader = Macrocosm.GetShader("RadialLighting");
                    float rotation = (Center - lightSource.Center).ToRotation();
                    Vector2 shadeResolution = backTexture.Size();
                    Vector4 sourceRect = backSourceRect.HasValue ? backSourceRect.Value.Normalize(backTexture.Size()) : new Vector4(0, 0, 1, 1);
                    ConfigureBackRadialShader(this, rotation, out float intensity, out Vector2 offset, out float radius, ref shadeResolution);
                    backShader.Parameters["uOffset"].SetValue(offset);
                    backShader.Parameters["uIntensity"].SetValue(intensity);
                    backShader.Parameters["uRadius"].SetValue(radius);
                    backShader.Parameters["uShadeResolution"].SetValue(shadeResolution);
                    backShader.Parameters["uSourceRect"].SetValue(sourceRect);
                        // TODO(?): Use the COLOR0 shader semantic to pull from SpriteBatch.Draw's color argument rather than a set shader parameter.
                    backShader.Parameters["uColor"].SetValue(Color.ToVector4());
                }
                else if (ConfigureBackSphericalShader is not null)
                {
                    backShader = Macrocosm.GetShader("SphereLighting");
                    ConfigureBackSphericalShader(this, lightSource, out Vector3 lightPosition, out float radius, out int pixelSize);
                    backShader.Parameters["uLightSource"].SetValue(lightPosition);
                    backShader.Parameters["uEntityPosition"].SetValue(Position);
                        // TODO(?): Make these values distict or refer to the same value. unsure of why they are like this currently.
                    backShader.Parameters["uTextureSize"].SetValue(backTexture.Size());
                    backShader.Parameters["uEntitySize"].SetValue(backTexture.Size());
                    backShader.Parameters["uRadius"].SetValue(radius);
                    backShader.Parameters["uPixelSize"].SetValue(pixelSize);
                        // TODO(?): Use the COLOR0 shader semantic to pull from SpriteBatch.Draw's color argument rather than a set shader parameter.
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
                    bodyShader = Macrocosm.GetShader("RadialLighting");
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
                    bodyShader = Macrocosm.GetShader("SphereLighting");
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
                    frontShader = Macrocosm.GetShader("RadialLighting");
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
                    frontShader = Macrocosm.GetShader("SphereLighting");
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
    }
}
