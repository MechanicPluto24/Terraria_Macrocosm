using Macrocosm.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Backgrounds
{
    public class MoonUgBgStyle : ModUgBgStyle
	{
		public override bool ChooseBgStyle()
		{
            return Main.LocalPlayer.GetModPlayer<MacrocosmPlayer>().ZoneMoon;
        }

        public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = mod.GetBackgroundSlot("Backgrounds/MoonCavernUG0");
			textureSlots[1] = mod.GetBackgroundSlot("Backgrounds/MoonCavernUG1");
			textureSlots[2] = mod.GetBackgroundSlot("Backgrounds/MoonCavernUG2");
			textureSlots[3] = mod.GetBackgroundSlot("Backgrounds/MoonCavernUG3");
		}
	}
}