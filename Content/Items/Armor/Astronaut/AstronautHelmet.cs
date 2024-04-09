using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Materials.Refined;
using Macrocosm.Content.Players;
using Macrocosm.Content.Tiles.Crafting;
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

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<AstronautHelmet>() && body.type == ModContent.ItemType<AstronautSuit>() && legs.type == ModContent.ItemType<AstronautLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SpacesuitFabric>(20)
            .AddIngredient<LexanGlass>(5)
            .AddTile<IndustrialLoom>()
            .Register();
        }
    }
}