// using Macrocosm.Tiles;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Chandrium
{
    [AutoloadEquip(EquipType.Body)]
    public class ChandriumBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.IncludedCapeBack[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = capeSlot;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = capeSlot;
        }

        private int capeSlot;

        public override void Load()
        {
            capeSlot = EquipLoader.AddEquipTexture(Mod, "Macrocosm/Content/Items/Armor/Chandrium/ChandriumCape", EquipType.Back, name: "ChandriumCape");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.whipRangeMultiplier += 0.15f;
            player.GetAttackSpeed<SummonDamageClass>() += 0.15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<ChandriumBar>(), 16);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}