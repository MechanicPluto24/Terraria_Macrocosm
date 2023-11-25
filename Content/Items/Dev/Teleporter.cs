using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Items.Global;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    class Teleporter : ModItem, IDevItem
    {
        public override void SetStaticDefaults()
        {
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
                    MacrocosmSubworld.Travel("Moon");
                else
                    MacrocosmSubworld.Travel("Earth");
            }

            return true;
        }
    }
}
