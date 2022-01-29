using Microsoft.Xna.Framework;
using System;
using Terraria;
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
			item.damage = 80;
			item.ranged = true;
			item.width = 52;
			item.height = 16;
			item.useTime = 34;
			item.useAnimation = 34;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 8f;
			item.value = 10000;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item38;
			item.autoReuse = true;
			item.shoot = ProjectileID.PurificationPowder;
			item.shootSpeed = 20f;
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useTime = 68;
				item.useAnimation = 68;
				item.shoot =  ProjectileType<Content.Projectiles.Friendly.Weapons.CruithneBlackSlug>();
			}
			else
			{
				item.useTime = 34;
				item.useAnimation = 34;
				item.shoot =  ProjectileType<Content.Projectiles.Friendly.Weapons.CruithneGreenSlug>();
			}
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numberProjectiles = player.altFunctionUse == 2 ? 12 : 6;
			int degree = player.altFunctionUse == 2 ? 20 : 10;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 12f;
				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
					position += muzzleOffset;
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(degree));
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, item.shoot, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-12, 0);
		}
	}
}
