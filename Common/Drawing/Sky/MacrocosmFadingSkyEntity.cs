using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing.Sky
{
    public class MacrocosmFadingSkyEntity : MacrocosmSkyEntity
    {
        protected int LifeTime;
        protected Vector2 Velocity;
        protected int FramingSpeed;
        protected int TimeEntitySpawnedIn;
        protected float Opacity;
        protected float BrightnessLerper;
        protected float FinalOpacityMultiplier;
        protected float OpacityNormalizedTimeToFadeIn;
        protected float OpacityNormalizedTimeToFadeOut;
        protected int FrameOffset;

        protected MacrocosmFadingSkyEntity(Player player, FastRandom random) : base(player, random)
        {
            Opacity = 0f;
            TimeEntitySpawnedIn = -1;
            BrightnessLerper = 0f;
            FinalOpacityMultiplier = 1f;
            OpacityNormalizedTimeToFadeIn = 0.1f;
            OpacityNormalizedTimeToFadeOut = 0.9f;
        }

        public override void Update(int frameCount)
        {
            if (!IsMovementDone(frameCount))
            {
                UpdateOpacity(frameCount);
                if ((frameCount + FrameOffset) % FramingSpeed == 0)
                    NextFrame();

                UpdateVelocity(frameCount);
                Position += Velocity;
            }
        }

        public virtual void UpdateVelocity(int frameCount)
        {
        }

        private void UpdateOpacity(int frameCount)
        {
            int num = frameCount - TimeEntitySpawnedIn;
            if (num >= LifeTime * OpacityNormalizedTimeToFadeOut)
                Opacity = Terraria.Utils.GetLerpValue(LifeTime, LifeTime * OpacityNormalizedTimeToFadeOut, num, clamped: true);
            else
                Opacity = Terraria.Utils.GetLerpValue(0f, LifeTime * OpacityNormalizedTimeToFadeIn, num, clamped: true);
        }

        private bool IsMovementDone(int frameCount)
        {
            if (TimeEntitySpawnedIn == -1)
                TimeEntitySpawnedIn = frameCount;

            if (frameCount - TimeEntitySpawnedIn >= LifeTime)
            {
                IsActive = false;
                return true;
            }

            return false;
        }

        public override Color GetColor(Color backgroundColor) => Color.Lerp(backgroundColor, Color.White, BrightnessLerper) * Opacity * FinalOpacityMultiplier;

        public void StartFadingOut(int currentFrameCount)
        {
            int num = (int)(LifeTime * OpacityNormalizedTimeToFadeOut);
            int num2 = currentFrameCount - num;
            if (num2 < TimeEntitySpawnedIn)
                TimeEntitySpawnedIn = num2;
        }

        public override Vector2 GetDrawPosition() => Position;
    }
}
