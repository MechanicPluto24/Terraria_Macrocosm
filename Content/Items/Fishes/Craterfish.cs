using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Rarities;

namespace Macrocosm.Content.Items.Fishes
{
    public class Craterfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
