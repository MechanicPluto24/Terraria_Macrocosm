using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
            item.damage = 320;
            item.ranged = true;
            item.width = 40;
            item.height = 20;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item2;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(6, 0);
        }
        public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<LuminiteCrystal>(), 1);
			recipe.AddIngredient(ItemType<SeleniteBar>(), 12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
            recipe.AddRecipe();
            // FIXME: Definitely not finalized, someone check with this and see if the values are appropriate
		}
	}
}
