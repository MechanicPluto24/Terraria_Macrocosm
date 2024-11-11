using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Sets;

namespace Macrocosm.Content.Items.Armor.Vanity.Dev.Clyder
{
    [AutoloadEquip(EquipType.Head)]
    public class ClydersHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.UnobtainableItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<DevRarity>();
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
        }
    }
}