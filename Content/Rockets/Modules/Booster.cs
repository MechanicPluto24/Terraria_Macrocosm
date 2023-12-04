using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract class Booster : AnimatedRocketModule
    {
        public override int DrawPriority => 1;
        public abstract float ExhaustOffsetX { get; }
        protected abstract Vector2 LandingLegDrawOffset { get; }

        private SpriteBatchState state1, state2;

        protected Booster(Rocket rocket) : base(rocket)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // Draw the booster module with the base logic
            base.Draw(spriteBatch, position);

            state1.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state1);

            Texture2D tex = ModContent.Request<Texture2D>(TexturePath + "_LandingLeg").Value;
            spriteBatch.Draw(tex, position + LandingLegDrawOffset, tex.Frame(1, NumberOfFrames, frameY: CurrentFrame), Color.White);

            spriteBatch.End();
            spriteBatch.Begin(state1);
        }

        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position)
        {
            state2.SaveState(spriteBatch, true);

            if (rocket.StaticFire || rocket.InFlight || rocket.ForcedFlightAppearance)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state2);

                if (rocket.StaticFire)
                    DrawTrail(position, 0.5f + 0.3f * Utility.QuadraticEaseIn(rocket.StaticFireProgress));

                if (rocket.InFlight || rocket.ForcedFlightAppearance)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(rocket.FlightProgress, 0f, 0.1f) * 10f));

                spriteBatch.End();
                spriteBatch.Begin(state2);
            }
        }

        private void DrawTrail(Vector2 position, float intensity = 1f)
        {
            VertexStrip strip = new();
            int stripDataCount = (int)(38 * intensity);
            Vector2[] positions = new Vector2[stripDataCount];
            float[] rotations = new float[stripDataCount];
            Array.Fill(positions, new Vector2(position.X + ExhaustOffsetX, position.Y + Height));
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
                (float progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * intensity / intensity)), new Color(255, 0, 0, 0), 0f),
                (float progress) => MathHelper.Lerp(15, 45, progress)
            );

            strip.DrawTrail();
        }
    }
}
