using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Potions
{
    public class ZoningPotion : ModItem
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
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.UseSound = SoundID.Item3;
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.value = Item.buyPrice(silver: 50);
            Item.buffType = ModContent.BuffType<Buffs.Potions.ZoningBuff>();
            Item.buffTime = 5 * 60 * 60;
        }
    }
}