using Macrocosm.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Backgrounds {
    public class MoonUgBgStyle : ModUndergroundBackgroundStyle {
        public override void FillTextureArray(int[] textureSlots) {
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Terraria_Macrocosm/Backgrounds/MoonCavernUG0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Terraria_Macrocosm/Backgrounds/MoonCavernUG1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Terraria_Macrocosm/Backgrounds/MoonCavernUG2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Terraria_Macrocosm/Backgrounds/MoonCavernUG3");
		}
	}
}