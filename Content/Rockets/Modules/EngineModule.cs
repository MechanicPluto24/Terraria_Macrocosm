using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Modules
{
    public class EngineModule : AnimatedRocketModule
    {
        public override int DrawPriority => 0;
        public bool RearLandingLegRaised { get; set; } = false;
        public Nameplate Nameplate { get; set; } = new();

        public override int Width => 120;
        public override int Height => 302 + (RearLandingLegRaised ? 18 : 26);

        public EngineModule(Rocket rocket) : base(rocket)
        {
        }

        private SpriteBatchState state1, state2, state3;
        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            state1.SaveState(spriteBatch, true);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.AlphaBlend, SamplerState.PointClamp, state1);

            // Draw the rear landing behind the rear booster 
            Texture2D rearLandingLeg = ModContent.Request<Texture2D>(TexturePath + "_LandingLeg", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(rearLandingLeg, Position + new Vector2(Texture.Width / 2f - rearLandingLeg.Width / 2f, 314f) - screenPos, rearLandingLeg.Frame(1, NumberOfFrames, frameY: CurrentFrame), drawColor);

            // Draw the rear booster behind the engine module 
            Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(boosterRear, Position + new Vector2(Texture.Width / 2f - boosterRear.Width / 2f, 294f) - screenPos, null, drawColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

            // Draw the exhaust trail 
            if (rocket.StaticFire || rocket.InFlight || rocket.ForcedFlightAppearance)
            {
                spriteBatch.End();
                spriteBatch.Begin(BlendState.Additive, state1);

                if (rocket.StaticFire)
                    DrawTrail(spriteBatch, 0.5f + 0.3f * Utility.QuadraticEaseIn(rocket.StaticFireProgress));

                if (rocket.InFlight || rocket.ForcedFlightAppearance)
                    DrawTrail(spriteBatch, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(rocket.FlightProgress, 0f, 0.1f) * 10f));
            }

            spriteBatch.End();
            spriteBatch.Begin(state1);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            state2.SaveState(spriteBatch, true);
            spriteBatch.End();
            spriteBatch.Begin(state2);

            // Draw the engine module with the base logic
            base.Draw(spriteBatch, screenPos, ambientColor);

            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state2);

            // Draw the nameplate
            Nameplate.Draw(spriteBatch, new Vector2(Center.X, Position.Y) - screenPos, ambientColor);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        public override void DrawOverlay(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            // Draw the rocket flight lens flare
            if (rocket.InFlight || rocket.ForcedFlightAppearance)
            {
                state3.SaveState(spriteBatch, true);
                spriteBatch.End();
                spriteBatch.Begin(BlendState.Additive, state3);

                float scale = 1.2f * Main.rand.NextFloat(0.85f, 1f);
                if (rocket.FlightProgress < 0.1f)
                    scale *= Utility.QuadraticEaseOut(rocket.FlightProgress * 10f);

                var flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Flare2").Value;
                spriteBatch.Draw(flare, new Vector2(rocket.Center.X, rocket.Position.Y + rocket.Bounds.Height) - Main.screenPosition, null, new Color(255, 69, 0), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(state3);
            }
        }

        private void DrawTrail(SpriteBatch spriteBatch, float intensity = 1f)
        {
            VertexStrip strip = new();
            int stripDataCount = (int)(58 * intensity);
            Vector2[] positions = new Vector2[stripDataCount];
            float[] rotations = new float[stripDataCount];
            Array.Fill(positions, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition);
            Array.Fill(rotations, MathHelper.Pi + MathHelper.PiOver2);

            for (int i = 0; i < stripDataCount; i++)
                positions[i] += new Vector2(0f, 4f * i);

            var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                            .UseProjectionMatrix(doUse: false)
                            .UseSaturation(-2.2f)
                            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
                            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"))
                            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"));

            shader.Apply();

            strip.PrepareStrip(
                positions,
                rotations,
                (float progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * (1 - intensity))), new Color(255, 0, 0, 0), Utility.QuadraticEaseIn(progress)),
                (float progress) => MathHelper.Lerp(40, 75, progress)
            );

            strip.DrawTrail();
        }

        protected override TagCompound SerializeModuleSpecificData()
        {
            return new()
            {
                [nameof(Nameplate)] = Nameplate,
            };
        }

        protected override void DeserializeModuleSpecificData(TagCompound tag, Rocket ownerRocket)
        {
            if (tag.ContainsKey(nameof(Nameplate)))
                Nameplate = tag.Get<Nameplate>(nameof(Nameplate));
        }
    }
}
