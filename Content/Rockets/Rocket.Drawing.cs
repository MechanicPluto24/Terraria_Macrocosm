using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.CursorIcons;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Rockets.Storage;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        private RenderTarget2D renderTarget;
        private SpriteBatchState state;
        private void ResetRenderTarget() => renderTarget?.Dispose();

        /// <summary> Draw the rocket to a RenderTarget and then in the world </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Module relative positions (also) set here, in the update method only they lag behind 
            SetModuleRelativePositions();

            state.SaveState(spriteBatch);
            if (renderTarget is null || renderTarget.IsDisposed)
            {
                RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
                foreach(var binding in originalRenderTargets)
                    typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

                renderTarget = new(spriteBatch.GraphicsDevice, Bounds.Width, Bounds.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                spriteBatch.End();
                spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, state.Effect, Matrix.CreateScale(1f));

                foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
                {
                    // Cancel out world position
                    module.Draw(spriteBatch, screenPos: Position, drawColor);
                }

                spriteBatch.End();
                spriteBatch.GraphicsDevice.SetRenderTargets(originalRenderTargets);
            }

            spriteBatch.EndIfBeginCalled();

            SamplerState samplerState = Stationary ? SamplerState.PointClamp : SamplerState.LinearClamp;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);
           
            spriteBatch.Draw(renderTarget, Position - screenPos, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }

        /// <summary> Draw the rocket directly </summary>
        public void DrawDirect(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Module relative positions (also) set here, in the update method only they lag behind 
            SetModuleRelativePositions();

            foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
            {
                module.Draw(spriteBatch, screenPos, drawColor);
            }
        }

        /// <summary> Draw the rocket as a dummy </summary>
        public void DrawDummy(SpriteBatch spriteBatch, Vector2 offset, Color drawColor)
        {
            // Set module relative positions also before PreDraw...
            SetModuleRelativePositions();

            // Passing Rocket world position as "screenPosition" cancels it out  
            PreDrawBeforeTiles(spriteBatch, Position - offset, drawColor);
            DrawDirect(spriteBatch, Position - offset, drawColor);
            DrawOverlay(spriteBatch, Position - offset);
        }

        public void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
            {
                module.PreDrawBeforeTiles(spriteBatch, screenPos, drawColor);
            }
        }

        public void DrawOverlay(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            foreach (RocketModule module in Modules.Values.OrderBy(module => module.DrawPriority))
            {
                module.DrawOverlay(spriteBatch, screenPos);
            }
        }
    }
}
