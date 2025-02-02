using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public abstract class BaseBooster : AnimatedRocketModule
    {
        public override int DrawPriority => 1;
        public override bool Interactible => false;

        public virtual float? ExhaustOffsetX => null;
        protected virtual Vector2? LandingLegDrawOffset => null;


        protected abstract int Direction { get; }
        protected virtual string LandingLegPath => TexturePath.Replace(Name, "LandingLeg" + (Direction > 0 ? "Right" : "Left"));

        private SpriteBatchState state1, state2;
        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            if (!ExhaustOffsetX.HasValue)
                return;

            state2.SaveState(spriteBatch, true);

            if (Rocket.ForcedFlightAppearance || Rocket.State is not Rocket.ActionState.Idle and not Rocket.ActionState.PreLaunch)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state2);

                if (Rocket.State is Rocket.ActionState.StaticFire)
                    DrawTrail(position, 0.5f + 0.3f * Utility.QuadraticEaseIn(Rocket.StaticFireProgress));

                if (Rocket.State is Rocket.ActionState.Flight || Rocket.ForcedFlightAppearance)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.FlightProgress, 0f, 0.1f) * 10f));

                if (Rocket.State is Rocket.ActionState.Landing)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.LandingProgress, 0f, 0.1f) * 10f));

                if (Rocket.State is Rocket.ActionState.Docking)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.DockingProgress, 0f, 0.1f) * 10f));

                if (Rocket.State is Rocket.ActionState.Undocking)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.UndockingProgress, 0f, 0.1f) * 10f));

                spriteBatch.End();
                spriteBatch.Begin(state2);
            }
        }

        private Asset<Texture2D> landingLegTexture;
        private Mesh2D landingLegMesh;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            if (!LandingLegDrawOffset.HasValue)
                return;

            landingLegMesh ??= new(spriteBatch.GraphicsDevice);
            landingLegTexture ??= ModContent.Request<Texture2D>(LandingLegPath, AssetRequestMode.ImmediateLoad);

            Rectangle frame = landingLegTexture.Frame(1, base.NumberOfFrames, frameY: CurrentFrame);
            Func<Vector2, Color> getDrawColor = inWorld ? Rocket.GetDrawColor : (_) => Color.White;
            landingLegMesh.CreateRectangle(position + (LandingLegDrawOffset ?? default), frame.Width, frame.Height, horizontalResolution: 2, verticalResolution: 2, colorFunction: getDrawColor);

            state1.SaveState(spriteBatch);
            spriteBatch.End();
            landingLegMesh.Draw(landingLegTexture.Value, state1.Matrix, sourceRect: frame, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Begin(state1);
        }

        private void DrawTrail(Vector2 position, float intensity = 1f)
        {
            VertexStrip strip = new();
            int stripDataCount = (int)(38 * intensity);
            if (stripDataCount < 0)
                stripDataCount = 0;
            Vector2[] positions = new Vector2[stripDataCount];
            float[] rotations = new float[stripDataCount];
            Array.Fill(positions, new Vector2(position.X + (ExhaustOffsetX ?? default), position.Y + Height));
            Array.Fill(rotations, MathHelper.Pi + MathHelper.PiOver2);

            for (int i = 0; i < stripDataCount; i++)
                positions[i] += new Vector2(0f, 4f * i);

            var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                .UseProjectionMatrix(doUse: false)
                .UseSaturation(-2.2f)
                .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
                .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail2"))
                .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail2"));

            shader.Apply();

            strip.PrepareStrip(
                positions,
                rotations,
                (progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * intensity / intensity)), new Color(255, 0, 0, 0), 0f),
                (progress) => MathHelper.Lerp(15, 45, progress)
            );

            strip.DrawTrail();
        }
    }
}
