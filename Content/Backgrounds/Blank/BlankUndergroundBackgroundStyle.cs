using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Blank
{
    public class BlankUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i < textureSlots.Length; i++)
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Macrocosm/Content/Backgrounds/Blank/Blank");
        }
    }
}