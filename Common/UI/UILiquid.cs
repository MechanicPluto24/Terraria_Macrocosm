using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UILiquid : UIElement
    {
        private readonly Asset<Texture2D> texture;

        private Rectangle surfaceSourceRectangle;
        private Rectangle fillSourceRectangle;

        private /*const*/ readonly int sliceSize = 1;
        private /*const*/ readonly int surfaceSliceHeight = 3;

        private readonly LiquidType liquidType;

        public float LiquidLevel { get; set; } = 0f;
        public float WaveFrequency { get; set; } = 5f;
        public float WaveAmplitude { get; set; } = 0.1f;
        public bool RoundCorners { get; set; } = false;
        public bool Bubbles { get; set; } = true;
        public float Opacity { get; set; } = 1f;

        public UILiquid(LiquidType liquidType)
        {
            this.liquidType = liquidType;

            texture = liquidType switch
            {
                LiquidType.Water => LiquidRenderer.Instance._liquidTextures[WaterStyleID.Purity],
                LiquidType.Lava => LiquidRenderer.Instance._liquidTextures[WaterStyleID.Lava],
                LiquidType.Honey => LiquidRenderer.Instance._liquidTextures[WaterStyleID.Honey],
                LiquidType.Shimmer => LiquidRenderer.Instance._liquidTextures[14],
                LiquidType.Oil => ModContent.Request<Texture2D>("Macrocosm/Content/Liquids/" + liquidType.ToString(), AssetRequestMode.ImmediateLoad),
                LiquidType.RocketFuel => ModContent.Request<Texture2D>("Macrocosm/Content/Liquids/" + liquidType.ToString(), AssetRequestMode.ImmediateLoad),
                _ => Macrocosm.EmptyTex,
            };

            surfaceSourceRectangle = new(16, 1280, sliceSize * 2, surfaceSliceHeight);
            fillSourceRectangle = new(16, 64, sliceSize, sliceSize);
            OverflowHidden = true;
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle fillArea = GetFillArea();
            List<Particle> bubbles = ParticleManager.GetParticlesDrawnBy(this);

            if (Bubbles && bubbles.Count < (float)(20 * LiquidLevel))
            {
                if (liquidType == LiquidType.RocketFuel)
                {
                    Particle.Create<RocketFuelBubble>((p) =>
                    {
                        p.Position = new(fillArea.X + Main.rand.NextFloat(fillArea.Width), fillArea.Bottom);
                        p.Velocity = new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.5f, 2.2f) * -1f * LiquidLevel);
                        p.Scale = new(Main.rand.NextFloat(0.3f, 0.7f));
                        p.CustomDrawer = this;
                    });
                }
                /*
                else if (liquidType == LiquidType.Oil)
                {
                    Rectangle fillArea = GetFillArea();
                    Particle.CreateParticle<OilBubble>((p) =>
                    {
                        p.Position = new(fillArea.X + Main.rand.NextFloat(fillArea.Width), fillArea.Bottom);
                        p.MaxY = fillArea.Top;
                        p.Velocity = new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.5f, 2f) * -1f * LiquidLevel);
                        p.Scale = Main.rand.NextFloat(0.3f, 0.7f);
                        p.CustomDrawer = this;
                    });
                }
                */
            }

            foreach (Particle bubble in bubbles)
            {
                if (bubble.Position.X < fillArea.X)
                    bubble.Velocity.X = 0.5f;

                if (bubble.Position.X > fillArea.Right)
                    bubble.Velocity.X = -0.5f;

                if (bubble.Position.Y < fillArea.Top)
                    bubble.Kill();
            }
        }

        SpriteBatchState state;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Parent.OverflowHidden)
            {
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(state.SpriteSortMode, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, CustomRasterizerStates.ScissorTest, state.Effect, state.Matrix);
            }

            Rectangle fillArea = GetFillArea();
            float time = (float)Main.gameTimeCache.TotalGameTime.TotalSeconds;

            float secondaryWaveAmplitude = WaveAmplitude * 0.5f;
            float secondaryWaveFrequency = WaveFrequency * 1.5f;
            float tertiaryWaveAmplitude = WaveAmplitude * 0.25f;
            float tertiaryWaveFrequency = WaveFrequency * 2f;
            float quaternaryWaveAmplitude = WaveAmplitude * 0.15f;
            float quaternaryWaveFrequency = WaveFrequency * 2f;

            Rectangle dims = GetDimensions().ToRectangle();

            for (int x = 0; x < fillArea.Width; x += sliceSize)
            {
                float primaryWaveOffset = (float)Math.Sin(time * WaveFrequency + x * 0.1f) * WaveAmplitude;
                float secondaryWaveOffset = (float)Math.Sin(time * secondaryWaveFrequency + x * 0.1f) * secondaryWaveAmplitude;
                float tertiaryWaveOffset = (float)Math.Sin(time * tertiaryWaveFrequency + x * 0.05f) * tertiaryWaveAmplitude;
                float quaternaryWaveOffset = (float)Math.Sin(time * quaternaryWaveFrequency + x * 0.15f) * quaternaryWaveAmplitude;
                float totalWaveOffset = primaryWaveOffset + secondaryWaveOffset + tertiaryWaveOffset + quaternaryWaveOffset;

                float waveTop = fillArea.Top + totalWaveOffset;

                int fillBottom = dims.Bottom;
                if (RoundCorners && (x < 2 || x >= dims.Width - 2))
                    fillBottom -= 2;

                int waveFillHeight = fillBottom - (int)waveTop;
                if (waveFillHeight > 0)
                {
                    spriteBatch.Draw(texture.Value, new Rectangle(fillArea.X + x, (int)waveTop, sliceSize, waveFillHeight), fillSourceRectangle, Color.White * Opacity);
                }

                spriteBatch.Draw(texture.Value, new Vector2(fillArea.X + x, waveTop - surfaceSliceHeight), surfaceSourceRectangle, Color.White * 0.8f * Opacity);

            }

            if (Bubbles)
            {
                foreach (var bubble in ParticleManager.GetParticlesDrawnBy(this))
                    bubble.Draw(spriteBatch, Vector2.Zero, Color.White * 0.5f);
            }

            if (Parent.OverflowHidden)
            {
                spriteBatch.End();
                spriteBatch.Begin(state);
            }
        }

        private void DrawWaves(SpriteBatch spriteBatch, Rectangle fillArea)
        {

        }

        private Rectangle GetFillArea()
        {
            Rectangle baseArea = GetDimensions().ToRectangle();
            int fluidHeight = (int)(baseArea.Height * LiquidLevel);
            return new Rectangle(baseArea.X, baseArea.Y + baseArea.Height - fluidHeight, baseArea.Width, fluidHeight);
        }
    }
}