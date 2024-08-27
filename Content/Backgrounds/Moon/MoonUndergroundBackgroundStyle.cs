using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Moon
{
    public class MoonUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
    {
        private const string Path = "Macrocosm/Content/Backgrounds/Moon/";

        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Path + "MoonCavernUG0");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Path + "MoonCavernUG1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Path + "MoonCavernUG2");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Path + "MoonCavernUG3");
        }
    }
}