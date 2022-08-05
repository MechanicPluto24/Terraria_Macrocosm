using Terraria.ModLoader;

namespace Macrocosm.Backgrounds.Moon
{
	public class MoonUgBgStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Backgrounds/Moon/MoonCavernUG0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Backgrounds/Moon/MoonCavernUG1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Backgrounds/Moon/MoonCavernUG2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Backgrounds/Moon/MoonCavernUG3");
		}
	}
}