using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Items.Global;

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
			if(player.whoAmI == Main.myPlayer)
			{
				if (!SubworldSystem.AnyActive<Macrocosm>())
					SubworldSystem.Enter("Macrocosm/Moon");
				else
					SubworldSystem.Exit();
			}
			
 			return true;
		}
	}
}
