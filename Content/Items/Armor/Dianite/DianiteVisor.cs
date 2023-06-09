using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Dianite
{
    [AutoloadEquip(EquipType.Head)]
    public class DianiteVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases maximum mana by 50"
                             + "\n10% increased magic damage");
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 60;
            player.GetDamage<MagicDamageClass>() += 0.1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<DianiteVisor>() && body.type == ModContent.ItemType<DianiteBreastplate>() && legs.type == ModContent.ItemType<DianiteLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.Macrocosm().AccMoonArmor = true;
            player.setBonus = "Pressurized spacesuit allows for safe exploration of other celestial bodies"
                            + "\nTier 1 extraterrestrial protection"
                            + "\nVastly extends underwater breathing time";
            player.buffImmune[ModContent.BuffType<SuitBreach>()] = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<DianiteBar>(), 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}