using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Moon
{
    public class MoonSurfaceBgStyle : ModSurfaceBackgroundStyle
	{
		public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonSurfaceFar");
 
		public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonSurfaceMid");

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) 
		{
			//parallax = 0.9f;
			return BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonSurfaceNear");
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
	}
}