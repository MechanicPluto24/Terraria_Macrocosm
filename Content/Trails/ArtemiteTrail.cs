using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class ArtemiteTrail : VertexTrail
    {
        public Color Color { get; set; } = new Color(130, 220, 199, 255) * 1.4f;

        public override MiscShaderData TrailShader => new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
            .UseProjectionMatrix(doUse: true)
            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Beam1"))
            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeInTrail"));


        public override Color TrailColors(float progressOnStrip)
        {
            return Color.Lerp(Color.Transparent, Color * 1f, progressOnStrip * 1 / progressOnStrip) * Opacity;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return 200;
        }
    }
}
