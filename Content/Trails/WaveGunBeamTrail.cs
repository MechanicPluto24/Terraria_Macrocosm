using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class WaveGunBeamTrail : VertexTrail
    {
        public Color Color;

        public WaveGunBeamTrail(Color color)
        {
            Color = color;
        }

        public override MiscShaderData TrailShader => new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseSaturation(Saturation)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Spark6"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Spark6"));

        public override float Saturation => -6f;

        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(Color.Transparent, Color, 1f - progressOnStrip);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return 80f * (1f - MathF.Pow(progressOnStrip - 1, 2));
        }
    }
}
