using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev;

class Teleporter : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemSets.DeveloperItem[Type] = true;
    }
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 36;
        Item.rare = ModContent.RarityType<DevRarity>();
        Item.value = 100000;
        Item.maxStack = 1;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.UseSound = SoundID.Item6;
    }
    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            if (!SubworldSystem.AnyActive<Macrocosm>())
                SubworldTravelPlayer.Travel(Moon.Instance.ID);
            else
                SubworldTravelPlayer.Travel(Earth.ID);
        }

        return true;
    }
}
