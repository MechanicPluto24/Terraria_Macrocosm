using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
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
            Item.defense = 6;
        }
        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.GetModPlayer<MacrocosmPlayer>();
            player.GetDamage<RangedDamageClass>() += 0.1f;
            modPlayer.ChanceToNotConsumeAmmo += 0.15f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<SeleniteHeadgear>() && body.type == ModContent.ItemType<SeleniteBreastplate>() && legs.type == ModContent.ItemType<SeleniteLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection = SpaceProtection.Tier1;
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