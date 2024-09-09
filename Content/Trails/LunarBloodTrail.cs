using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.Trails
{
    public class LunarBloodTrail : VertexTrail
    {
        public  Color BloodColour1{ get; set; }=new Color(94, 229, 163, 0);
        public  Color BloodColour2{ get; set; }=new Color(213, 155, 148, 0);
        public override MiscShaderData TrailShader => new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
                        .UseProjectionMatrix(doUse: true)
                        .UseSaturation(Saturation)
                        .UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
                        .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "FadeOutMask"))
                        .UseImage2("Images/Extra_193");

      
        public override float Saturation => -2f;
        public override Color TrailColors(float progressOnStrip)
        {
             float lerp = Utility.InverseLerp(0, 0.01f, progressOnStrip);
             Color result = Color.Lerp(Color.Lerp(Color.Black.WithAlpha(0), BloodColour1, lerp), BloodColour2, progressOnStrip);
            return result;
        }

        public override float TrailWidths(float progressOnStrip)
        {
            return MathHelper.Lerp(5, 5, progressOnStrip);
        }
    }
}
