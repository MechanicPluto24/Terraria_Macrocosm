using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
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
			Tooltip.SetDefault("Shoots an arrow that slows to a stop\nOnce this arrow slows down all the way, it will shoot a spread o");
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
