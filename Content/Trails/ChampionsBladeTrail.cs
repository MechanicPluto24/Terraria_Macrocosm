using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class ChampionsBladeTrail : VertexTrail
    {
        public Color Color { get; set; } = new Color(30, 255, 105, 255) * 1.4f;
        public float WidthMult;

        public override MiscShaderData TrailShader => new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "RocketExhaustTrail1"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeInTrail"));


        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(Color.Transparent, Color * 1f, progressOnStrip * 1 / progressOnStrip) * Opacity;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return 150 * WidthMult;
        }
    }
}
