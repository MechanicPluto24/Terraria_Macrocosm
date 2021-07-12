using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Currency
{
	public class CurrencyManager
	{
		public static int UnuCredit;
		public static void LoadCurrencies()
		{
			UnuCredit = CustomCurrencyManager.RegisterCurrency(new MoonCoinData(ModContent.ItemType<MoonCoin>(), 999999L));
		}
	}
}
