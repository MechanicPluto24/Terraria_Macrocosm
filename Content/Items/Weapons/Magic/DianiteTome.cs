using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
	public class DianiteTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 125;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 30;
			Item.width = 80;
			Item.height = 80;
			Item.useTime = 12;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 5;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DianiteTomeProjectileSmall>();
			Item.shootSpeed = 16f;
			Item.tileBoost = 50;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient<LuminiteCrystal>();
			recipe.AddIngredient<DianiteBar>(12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
		{
			int numberProjectiles = 4 + Main.rand.Next(2);  //This defines how many projectiles to shoot
			for (int index = 0; index < numberProjectiles; ++index)
			{

				bool bigProjectile = Main.rand.NextBool(4);
				int projType = bigProjectile ? ModContent.ProjectileType<DianiteTomeProjectile>() : type;
				damage = (int)(damage * (bigProjectile ? 1.4f : 1f));

				Vector2 vector2_1 = new Vector2((float)(player.position.X + player.width * 0.5 + Main.rand.Next(201) * -player.direction + (Main.mouseX + (double)Main.screenPosition.X - player.position.X)), (float)(player.position.Y + player.height * 0.5 - 600.0));   //this defines the Projectile width, direction and position
				vector2_1.X = (float)((vector2_1.X + (double)player.Center.X) / 2.0) + Main.rand.Next(-200, 201);
				vector2_1.Y -= 100 * index;
				float num12 = Main.mouseX + Main.screenPosition.X - vector2_1.X;
				float num13 = Main.mouseY + Main.screenPosition.Y - vector2_1.Y;
				if ((double)num13 < 0.0) num13 *= -1f;
				if ((double)num13 < 20.0) num13 = 20f;
				float num14 = (float)Math.Sqrt((double)num12 * (double)num12 + (double)num13 * (double)num13);
				float num15 = Item.shootSpeed / num14;
				float num16 = num12 * num15;
				float num17 = num13 * num15;
				float SpeedX = num16 + Main.rand.Next(-40, 41) * 0.02f;  //this defines the Projectile X position speed and randomness
				float SpeedY = num17 + Main.rand.Next(-40, 41) * 0.02f;  //this defines the Projectile Y position speed and randomness
				Projectile.NewProjectile(source, vector2_1.X, vector2_1.Y, SpeedX, SpeedY, projType, damage, knockBack, Main.myPlayer, 0.0f, Main.rand.Next(5));
			}
			return false;
		}
	}
}