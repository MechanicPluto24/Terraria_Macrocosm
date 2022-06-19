using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons
{
	public class SeleniteBow : ModItem
	{
		public override void SetStaticDefaults() 
		{
            DisplayName.SetDefault("Selenite Diffuser");
			Tooltip.SetDefault("Shoots an arrow that slows to a stop\nOnce this arrow slows down all the way, it will shoot a spread o");
		}
        public override void SetDefaults()
        {
            // Kinda mentally bored rn so note
            // TODO: Rework this failiure of a bow - Ryan
            // Its october and im still bored - Ryan
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
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item2;
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
            Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient(ModContent.ItemType<LuminiteCrystal>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 12);
			recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
            // FIXME: Definitely not finalized, someone check with this and see if the values are appropriate
		}
	}
}
