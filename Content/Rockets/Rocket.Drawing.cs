using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI.Themes;
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
using static Macrocosm.Content.Tiles.Furniture.MoonBase.MoonBaseChest;
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
        private SpriteBatchState state1;
        public void ResetRenderTarget() => renderTarget?.Dispose();

        /// <summary> Draw the rocket to a RenderTarget and then in the world </summary>
        public void Draw(DrawMode drawMode, SpriteBatch spriteBatch, Vector2 position)
        {
            state1.SaveState(spriteBatch);
            renderTarget = GetOrPrepareRenderTarget(drawMode);

            spriteBatch.EndIfBeginCalled();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state1.DepthStencilState, state1.RasterizerState, state1.Effect, state1.Matrix);
           
            spriteBatch.Draw(renderTarget, position, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(state1);
        }

        public void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                module.PreDrawBeforeTiles(spriteBatch, GetModuleRelativePosition(module, position));
            }
        }

        public void DrawOverlay(SpriteBatch spriteBatch, Vector2 position)
        {
            if (InFlight || ForcedFlightAppearance)
            {
                float scale = 1.2f * Main.rand.NextFloat(0.85f, 1f);
                if (FlightProgress < 0.1f)
                    scale *= Utility.QuadraticEaseOut(FlightProgress * 10f);

                var flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Flare2").Value;
                spriteBatch.Draw(flare, position + new Vector2(Bounds.Width/2, Bounds.Height), null, new Color(255, 69, 0), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }

        public RenderTarget2D GetOrPrepareRenderTarget(DrawMode drawMode)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            if (renderTarget is null || renderTarget.IsDisposed)
            {
                RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
                foreach (var binding in originalRenderTargets)
                    typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

                renderTarget = new(spriteBatch.GraphicsDevice, Bounds.Width, Bounds.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(renderTarget);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state1.DepthStencilState, state1.RasterizerState, state1.Effect, Matrix.CreateScale(1f));

                switch (drawMode)
                {
                    case DrawMode.World:
                        DrawDirect(spriteBatch, default);
                        break;

                    case DrawMode.Dummy:
                        DrawDummy(spriteBatch, default);
                        break;

                    case DrawMode.Blueprint:
                        DrawBlueprint(spriteBatch, default);
                        break;
                }

                spriteBatch.End();

                graphicsDevice.SetRenderTargets(originalRenderTargets);
            }

            return renderTarget;
        }

        private void DrawDirect(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                module.Draw(spriteBatch, GetModuleRelativePosition(module, position));
            }
        }

        private void DrawDummy(SpriteBatch spriteBatch, Vector2 position)
        {
            PreDrawBeforeTiles(spriteBatch, position);
            DrawDirect(spriteBatch, position);
            DrawOverlay(spriteBatch, position);
        }

        private void DrawBlueprint(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                Vector2 drawPosition = GetModuleRelativePosition(module, position);

                if (module is BoosterLeft)
                    drawPosition.X -= 78;

                module.DrawBlueprint(spriteBatch, drawPosition);
            }
        }
    }
}
