using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
	public class Cruithne3753 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cruithne-3753");
			Tooltip.SetDefault("Two different firing modes");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 80;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 52;
			Item.height = 16;
			Item.useTime = 34;
			Item.useAnimation = 34;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 8f;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<MoonRarityT1>();
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
 				Item.shoot = ModContent.ProjectileType<DeliriumShell>();
			}
			else
			{
				Item.useTime = 34;
				Item.useAnimation = 34;
 				Item.shoot = ModContent.ProjectileType<CruithneGreenSlug>();
			}
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
		{
			int numberProjectiles = player.altFunctionUse == 2 ? 1 : 6;
			int degree = player.altFunctionUse == 2 ? 0 : 12;
 
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

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (player.altFunctionUse == 2)                                                                                                                                                                                                  
				velocity *= 0.2f;
 		}

		public override Vector2? HoldoutOffset() => new Vector2(-12, 0);
	}
}
