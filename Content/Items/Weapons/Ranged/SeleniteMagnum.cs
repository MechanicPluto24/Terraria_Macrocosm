using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Common.Bases;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
	internal class SeleniteMagnum : GunHeldProjectileItem
	{
		public override GunHeldProjectileData GunHeldProjectileData => new()
		{
			GunBarrelPosition = new(18, 7),
			CenterYOffset = 4,
			MuzzleOffset = 20,
			Recoil = (7, 1.1f)
		};

        public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaultsHeldProjectile()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 34;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4f;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.shoot = ProjectileID.PurificationPowder;  
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item38;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position += Vector2.Normalize(velocity) * 30;
			type = ModContent.ProjectileType<SeleniteMagnumProjectile>();
 		}

		public override Vector2? HoldoutOffset() => new Vector2(4, 0);

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
