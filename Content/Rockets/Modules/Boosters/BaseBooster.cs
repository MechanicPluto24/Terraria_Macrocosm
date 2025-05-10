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

        public virtual Vector2? ExhaustOffset => null;
        protected virtual Vector2? LandingLegDrawOffset => null;


        protected abstract int Direction { get; }

        protected virtual string LandingLegPath => Texture.Replace(Name, "LandingLeg" + (Direction > 0 ? "Right" : "Left"));

        protected Rectangle LandingLegFrame => LandingLeg.Frame(1, base.NumberOfFrames, frameY: CurrentFrame);
        protected Asset<Texture2D> LandingLeg => _landingLeg ??= ModContent.Request<Texture2D>(LandingLegPath, AssetRequestMode.ImmediateLoad);
        private Asset<Texture2D> _landingLeg;

        private SpriteBatchState state1, state2;
        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            if (!ExhaustOffset.HasValue)
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

        private Mesh landingLegMesh;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            if (!LandingLegDrawOffset.HasValue)
                return;

            landingLegMesh ??= new();

            Func<Vector2, Color> getDrawColor = inWorld ? Rocket.GetDrawColor : (_) => Color.White;

            Vector2 rotatedOffset = LandingLegDrawOffset.Value.RotatedBy(Rocket.Rotation);
            Vector2 drawPosition = position + rotatedOffset;
            landingLegMesh.CreateRectangle(drawPosition, LandingLegFrame.Width, LandingLegFrame.Height, horizontalResolution: 2, verticalResolution: 2, rotation: Rocket.Rotation, origin: drawPosition, colorFunction: getDrawColor);
            state1.SaveState(spriteBatch);
            spriteBatch.End();
            landingLegMesh.Draw(LandingLeg.Value, state1.Matrix, sourceRect: LandingLegFrame, samplerState: SamplerState.PointClamp, scissorTestEnable: true);
            spriteBatch.Begin(state1);
        }

        private void DrawTrail(Vector2 position, float intensity = 1f)
        {
            if (!ExhaustOffset.HasValue)
                return;

            VertexStrip strip = new();
            int stripDataCount = (int)(38 * intensity);
            if (stripDataCount < 0)
                stripDataCount = 0;
            Vector2[] positions = new Vector2[stripDataCount];
            float[] rotations = new float[stripDataCount];
            for (int i = 0; i < stripDataCount; i++)
            {
                positions[i] = position + new Vector2(ExhaustOffset.Value.X, Height + ExhaustOffset.Value.Y + 4f * i).RotatedBy(Rocket.Rotation);
                rotations[i] = MathHelper.Pi + MathHelper.PiOver2 + Rocket.Rotation;
            }

            var shader = new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
                .UseProjectionMatrix(doUse: false)
                .UseSaturation(-2.2f)
                .UseImage0(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeOutMask"))
                .UseImage1(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "RocketExhaustTrail2"))
                .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "RocketExhaustTrail2"));

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
