using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.Utilities;
using Terraria;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Drawing.Sky;

namespace Macrocosm.Content.Skies.Ambience.Moon
{
    public class MoonMeteor : MacrocosmFadingSkyEntity
    {
        public MoonMeteor(Player player, FastRandom random) : base(player, random)
        {
            Depth = random.NextFloat() * 3f + 3f;
            Position.X = (player.Center.ToTileCoordinates().X + 600f * (random.NextFloat() * 2f - 1f)) * 16f;
            Position.Y = player.Center.ToTileCoordinates().Y - random.Next(40, 81);

            Effects = ((random.Next(2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Frame = new(1, 4) { CurrentRow = (byte)random.Next(4) };

            LifeTime = random.Next(20, 51) * 60;
            OpacityNormalizedTimeToFadeIn = 0.01f;
            OpacityNormalizedTimeToFadeOut = 0.99f;
            BrightnessLerper = 1f;
            FinalOpacityMultiplier = 1f;
            FramingSpeed = int.MaxValue;

            float speedX = Main.rand.Next(-400, 401);
            float speedY = Main.rand.Next(1200) + 8000;
            float mult = 8 / (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            speedX *= mult;
            speedY *= mult;
            Velocity = new(speedX, speedY);
        }

        public override void UpdateVelocity(int frameCount)
        {
            Rotation += (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) * 0.1f * Math.Sign(Velocity.X);
        }
    }
}
