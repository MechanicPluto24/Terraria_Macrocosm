using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class LightningTrail : VertexTrail
    {
        public Color Color;
        public float Width;

        public LightningTrail(Color color, float width)
        {
            Color = color;
            Width = width;
        }

        public override MiscShaderData TrailShader => new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseSaturation(Saturation)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeOutMask"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark6"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark6"));

        public override float Saturation => -2f;

        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(Color * 0.1f, Color, progressOnStrip);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return Width * (1f - MathF.Pow(progressOnStrip - 1, 2));
        }
    }
}
