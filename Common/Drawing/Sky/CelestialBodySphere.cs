using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Macrocosm.Common.Drawing.Sky;

/// <summary>
/// A CelestialBody that uses a Mercator projection texture to draw a wrapped sphere.
/// </summary>
public class CelestialBodySphere : CelestialBody
{
    /// <param name="radius">A value from 0-1 which affects how much of the drawn sprite is given to the atmosphere.</param>
    public delegate void FuncConfigureSphereShader(
        CelestialBodySphere celestialBody,
        CelestialBody lightSource,
        out Vector3 lightPosition,
        out Vector4 lightColor,
        out Vector4 shadowColor,
        out Vector4 lightAtmosphereColor,
        out Vector4 shadowAtmosphereColor,
        out Matrix bodyRotation,
        out float radius);

    public FuncConfigureSphereShader ConfigureSphereShader = null;

    #region Private fields

    private Asset<Texture2D> projectionTexture;

    #endregion

    public CelestialBodySphere
    (
        Asset<Texture2D> projectionTexture,
        Vector2 size,
        float scale = 1f,
        float rotation = 0f
    )
        : base(scale, rotation, size) =>
        this.projectionTexture = projectionTexture;

    /// <summary> Set the projection texture of the CelestialBody </summary>
    public void SetTexture(Asset<Texture2D> projectionTexture) =>
        this.projectionTexture = projectionTexture;

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Effect sphereShader = null;
        if (ConfigureSphereShader is not null)
        {
                // TODO(?): Rename the original SphereLighting shader to 'FauxSphereLighting'.
            sphereShader = Macrocosm.GetShader("SphereProjection");
            ConfigureSphereShader(
                this,
                LightSource,
                out Vector3 lightPosition,
                out Vector4 lightColor,
                out Vector4 shadowColor,
                out Vector4 lightAtmosphereColor,
                out Vector4 shadowAtmosphereColor,
                out Matrix bodyRotation,
                out float radius);

            sphereShader.Parameters["uLightSource"].SetValue(lightPosition);
            sphereShader.Parameters["uLightColor"].SetValue(lightColor);
            sphereShader.Parameters["uShadowColor"].SetValue(shadowColor);
            sphereShader.Parameters["uLightAtmosphereColor"].SetValue(lightAtmosphereColor);
            sphereShader.Parameters["uShadowAtmosphereColor"].SetValue(shadowAtmosphereColor);
            sphereShader.Parameters["uEntityPosition"].SetValue(Position);
            sphereShader.Parameters["uEntitySize"].SetValue(Size);
            sphereShader.Parameters["uOrientation"].SetValue(bodyRotation);
            sphereShader.Parameters["uRadius"].SetValue(radius);
            sphereShader.Parameters["uColor"].SetValue(Color.ToVector4());
        }

        if (ResetSpritebatch)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, sphereShader, state.Matrix);

        spriteBatch.Draw(projectionTexture.Value, new((int)Center.X, (int)Center.Y, (int)Size.X, (int)Size.Y), null, Color, Rotation, projectionTexture.Value.Size() * .5f, default, 0f);

        if (ResetSpritebatch)
            spriteBatch.End();
    }
}
