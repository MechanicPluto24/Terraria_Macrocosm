using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class FlamingMeteorTrail : VertexTrail
    {
        public Color Color;
        public int Width;

        public FlamingMeteorTrail(Color color)
        {
            Color = color;
            Width = 40;
        }

        public FlamingMeteorTrail(Color color, int width)
        {
            Color = color;
            Width = width;
        }

        public override MiscShaderData TrailShader => new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseSaturation(Saturation)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutTrail"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail1"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail2"));

        public override float Saturation => -2f;

        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(Color * 0.2f, Color * 1f, progressOnStrip * 2f / progressOnStrip);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return Width;
        }
    }
}
