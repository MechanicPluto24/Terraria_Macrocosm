using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
	public class SeleniteBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Selenite Diffuser");
 			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

		}
		public override void SetDefaults()
		{
			Item.damage = 320;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 20;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Arrow;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(6, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// the effect could look like this
			//for (int i = 0; i < 10; i++)
			//	Projectile.NewProjectile(null, position + new Vector2(- 5 + i * Math.Sign(velocity.X) * 2, - 5 + i * Math.Sign(velocity.Y) * 2f), velocity * 0.33f, ProjectileID.ChargedBlasterOrb, 0, 0);
			//
			return true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			//	type = ProjectileID.ShadowFlameArrow;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<LuminiteCrystal>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
 		}
	}
}
