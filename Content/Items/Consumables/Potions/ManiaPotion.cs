using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Potions
{
    public class ManiaPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useTurn = true;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.UseSound = SoundID.Item3;
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.value = Item.buyPrice(silver: 50);
            Item.buffType = ModContent.BuffType<Buffs.Potions.ManiaBuff>();
            Item.buffTime = 4 * 60 * 60;
        }
    }
}