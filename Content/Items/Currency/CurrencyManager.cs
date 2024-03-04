using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Currency
{
	public class CurrencyManager
	{
        public static int MoonStone { get; set; }

        public static void Load()
		{
            MoonStone = CustomCurrencyManager.RegisterCurrency(new MoonstoneData
			(
				ModContent.ItemType<Moonstone>(), 999999L, "Mods.Macrocosm.Items.Moonstone.DisplayName"
			));
        }

        public static void Unload() 
		{
		}


	}
}
