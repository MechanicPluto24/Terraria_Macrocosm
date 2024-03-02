using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shoes)]
	public class OsmiumBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
		}
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.gravity *= 2f;

            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.AccDashDownwards = true;
            dashPlayer.AccDashSpeedY = 14f;
            dashPlayer.AccDashAfterImage = false;
        }
	}
}