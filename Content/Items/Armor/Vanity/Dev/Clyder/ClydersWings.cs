using Macrocosm.Common.Sets;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Vanity.Dev.Clyder
{
    [AutoloadEquip(EquipType.Wings)]
    public class ClydersWings : ModItem
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
            Item.accessory = true;

        }

        public override void AddRecipes()
        {
        }
    }
}