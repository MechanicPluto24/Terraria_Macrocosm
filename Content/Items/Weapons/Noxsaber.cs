using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Common.Utility;

namespace Macrocosm.Content.Items.Weapons
{
	public class Noxsaber : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("The Noxsaber"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("An ancient black energy saber, stolen from the temple of a long dead cult");
		}
        
        public override void SetDefaults() 
		{
			Item.damage = 777;
			Item.DamageType = DamageClass.Melee;
			Item.width = 100;
			Item.height = 100;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing; 
			Item.knockBack = 6f;
			Item.value = 10000;
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item15;
			Item.autoReuse = true; // Lets you use the item without clicking the mouse repeatedly (i.e. swinging swords)
			Item.GetGlobalItem<GlowmaskGlobalItem>().glowTexture = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/Noxsaber_Glow").Value;
		}
        
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			//if (Main.rand.NextBool(2))
			//{
			//	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<CrucibleDust>());
			//}
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.85f * Main.essScale);
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