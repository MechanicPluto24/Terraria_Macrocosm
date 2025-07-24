using Macrocosm.Common.Sets;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Vanity.Dev.Clyder;

[AutoloadEquip(EquipType.Body)]
public class ClydersRobe : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemSets.UnobtainableItem[Type] = true;
    }
    public override void Load()
    {
        if (Main.netMode != NetmodeID.Server)
        {
            EquipLoader.AddEquipTexture(Mod, "Macrocosm/Content/Items/Armor/Vanity/Dev/Clyder/ClydersRobe_Legs", EquipType.Legs, this);
        }
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ModContent.RarityType<DevRarity>();
        Item.vanity = true;
    }
    public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
    {
        robes = true;
        equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
    }

    public override void AddRecipes()
    {
    }
}