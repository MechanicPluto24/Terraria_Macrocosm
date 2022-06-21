using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Common.Utility;

namespace Macrocosm.Content.Items.Weapons
{
	public class Crucible : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("BasicSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("A sword forged from the fires of Hell itself" + "\n'The only thing they fear is you.'");
		}
        
		public override void SetDefaults() 
		{
			Item.damage = 666;
			Item.DamageType = DamageClass.Melee;
			Item.width = 76;
			Item.height = 76;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing; // 1 = sword
			Item.knockBack = 6f;
			Item.value = 10000;
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true; // Lets you use the item without clicking the mouse repeatedly (i.e. swinging swords)
			Item.GetGlobalItem<GlowmaskGlobalItem>().glowTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/Crucible_Glow").Value;
		}
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<CrucibleDust>());
			}
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.85f * Main.essScale);
		}

		public override void AddRecipes() 
		{
			Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient(ItemID.HellstoneBar, 20);
			recipe.AddIngredient(ItemID.SoulofFright, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}