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
    public class FlamingMeteorTrail : VertexTrail
    {
        private static MiscShaderData shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                            .UseProjectionMatrix(doUse: true)
                            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
                            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"))
                            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"));



        public override MiscShaderData TrailShader => shader;

        public override Color TrailColors(float progressOnStrip)
        {
            shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                            .UseProjectionMatrix(doUse: true)
                            .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
                            .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FlamingTrail1"))
                            .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FlamingTrail2"));

            return Color.Lerp(new Color(229, 128, 36, 255), new Color(255, 196, 27, 127), progressOnStrip * progressOnStrip);
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(30,5, progressOnStrip);
        }
    }
}
