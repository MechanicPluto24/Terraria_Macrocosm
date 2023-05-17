using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Selenite
{
    [AutoloadEquip(EquipType.Head)]
    public class SeleniteHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.defense = 26;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<SeleniteHeadgear>() && body.type == ModContent.ItemType<SeleniteBreastplate>() && legs.type == ModContent.ItemType<SeleniteLeggings>();
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
            recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}