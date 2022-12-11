
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.MeteorChunks
{
	public class StardustChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardust Chunk");
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
			int itemType = ItemID.FragmentStardust;
			player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(20, 50));

		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, Main.rand.NextFromList(DustID.YellowStarDust, DustID.DungeonWater));
				dust.velocity.X = Main.rand.NextFloat(-0.2f, 0.2f);
				dust.velocity.Y = -0.8f;
				dust.scale = 1.1f;
 				dust.noGravity = true;
			}
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			ParticleOrchestraSettings settings;

			if(Main.rand.NextBool(4))
			{
				settings = new()
				{
					PositionInWorld = Item.Center + new Vector2(Item.width, Item.height).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(),
					MovementVector = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 2
				};

				ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
			}
		}
	}
}

