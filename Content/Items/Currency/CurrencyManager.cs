using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Currency {
    public class CurrencyManager : ILoadable {
        public void Load(Mod mod) {
            LoadCurrencies();
        }
        public void Unload() { }

        public static int UnuCredit { get; set; }
        public static void LoadCurrencies() {
            UnuCredit = CustomCurrencyManager.RegisterCurrency(new MoonCoinData(ModContent.ItemType<MoonCoin>(), 999999L));
        }
    }
}
