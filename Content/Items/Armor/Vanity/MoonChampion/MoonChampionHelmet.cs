using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Vanity.MoonChampion
{
    [AutoloadEquip(EquipType.Head)]
    public class MoonChampionHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 200);
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
        }
    }
}