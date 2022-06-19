using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Weapons
{
	public class Cruithne3753 : ModItem
	{
		public override void SetStaticDefaults() 
		{
            DisplayName.SetDefault("Cruithne 3753");
			Tooltip.SetDefault("Two different firing modes");
		}

		public override void SetDefaults()
		{
			Item.damage = 80;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 52;
			Item.height = 16;
			Item.useTime = 34;
			Item.useAnimation = 34;
			Item.useStyle = ItemUseStyleID.Shoot; // was HoldingOut
			Item.noMelee = true;
			Item.knockBack = 8f;
			Item.value = 10000;
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item38;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 68;
				Item.useAnimation = 68;
				Item.shoot =  ProjectileType<Content.Projectiles.Friendly.Weapons.CruithneBlackSlug>();
			}
			else
			{
				Item.useTime = 34;
				Item.useAnimation = 34;
				Item.shoot =  ProjectileType<Content.Projectiles.Friendly.Weapons.CruithneGreenSlug>();
			}
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
		{
			int numberProjectiles = player.altFunctionUse == 2 ? 12 : 6;
			int degree = player.altFunctionUse == 2 ? 20 : 10;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 muzzleOffset = Vector2.Normalize(velocity) * 12f;
				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
					position += muzzleOffset;
				Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(degree));
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, Item.shoot, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-12, 0);
		}
	}
}
