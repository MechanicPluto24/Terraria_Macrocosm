using Macrocosm.Common.Drawing.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Utilities;

namespace Macrocosm.Content.Skies.Ambience.Moon
{
    public class MoonMeteor : MacrocosmFadingSkyEntity
    {
        public MoonMeteor(Player player, FastRandom random) : base(player, random)
        {
            Depth = random.NextFloat() * 2f + 2.7f;

            Position.X = (player.Center.ToTileCoordinates().X + 600f * (random.NextFloat() * 2f - 1f)) * 16f;
            Position.Y = player.Center.ToTileCoordinates().Y - random.Next(40, 81);

            Effects = ((random.Next(2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Frame = new(1, 4) { CurrentRow = (byte)random.Next(4) };

            LifeTime = random.Next(4, 7) * 240;
            OpacityNormalizedTimeToFadeIn = 0.01f;
            OpacityNormalizedTimeToFadeOut = 0.99f;
            BrightnessLerper = 1f;
            FinalOpacityMultiplier = 1f;
            FramingSpeed = int.MaxValue;

            float speedX = Main.rand.Next(-50, 51);
            float speedY = Main.rand.Next(80) + 20;
            float mult = 8 / (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            speedX *= mult;
            speedY *= mult;
            Velocity = new Vector2(speedX, speedY);
        }

        public override void UpdateVelocity(int frameCount)
        {
            Rotation += (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) * 0.005f * Math.Sign(Velocity.X);
        }
    }
}
