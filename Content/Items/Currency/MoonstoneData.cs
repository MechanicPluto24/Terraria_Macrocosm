using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace Macrocosm.Content.Items.Currency
{
    public class MoonstoneData : CustomCurrencySingleCoin
    {
        public MoonstoneData(int coinItemID, long currencyCap, string currencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = currencyTextKey;
            CurrencyTextColor = Color.DarkGray;
        }
    }
}