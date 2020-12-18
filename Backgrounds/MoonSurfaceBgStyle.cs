using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Backgrounds
{
	public class MoonSurfaceBgStyle : ModSurfaceBgStyle
	{
		public override bool ChooseBgStyle()
		{
			return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<MacrocosmPlayer>().ZoneMoon;
		}
        // Use this to keep far Backgrounds like the mountains.
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
		{
			return mod.GetBackgroundSlot("Backgrounds/MoonSurfaceFar");
		}

        public override int ChooseMiddleTexture()
        {
            return mod.GetBackgroundSlot("Backgrounds/MoonSurfaceMid");
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return mod.GetBackgroundSlot("Backgrounds/MoonSurfaceClose");
		}
	}
}