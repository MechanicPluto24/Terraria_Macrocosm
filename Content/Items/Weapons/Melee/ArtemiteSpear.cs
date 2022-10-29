using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
	public class ArtemiteSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			//Tooltip.SetDefault("");

			ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
 			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.value = Item.sellPrice(gold: 1);  
			Item.useStyle = ItemUseStyleID.Shoot; 
			Item.useAnimation = 18;  
			Item.useTime = 24;  
			Item.UseSound = SoundID.Item71;  
			Item.autoReuse = true;  
			Item.damage = 225;
			Item.knockBack = 6.5f;
			Item.noUseGraphic = true;  
			Item.DamageType = DamageClass.MeleeNoSpeed; 
			Item.noMelee = true;  
			Item.shootSpeed = 1f;  
			Item.shoot = ModContent.ProjectileType<ArtemiteSpearProjectile>();  
		}

		public override bool CanUseItem(Player player)
		{
			// Ensures no more than one spear can be thrown out, use this when using autoReuse
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override bool? UseItem(Player player)
		{
			// Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
			if (!Main.dedServ && Item.UseSound.HasValue)
			{
				SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
			}
			return null;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient<LuminiteCrystal>();
			recipe.AddIngredient<ArtemiteBar>(12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
