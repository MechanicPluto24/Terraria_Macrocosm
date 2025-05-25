using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Potions
{
    public class SafeguardPotion : ModItem
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
            Item.buffTime = 3 * 60 * 60;
        }

        public override bool CanUseItem(Player player) => !player.HasBuff(ModContent.BuffType<Buffs.Potions.SafeguardPotionCooldown>());

        public sealed override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Buffs.Potions.SafeguardPotionBuff>(), Item.buffTime);
            player.AddBuff(ModContent.BuffType<Buffs.Potions.SafeguardPotionCooldown>(), 5 * 60 * 60);
            return true;
        }
    }
}