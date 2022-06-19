using System;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Materials;

namespace Macrocosm.Content.Items.Weapons
{
	public class DianiteTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			
		}

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 30;
			Item.width = 80;
			Item.height = 80;
			Item.useTime = 12;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true; //so the item's animation doesn't do damage
			Item.knockBack = 5;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DianiteTomeProjectile>();
			Item.shootSpeed = 16f;
			Item.tileBoost = 50;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient<LuminiteCrystal>();
			recipe.AddIngredient<DianiteBar>(12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
		{
			int numberProjectiles = 6 + Main.rand.Next(2);  //This defines how many projectiles to shot
			for (int index = 0; index < numberProjectiles; ++index)
			{
				Vector2 vector2_1 = new Vector2((float)((double)player.position.X + (double)player.width * 0.5 + (double)(Main.rand.Next(201) * -player.direction) + ((double)Main.mouseX + (double)Main.screenPosition.X - (double)player.position.X)), (float)((double)player.position.Y + (double)player.height * 0.5 - 600.0));   //this defines the Projectile width, direction and position
				vector2_1.X = (float)(((double)vector2_1.X + (double)player.Center.X) / 2.0) + (float)Main.rand.Next(-200, 201);
				vector2_1.Y -= (float)(100 * index);
				float num12 = (float)Main.mouseX + Main.screenPosition.X - vector2_1.X;
				float num13 = (float)Main.mouseY + Main.screenPosition.Y - vector2_1.Y;
				if ((double)num13 < 0.0) num13 *= -1f;
				if ((double)num13 < 20.0) num13 = 20f;
				float num14 = (float)Math.Sqrt((double)num12 * (double)num12 + (double)num13 * (double)num13);
				float num15 = Item.shootSpeed / num14;
				float num16 = num12 * num15;
				float num17 = num13 * num15;
				float SpeedX = num16 + (float)Main.rand.Next(-40, 41) * 0.02f;  //this defines the Projectile X position speed and randomnes
				float SpeedY = num17 + (float)Main.rand.Next(-40, 41) * 0.02f;  //this defines the Projectile Y position speed and randomnes
				Projectile.NewProjectile(source, vector2_1.X, vector2_1.Y, SpeedX, SpeedY, type, damage, knockBack, Main.myPlayer, 0.0f, (float)Main.rand.Next(5));
			}
			return false;
		}
	}
}