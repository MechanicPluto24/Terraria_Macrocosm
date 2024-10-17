﻿using Macrocosm.Common.DataStructures;
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
using static Macrocosm.Content.Tiles.Furniture.Industrial.IndustrialChest;

namespace Macrocosm.Content.Rockets.Modules
{
    public class EngineModule : AnimatedRocketModule
    {
        public override int DrawPriority => 0;
        public bool RearLandingLegRaised { get; set; } = false;

        public override int Width => 120;
        public override int Height => 302 + (RearLandingLegRaised ? 18 : 26);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 45),
            new(ModContent.ItemType<Canister>(), 15),
            new(ModContent.ItemType<EngineComponent>(), 4),
            new(ModContent.ItemType<LandingGear>(), 3)
        };

        private SpriteBatchState state1, state2;
        public override void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            state1.SaveState(spriteBatch, true);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.AlphaBlend, SamplerState.PointClamp, state1);

            Color lightColor = Color.White;

            // Draw the rear landing behind the rear booster 
            Texture2D rearLandingLeg = ModContent.Request<Texture2D>(TexturePath + "_LandingLeg", AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawPos = position + new Vector2(Texture.Width / 2f - rearLandingLeg.Width / 2f, 314f);

            if (inWorld)
                lightColor = Lighting.GetColor((drawPos + Main.screenPosition).ToTileCoordinates());

            spriteBatch.Draw(rearLandingLeg, drawPos, rearLandingLeg.Frame(1, base.NumberOfFrames, frameY: CurrentFrame), lightColor * rocket.Transparency);

            // Draw the rear booster behind the engine module 
            Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(boosterRear, position + new Vector2(Texture.Width / 2f - boosterRear.Width / 2f, 294f), null, lightColor * rocket.Transparency, 0f, Origin, 1f, SpriteEffects.None, 0f);

            // Draw the exhaust trail 
            if ( rocket.ForcedFlightAppearance || (rocket.State != Rocket.ActionState.Idle && rocket.State != Rocket.ActionState.PreLaunch))
            {
                spriteBatch.End();
                spriteBatch.Begin(BlendState.Additive, state1);

                if (rocket.State == Rocket.ActionState.StaticFire)
                    DrawTrail(position, 0.5f + 0.3f * Utility.QuadraticEaseIn(rocket.StaticFireProgress));

                if (rocket.State == Rocket.ActionState.Flight || rocket.ForcedFlightAppearance)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(rocket.FlightProgress, 0f, 0.1f) * 10f));

                if (rocket.State == Rocket.ActionState.Landing)
                    DrawTrail(position, MathHelper.Lerp(0.8f, 1f, MathHelper.Clamp(rocket.LandingProgress, 0f, 0.1f) * 10f));
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
            rocket.Nameplate.Draw(spriteBatch, position + new Vector2(Width / 2, 0));

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void DrawTrail(Vector2 position, float intensity = 1f)
        {
            VertexStrip strip = new();
            int stripDataCount = (int)(58 * intensity);
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
                (float progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * (1 - intensity))), new Color(255, 0, 0, 0), Utility.QuadraticEaseIn(progress)),
                (float progress) => MathHelper.Lerp(40, 75, progress)
            );

            strip.DrawTrail();
        }
    }
}
