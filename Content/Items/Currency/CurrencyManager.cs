using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Currency
{
    public class CurrencyManager : ILoadable
    {
        public void Load(Mod mod)
        {
            LoadCurrencies();
        }
        public void Unload() { }

        public static int MoonStone { get; set; }

        public static void LoadCurrencies()
        {
            MoonStone = CustomCurrencyManager.RegisterCurrency(new MoonstoneData(ModContent.ItemType<Moonstone>(), 999999L));
        }
    }
}
