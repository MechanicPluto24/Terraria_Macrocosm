using Macrocosm.Common.Sets;
using Macrocosm.Content.Buffs.Potions;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Potions
{
    public class Medkit : ModItem
    {
        public static LocalizedText RestoreLifeText { get; private set; }

        public override void SetStaticDefaults()
        {
            RestoreLifeText = this.GetLocalization(nameof(RestoreLifeText));
            Item.ResearchUnlockCount = 20;

            ItemSets.PotionDelay[Type] = 30 * 60;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHealingPotion(newwidth: 26, newheight: 18, healingAmount: 100);
            Item.useStyle = ItemUseStyleID.EatFood; // ?
            Item.UseSound = SoundID.Item3;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.value = Item.buyPrice(gold: 1);
        }

        public virtual int HealthPerPeriod => 20;

        /// <summary> Healing period, in ticks. </summary>
        public virtual int HealPeriod => 30;

        /// <summary> Number of healing periods </summary>
        public virtual int HealPeriodNumber => 6;


        /// <summary> Duration of this medkit, in ticks. </summary>
        public int Duration => HealPeriodNumber * HealPeriod;

        /// <summary> Amount of life healed for the complete duration. </summary>
        public virtual int HealAmount => HealthPerPeriod * HealPeriodNumber;


        /// <summary> Hit cooldown for dimishing effect on hit </summary>
        public virtual int HitCooldown => 30;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Find the tooltip line that corresponds to 'Heals X life'
            // See https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html for a list of vanilla tooltip line names
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

            // Add "over Y seconds"
            if (line != null)
                line.Text = RestoreLifeText.Format(Language.GetText("CommonItemTooltip.RestoresLife").Format(HealAmount), Duration / 60);
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = 0;
        }

        public sealed override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<MedkitHigh>(), Duration);
            player.GetModPlayer<MedkitPlayer>().MedkitItemType = Type;
            return true;
        }

        public override void AddRecipes()
        {
        }
    }
}