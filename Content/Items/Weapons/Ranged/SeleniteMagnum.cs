using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Macrocosm.Content.Items.Materials;
 
namespace Macrocosm.Content.Items.Weapons.Ranged
{
	public class SeleniteMagnum : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
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
			Item.rare = ItemRarityID.Purple;
			Item.shoot = ProjectileID.PurificationPowder;  
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item38;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
 			position += new Vector2(0, -3); 
 		}

		public override Vector2? HoldoutOffset() => new Vector2(4, 0);

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
