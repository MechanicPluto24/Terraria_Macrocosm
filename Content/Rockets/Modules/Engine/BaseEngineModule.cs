using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Engine
{
    public abstract class BaseEngineModule : AnimatedRocketModule
    {
        protected virtual string LandingLegPath => TexturePath.Replace(Name, "LandingLeg");
        protected virtual string BoosterRear => TexturePath.Replace(Name, "BoosterRear");

        private SpriteBatchState state1, state2;
        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            state1.SaveState(spriteBatch, true);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.AlphaBlend, SamplerState.PointClamp, state1);

            Color lightColor = Color.White;

            // Draw the rear landing behind the rear booster 
            Texture2D rearLandingLeg = ModContent.Request<Texture2D>(LandingLegPath, AssetRequestMode.ImmediateLoad).Value;
            Rectangle rearLandingLegFrame = rearLandingLeg.Frame(1, base.NumberOfFrames, frameY: CurrentFrame);
            Vector2 drawPos = position + new Vector2(Width / 2f - rearLandingLeg.Width / 2f, Height + rearLandingLegFrame.Height / 2);

            if (inWorld)
                lightColor = Lighting.GetColor((drawPos + Main.screenPosition).ToTileCoordinates());

            spriteBatch.Draw(rearLandingLeg, drawPos, rearLandingLegFrame, lightColor * Rocket.Transparency);

            // Draw the rear booster behind the engine module 
            Texture2D boosterRear = ModContent.Request<Texture2D>(BoosterRear, AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(boosterRear, position + new Vector2(Width / 2f - boosterRear.Width / 2f, Height - boosterRear.Height / 4), null, lightColor * Rocket.Transparency, 0f, Origin, 1f, SpriteEffects.None, 0f);

            // Draw the exhaust trail 
            if (Rocket.ForcedFlightAppearance || Rocket.State is not Rocket.ActionState.Idle and not Rocket.ActionState.PreLaunch)
            {
                spriteBatch.End();
                spriteBatch.Begin(BlendState.Additive, state1);

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
            }

            spriteBatch.End();
            spriteBatch.Begin(state1);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            state2.SaveState(spriteBatch, true);
            spriteBatch.End();
            spriteBatch.Begin(state2);

            // Draw the engine module with the base logic
            base.Draw(spriteBatch, position);

            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state2);

            // Draw the nameplate
            Rocket.Nameplate.Draw(spriteBatch, position + new Vector2(Width / 2, 0));

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
            Array.Fill(positions, new Vector2(position.X + Width / 2f, position.Y + Height - 28));
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
                (progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * (1 - intensity))), new Color(255, 0, 0, 0), Utility.QuadraticEaseIn(progress)),
                (progress) => MathHelper.Lerp(40, 75, progress)
            );

            strip.DrawTrail();
        }
    }
}
