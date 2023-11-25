using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
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
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection = SpaceProtection.Tier1;
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