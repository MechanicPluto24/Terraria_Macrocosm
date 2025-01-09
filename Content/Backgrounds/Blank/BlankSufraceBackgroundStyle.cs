using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Blank
{
    public class BlankSufraceBackgroundStyle : ModSurfaceBackgroundStyle
    {
        public override int ChooseFarTexture() => -1;
        public override int ChooseMiddleTexture() => -1;
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => -1;

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            return false;
        }

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
        }
    }
}