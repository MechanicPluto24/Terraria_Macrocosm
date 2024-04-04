// using Macrocosm.Tiles;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
using Macrocosm.Content.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Astronaut
{
    [AutoloadEquip(EquipType.Body)]
    public class AstronautSuit : ModItem
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
            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SpacesuitFabric>(20)
            .AddTile<IndustrialLoom>()
            .Register();
        }
    }
}