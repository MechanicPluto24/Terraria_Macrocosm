using Macrocosm.Content.Items.Currency;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class CurrencySystem : ILoadable
    {
        public static int MoonStone { get; set; }

        public void Load(Mod mod)
        {
            MoonStone = CustomCurrencyManager.RegisterCurrency(new MoonstoneData
            (
                ModContent.ItemType<Moonstone>(), 999999L, "Mods.Macrocosm.Items.Moonstone.DisplayName"
            ));
        }

        public void Unload()
        {
        }
    }
}
