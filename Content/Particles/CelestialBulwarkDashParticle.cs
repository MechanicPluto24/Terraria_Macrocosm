using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Players;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class CelestialBulwarkDashParticle : Particle
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public override int TrailCacheLength => 24;

        public int PlayerID;
        public Color? SecondaryColor;
        public float Opacity;

        private float defScale;
        private float defRotation;
        private bool collided;

        private BlendState blendStateOverride;
        private bool rainbow;

        public Player Player => Main.player[PlayerID];
        public DashPlayer DashPlayer => Player.GetModPlayer<DashPlayer>();
        public float Progress => DashPlayer.DashProgress;

        public override void SetDefaults()
        {
            TimeToLive = 1000;
            PlayerID = 0;
            Opacity = 0f;
            DrawLayer = ParticleDrawLayer.BeforeNPCs;

            collided = false;
        }

        public override void OnSpawn()
        {
            defScale = Scale.X;
            defRotation = Rotation;
            CelestialBulwark.GetEffectColor(Player, out Color, out SecondaryColor, out blendStateOverride, out _, out rainbow);
        }

        private SpriteBatchState state;
        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            bool specialRainbow = false;
            Texture2D slash = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Slash1").Value;
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle5").Value;

            if (blendStateOverride is not null)
            {
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(blendStateOverride, state);
            }

            for (int i = 0; i < TrailCacheLength; i++)
            {
                float trailProgress = MathHelper.Clamp((float)i / TrailCacheLength, 0f, 1f);
                float scale = defScale - (Scale.X * trailProgress * 5f);

                bool even = i % 2 == 0;
                Color baseColor;
                if (SecondaryColor.HasValue)
                {
                    baseColor = even ? Color : SecondaryColor.Value;
                }
                else
                {
                    baseColor = Color;
                }


                if (rainbow)
                {
                    float rainbowProgress = Utility.WrapProgress(trailProgress + CelestialDisco.CelestialStyleProgress);
                    baseColor = Utility.Rainbow(rainbowProgress);

                    #region Special code for Subtractive + Rainbow

                    specialRainbow = blendStateOverride == CustomBlendStates.Subtractive && SecondaryColor.HasValue;
                    if (specialRainbow && even)
                    {
                        baseColor = SecondaryColor.Value * (1f - trailProgress);
                        spriteBatch.End();
                        spriteBatch.Begin(blendStateOverride, state);
                    }
                    #endregion
                }

                Color color = scale < 0 ? baseColor * Progress * (1f - trailProgress) : baseColor * Progress;

                Vector2 position = scale < 0 ? OldPositions[i] + new Vector2(0, 55).RotatedBy(OldRotations[i]) : Vector2.Lerp(OldPositions[i], Center, Progress * (1f - trailProgress));
                spriteBatch.Draw(slash, position - screenPosition, null, color, OldRotations[i], slash.Size() / 2, scale, SpriteEffects.None, 0f);

                #region Special code for Midnight Rainbow
                if (specialRainbow && even)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(state);
                }
                #endregion
            }

            spriteBatch.Draw(slash, Center - screenPosition, null, Color * Progress, Rotation, slash.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Center - screenPosition, null, Color.Lerp(Color.White, Color, 0.75f).WithOpacity(0.5f) * Progress, defRotation, glow.Size() / 2, Utility.QuadraticEaseIn(Progress) * 0.7f, SpriteEffects.None, 0f);

            if (blendStateOverride is not null)
            {
                spriteBatch.End();
                spriteBatch.Begin(state);
            }

            return false;
        }

        public override void AI()
        {
            if (DashPlayer.DashTimer <= 0 || Player.dead || Player.CCed)
                Kill();

            Scale.X = MathHelper.Lerp(Scale.X * 0.8f, defScale, Progress);

            Lighting.AddLight(Player.Center, Color.ToVector3() * 2f * Utility.QuadraticEaseIn(Progress));

            if (collided)
                Color *= 0.9f;
            else
                collided = DashPlayer.CollidedWithNPC;

            if (Player.velocity.Length() > 0.5f)
            {
                if (!collided)
                    Rotation = Player.velocity.ToRotation() - MathHelper.PiOver2;

                Position = Player.Center + new Vector2(0, 15).RotatedBy(Rotation);
            }
        }
    }
}
