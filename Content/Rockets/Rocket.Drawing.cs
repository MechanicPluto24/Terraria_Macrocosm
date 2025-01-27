using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public enum DrawMode
        {
            World,
            Dummy,
            Blueprint
        }

        private RenderTarget2D renderTarget;
        private Mesh2D mesh;
        private SpriteBatchState sbState;
        private bool firstDraw = true;

        public bool HasRenderTarget => renderTarget is not null && !renderTarget.IsDisposed;
        public void ResetRenderTarget()
        {
            renderTarget?.Dispose();
        }

        /// <summary> Draw the rocket </summary>
        /// <param name="drawMode"> 
        /// The drawing mode: <list type="bullet">
        /// <item> <see cref="DrawMode.World"/>: Draws the rocket in world </item>
        /// <item> <see cref="DrawMode.Dummy"/>: Draws the rocket as a dummy, for use in UI </item>
        /// <item> <see cref="DrawMode.Blueprint"/>:  Draws the rocket as a blueprint, for use in the assembly UI </item>
        ///</list> </param>
        /// <param name="spriteBatch"> The spritebatch </param>
        /// <param name="position"> The draw position </param>
        /// <param name="useRenderTarget"> 
        /// Whether to use a render target or draw the rocket directly.
        /// <br/> For performance considerations, prefer to use a render target for drawing, except where visual changes happen very often.
        /// <br/> NOTE: In world, you must use a render target in order for the lighting to work.
        /// </param>
        public void Draw(DrawMode drawMode, SpriteBatch spriteBatch, Vector2 position, bool useRenderTarget = true)
        {
            // TODO: find a way to apply reverse gravity to Rockets with RenderTarget
            if (Main.LocalPlayer.gravDir == -1f)
                useRenderTarget = false;

            if (useRenderTarget)
            {
                // Prepare our RenderTarget
                renderTarget = GetRenderTarget(drawMode);

                // Save our SpriteBatch state
                sbState.SaveState(spriteBatch);
                spriteBatch.EndIfBeginCalled();

                switch (drawMode)
                {
                    // Only DrawMode.World consumes the lighting buffers
                    case DrawMode.World:
                        DrawLightedMesh(position);
                        break;
                    case DrawMode.Dummy:
                        DrawDummyWithRenderTarget(spriteBatch, position);
                        break;
                }

                // Reset our SpriteBatch to its previous state
                spriteBatch.Begin(sbState);
            }
            else
            {
                switch (drawMode)
                {
                    case DrawMode.World:
                        DrawWorld(spriteBatch, position);
                        break;

                    case DrawMode.Dummy:
                        DrawDummy(spriteBatch, position);
                        break;

                    case DrawMode.Blueprint:
                        DrawBlueprint(spriteBatch, position);
                        break;
                }
            }
        }

        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;

        private void DrawLightedMesh(Vector2 position)
        {
            mesh = new(Main.graphics.GraphicsDevice);

            mesh.CreateRectangle
            (
                position,
                Width,
                Height,
                horizontalResolution: 6,
                verticalResolution: 8,
                (vertexPos) => new Color(Lighting.GetSubLight(vertexPos + Main.screenPosition))
            );

            mesh.Draw(renderTarget, GraphicsSystem.WorldViewProjection, BlendState.AlphaBlend, SamplerState.AnisotropicClamp);
        }

        public RenderTarget2D GetRenderTarget(DrawMode drawMode)
        {
            // We only need to prepare our RenderTarget if it's not ready to use
            if (renderTarget is not null && !renderTarget.IsDisposed)
                return renderTarget;

            // Initialize our RenderTarget
            renderTarget = new(Main.spriteBatch.GraphicsDevice, Bounds.Width, Bounds.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            PrepareRenderTarget(Main.spriteBatch, drawMode);

            return renderTarget;
        }

        private void PrepareRenderTarget(SpriteBatch spriteBatch, DrawMode drawMode)
        {
            // Store previous settings
            var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            // Capture original RenderTargets and preserve their contents
            spriteBatch.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
            foreach (var binding in originalRenderTargets)
                typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

            // Draw our modules
            sbState = spriteBatch.SaveState();
            spriteBatch.EndIfBeginCalled();

            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, Matrix.CreateScale(1f));

            switch (drawMode)
            {
                case DrawMode.World:
                    DrawWorld(spriteBatch, default);
                    break;

                case DrawMode.Dummy:
                    DrawDummy(spriteBatch, default);
                    break;

                case DrawMode.Blueprint:
                    DrawBlueprint(spriteBatch, default);
                    break;
            }

            spriteBatch.End();

            // Revert our RenderTargets back to the vanilla ones
            if (originalRenderTargets.Length > 0)
            {
                spriteBatch.GraphicsDevice.SetRenderTargets(originalRenderTargets);
            }
            else
            {
                spriteBatch.GraphicsDevice.SetRenderTarget(null);
            }

            // Reset our settings back to the previous ones
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
        }

        private void DrawDummyWithRenderTarget(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, sbState.Matrix);
            PreDrawBeforeTiles(spriteBatch, position, inWorld: false);
            spriteBatch.Draw(renderTarget, position, Color.White);
            PostDraw(spriteBatch, position, inWorld: false);
            spriteBatch.End();
            spriteBatch.Begin(sbState.SpriteSortMode, BlendState.Additive, sbState.SamplerState, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, Main.UIScaleMatrix);
            DrawOverlay(spriteBatch, position);
            spriteBatch.End();
        }

        // Draw types
        private void DrawWorld(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ActiveModulesByDrawPriority)
            {
                if (module.Active)
                    module.Draw(spriteBatch, GetModuleRelativePosition(module, position));
            }
        }

        private void DrawDummy(SpriteBatch spriteBatch, Vector2 position)
        {
            PreDrawBeforeTiles(spriteBatch, position, inWorld: false);
            DrawWorld(spriteBatch, position);
            PostDraw(spriteBatch, position, inWorld: false);

            sbState.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(sbState.SpriteSortMode, BlendState.Additive, sbState.SamplerState, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, Main.UIScaleMatrix);
            DrawOverlay(spriteBatch, position);
            spriteBatch.End();
            spriteBatch.Begin(sbState);
        }

        public void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld = true)
        {
            foreach (RocketModule module in ActiveModulesByDrawPriority)
            {
                if(module.Active)
                    module.PreDrawBeforeTiles(spriteBatch, GetModuleRelativePosition(module, position), inWorld);
            }
        }

        public void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld = true)
        {
            foreach (RocketModule module in ActiveModulesByDrawPriority)
            {
                if (module.Active)
                    module.PostDraw(spriteBatch, GetModuleRelativePosition(module, position), inWorld);
            }
        }

        private void DrawBlueprint(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ActiveModulesByDrawPriority.OrderBy(module => module.BlueprintHighlighted))
            {
                Vector2 drawPosition = GetModuleRelativePosition(module, position);

                if (module.Active)
                {
                    if (module.IsBlueprint)
                    {
                        if (module is BoosterLeft)
                            drawPosition.X -= 78;

                        module.DrawBlueprint(spriteBatch, drawPosition);
                    }
                    else
                    {
                        module.PreDrawBeforeTiles(spriteBatch, drawPosition, inWorld: false);
                        module.Draw(spriteBatch, drawPosition);
                    }
                }
            }
        }

        public void DrawOverlay(SpriteBatch spriteBatch, Vector2 position)
        {
            if (ForcedFlightAppearance || (State != ActionState.Idle && State != ActionState.PreLaunch))
            {
                float scale = 1.2f * Main.rand.NextFloat(0.85f, 1f);

                if (ForcedFlightAppearance)
                    scale *= 1.25f;

                if (State == ActionState.StaticFire)
                    scale *= Utility.QuadraticEaseOut(StaticFireProgress);

                if (State == ActionState.Landing && LandingProgress > 0.9f)
                    scale *= Utility.QuadraticEaseOut((1f - LandingProgress) * 10f);

                if (State == ActionState.Docking && DockingProgress > 0.9f)
                    scale *= Utility.QuadraticEaseOut((1f - DockingProgress) * 10f);

                if (State == ActionState.Undocking && UndockingProgress < 0.1f)
                    scale *= Utility.QuadraticEaseOut((UndockingProgress) * 10f);

                var flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare2").Value;
                spriteBatch.Draw(flare, position + new Vector2(Bounds.Width / 2, Bounds.Height), null, new Color(255, 69, 0), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }
    }
}