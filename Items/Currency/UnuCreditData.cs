using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;

namespace Macrocosm.Items.Currency
{
    public class UnuCreditData : CustomCurrencySingleCoin
    {
        public Color UnuCredittextcolor = Color.ForestGreen; //this defines the Custom Currency Buy Price color when shown in the shoop

        public UnuCreditData(int coinItemID, long currencyCap) : base(coinItemID, currencyCap)
        {

        }

        public override void GetPriceText(string[] lines, ref int currentLine, int price)
        {
            Color color = UnuCredittextcolor * ((float)Main.mouseTextColor / 255f);
            lines[currentLine++] = string.Format("[c/{0:X2}{1:X2}{2:X2}:{3} {4} {5}]", new object[]
                {
                    color.R,
                    color.G,
                    color.B,
                    Lang.tip[50],
                    price,
                    "Unu Credit" //this is the Currency name when shown in the shop
                });
        }
    }
}