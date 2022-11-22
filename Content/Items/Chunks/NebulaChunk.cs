
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Chunks
{
	public class NebulaChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nebula Chunk");
			Tooltip.SetDefault("Consumable\nRight click to smash open!");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
		}

		override public void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(silver: 1);
			Item.rare = ItemRarityID.Purple;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			int itemType = ItemID.FragmentNebula;
			player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(20, 50));
			
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (Main.rand.NextBool(3))
			{
				int type = Main.rand.NextFromList<int>(71, 260);
				Vector2 rotVector1 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * MathF.Abs(MathF.Sin((float)Main.timeForVisualEffects)) * 1.1f;
				Dust lightDust = Main.dust[Dust.NewDustPerfect(Item.Center - rotVector1, type).dustIndex];
				lightDust.noGravity = false;
				lightDust.position = Item.Center - rotVector1 * Main.rand.Next(10, 11);
				lightDust.velocity = rotVector1.RotatedBy(MathHelper.PiOver2) * 1.2f;
				lightDust.scale = .8f;
				lightDust.customData = Item.Center;
			}
		}
	}
}
