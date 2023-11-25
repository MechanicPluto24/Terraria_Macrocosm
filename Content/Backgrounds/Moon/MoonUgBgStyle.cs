using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Moon
{
    public class MoonUgBgStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonCavernUG0");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonCavernUG1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonCavernUG2");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Moon/MoonCavernUG3");
        }
    }
}