using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Engine
{
    public abstract class BaseEngineModule : AnimatedRocketModule
    {
        protected Asset<Texture2D> RearLandingLeg => _rearLandingLeg ??= ModContent.Request<Texture2D>(Texture.Replace(Name, "LandingLeg"), AssetRequestMode.ImmediateLoad);
        private Asset<Texture2D> _rearLandingLeg;
        protected Rectangle RearLandingLegFrame => RearLandingLeg.Frame(1, base.NumberOfFrames, frameY: CurrentFrame);

        protected Asset<Texture2D> BoosterRear => _boosterRear ??= ModContent.Request<Texture2D>(Texture.Replace(Name, "BoosterRear"), AssetRequestMode.ImmediateLoad);
        private Asset<Texture2D> _boosterRear;

        protected virtual Vector2? LandingLegDrawOffset => null;
        protected virtual Vector2? BoosterRearDrawOffset => null;


        private SpriteBatchState state1, state2;
        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            // Draw the exhaust trail 
            if (Rocket.ForcedFlightAppearance || Rocket.State is not Rocket.ActionState.Idle and not Rocket.ActionState.PreLaunch)
            {
                state1.SaveState(spriteBatch, true);
                spriteBatch.End();
                spriteBatch.Begin(BlendState.Additive, state1);

                if (Rocket.ForcedFlightAppearance)
                    DrawTrail(position, 1.1f);

                if (Rocket.State is Rocket.ActionState.StaticFire)
                    DrawTrail(position, 0.5f + 0.3f * Utility.QuadraticEaseIn(Rocket.StaticFireProgress));

                if (Rocket.State is Rocket.ActionState.Flight)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.FlightProgress, 0f, 0.1f) * 10f));

                if (Rocket.State is Rocket.ActionState.Landing)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.LandingProgress, 0f, 0.1f) * 10f));

                if (Rocket.State is Rocket.ActionState.Docking)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.DockingProgress, 0f, 0.1f) * 10f));

                if (Rocket.State is Rocket.ActionState.Undocking)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(Rocket.UndockingProgress, 0f, 0.1f) * 10f));

                spriteBatch.End();
                spriteBatch.Begin(state1);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            state2.SaveState(spriteBatch, true);
            spriteBatch.End();
            spriteBatch.Begin(state2);

            // Draw the rear landing behind the rear booster 
            if (LandingLegDrawOffset.HasValue)
            {
                Vector2 drawPos = position + (new Vector2(Width / 2f, Height) + LandingLegDrawOffset.Value).RotatedBy(Rocket.Rotation);
                spriteBatch.Draw(RearLandingLeg.Value, drawPos, RearLandingLegFrame, Color.White * Rocket.Transparency, Rocket.Rotation, RearLandingLegFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            // Draw the rear booster behind the engine module 
            if (BoosterRearDrawOffset.HasValue)
            {
                Vector2 drawPos = position + (new Vector2(Width / 2f, Height) + BoosterRearDrawOffset.Value).RotatedBy(Rocket.Rotation);
                spriteBatch.Draw(BoosterRear.Value, drawPos, null, Color.White * Rocket.Transparency, Rocket.Rotation, BoosterRear.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            // Draw base module
            base.Draw(spriteBatch, position);

            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state2);

            // Rotate and adjust nameplate
            Vector2 nameplateOffset = new Vector2(Width / 2, 0).RotatedBy(Rocket.Rotation);
            Rocket.Nameplate.Draw(spriteBatch, position + nameplateOffset, Rocket.Rotation);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void DrawTrail(Vector2 position, float intensity = 1f)
        {
            VertexStrip strip = new();
            int stripDataCount = (int)(58 * intensity);
            if (stripDataCount < 0)
                stripDataCount = 0;

            Vector2[] positions = new Vector2[stripDataCount];
            float[] rotations = new float[stripDataCount];
            for (int i = 0; i < stripDataCount; i++)
            {
                positions[i] = position + new Vector2(Width / 2f, Height - 28 + 4f * i).RotatedBy(Rocket.Rotation);
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
                (progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * (1 - intensity))), new Color(255, 0, 0, 0), Utility.QuadraticEaseIn(progress)),
                (progress) => MathHelper.Lerp(40, 75, progress)
            );

            strip.DrawTrail();
        }
    }
}
