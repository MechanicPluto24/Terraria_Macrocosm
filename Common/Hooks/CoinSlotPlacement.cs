using Macrocosm.Content.Items.Currency;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class CoinSlotPlacement : ILoadable
    {
        public void Load(Mod mod)
        {
            Terraria.UI.On_ItemSlot.PickItemMovementAction += MoonCoin_AllowCoinSlotPlacement;
        }

        public void Unload()
        {
            Terraria.UI.On_ItemSlot.PickItemMovementAction -= MoonCoin_AllowCoinSlotPlacement;
        }

        private int MoonCoin_AllowCoinSlotPlacement(Terraria.UI.On_ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem)
        {
            if (context == 1 && checkItem.type == ModContent.ItemType<Moonstone>())
                return 0;
            else
                return orig(inv, context, slot, checkItem);
        }
    }
}