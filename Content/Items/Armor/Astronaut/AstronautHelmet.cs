using Macrocosm.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Astronaut
{
    [AutoloadEquip(EquipType.Head)]
    public class AstronautHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Purple;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<AstronautHelmet>() && body.type == ModContent.ItemType<AstronautSuit>() && legs.type == ModContent.ItemType<AstronautLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection = SpaceProtection.Tier1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}