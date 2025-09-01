using Macrocosm.Content.Items.Currency;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class CurrencySystem
    {
        public static int MoonStone { get; set; }

        public static void Load()
        {
            MoonStone = CustomCurrencyManager.RegisterCurrency(new MoonstoneData
            (
                ModContent.ItemType<Moonstone>(), 999L, "Mods.Macrocosm.Items.Moonstone.DisplayName"
            ));
        }
    }
}
