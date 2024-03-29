﻿using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Sands
{
	public class SilicaPearlsandProjectile : ModProjectile
	{
		protected bool falling = true;
		protected int tileType;
		protected int dustType;
		protected int itemType;


		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.ForcePlateDetection[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.knockBack = 6f;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.penetrate = -1;

			//Projectile.aiStyle = ProjAIStyleID.FallingTile;

			//Set the tile type to ExampleSand
			tileType = ModContent.TileType<Tiles.Blocks.SilicaPearlsand>();
			dustType = ModContent.DustType<SilicaPearlsandDust>();
			itemType = ModContent.ItemType<Items.Placeable.Blocks.SilicaPearlsand>();
		}

		public override void AI()
		{
			if (Main.rand.NextBool(5))
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
				Main.dust[dust].velocity.X *= 0.4f;
			}

			Utility.AIFallingBlock(Projectile, falling);
		}

		public override void OnKill(int timeLeft)
		   => Utility.FallingBlockCreateTile(Projectile, tileType, itemType);

		public override bool? CanDamage() => Projectile.localAI[1] != -1f;
	}
}