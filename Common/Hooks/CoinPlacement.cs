using Terraria;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework.Graphics;
using System;
using Macrocosm.Backgrounds.Moon;
using Macrocosm.Content.Items.Currency;

namespace Macrocosm.Common.Hooks
{
    public class CoinPlacement : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.UI.ItemSlot.PickItemMovementAction += MoonCoin_AllowCoinSlotPlacement;
        }

        public void Unload() { }

        private int MoonCoin_AllowCoinSlotPlacement(On.Terraria.UI.ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem)
        {
            if (context == 1 && checkItem.type == ModContent.ItemType<MoonCoin>())
            {
                return 0;
            }
            else
            {
                return orig(inv, context, slot, checkItem);
            }
        }




    }
}