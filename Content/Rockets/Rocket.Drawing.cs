using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Rockets.Modules.Boosters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using static Terraria.GameContent.TextureAssets;

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

        private RenderTarget2D[] renderTargets = new RenderTarget2D[3];

        private Mesh2D mesh;
        private SpriteBatchState sbState;

        public void ResetRenderTarget()
        {
            foreach (var renderTarget in renderTargets)
                renderTarget?.Dispose();
        }

        public Color GetDrawColor(Vector2 vertexDrawPosition) => new Color(Lighting.GetSubLight(vertexDrawPosition + Main.screenPosition)) * Transparency;

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
                renderTargets[(int)drawMode] = GetRenderTarget(drawMode);

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

        private void DrawLightedMesh(Vector2 position)
        {
            mesh ??= new(Main.graphics.GraphicsDevice);
            mesh.CreateRectangle(position, Width, Height, horizontalResolution: 6, verticalResolution: 8, colorFunction: GetDrawColor);
            mesh.Draw(renderTargets[(int)DrawMode.World], Main.Transform, rotation: Rotation, origin: Center - Main.screenPosition, samplerState: SamplerState.PointClamp);
        }

        private void DrawDummyWithRenderTarget(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, sbState.Matrix);
            spriteBatch.Draw(renderTargets[(int)DrawMode.Dummy], position, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(sbState.SpriteSortMode, BlendState.Additive, sbState.SamplerState, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, Main.UIScaleMatrix);
            DrawOverlay(spriteBatch, position);
            spriteBatch.End();
        }

        public RenderTarget2D GetRenderTarget(DrawMode drawMode)
        {
            // If it already exists, return it.
            RenderTarget2D target = renderTargets[(int)drawMode];
            if (target is not null && !target.IsDisposed)
                return target;

            var spriteBatch = Main.spriteBatch;

            Rectangle renderBounds = new(0, 0, Bounds.Width, Bounds.Height);

            foreach (var module in Modules)
                renderBounds = module.ModifyRenderBounds(renderBounds, drawMode);

            Vector2 drawOffset = new(renderBounds.X, renderBounds.Y);

            target = new(spriteBatch.GraphicsDevice, renderBounds.Width, renderBounds.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            renderTargets[(int)drawMode] = target;

            // Store previous settings
            //var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            //var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            // Capture original RenderTargets and preserve their contents
            spriteBatch.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
           //RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
           //foreach (var binding in originalRenderTargets)
           //    typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

            // Draw our modules
            sbState = spriteBatch.SaveState();
            spriteBatch.EndIfBeginCalled();

            spriteBatch.GraphicsDevice.SetRenderTarget(target);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, sbState.DepthStencilState, sbState.RasterizerState, sbState.Effect, Matrix.CreateScale(1f));

            switch (drawMode)
            {
                case DrawMode.World:
                    DrawWorld(spriteBatch, drawOffset);
                    break;

                case DrawMode.Dummy:
                    DrawDummy(spriteBatch, drawOffset);
                    break;

                case DrawMode.Blueprint:
                    DrawBlueprint(spriteBatch, drawOffset);
                    break;  
            }

            spriteBatch.End();

            // Revert our RenderTargets back to the vanilla ones
            //if (originalRenderTargets.Length > 0)
            //    spriteBatch.GraphicsDevice.SetRenderTargets(originalRenderTargets);
            //else
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            // Reset our settings back to the previous ones
            //spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            //spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;

            return target;
        }

        // Draw types
        private void DrawWorld(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
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
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                 module.PreDrawBeforeTiles(spriteBatch, GetModuleRelativePosition(module, position), inWorld);
            }
        }

        public void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld = true)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                 module.PostDraw(spriteBatch, GetModuleRelativePosition(module, position), inWorld);
            }
        }

        private void DrawBlueprint(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority.OrderBy(module => module.BlueprintHighlighted))
            {
                Vector2 drawPosition = GetModuleRelativePosition(module, position);
                if (module.IsBlueprint)
                {
                    module.DrawBlueprint(spriteBatch, drawPosition);
                }
                else
                {
                    module.PreDrawBeforeTiles(spriteBatch, drawPosition, inWorld: false);
                    module.Draw(spriteBatch, drawPosition);
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
                spriteBatch.Draw(flare, position + new Vector2(Bounds.Width / 2, Bounds.Height).RotatedBy(Rotation), null, new Color(255, 69, 0), Rotation, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }
    }
}