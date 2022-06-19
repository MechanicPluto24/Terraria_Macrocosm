using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Items.Weapons
{
	public class Crucible : ModItem
	{
		public static short customGlowMask = 0;

		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("BasicSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("A sword forged from the fires of Hell itself" + "\n'The only thing they fear is you.'");

			if (Main.netMode != NetmodeID.Server)
			{
				Texture2D[] glowMasks = new Texture2D[Main.glowMaskTexture.Length + 1];
				for (int i = 0; i < Main.glowMaskTexture.Length; i++)
				{
					glowMasks[i] = Main.glowMaskTexture[i];
				}
				glowMasks[glowMasks.Length - 1] = mod.GetTexture("Content/Items/Weapons/" + GetType().Name + "_Glow");
				customGlowMask = (short)(glowMasks.Length - 1);
				Main.glowMaskTexture = glowMasks;
			}
			Item.glowMask = customGlowMask;
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
			Item.glowMask = customGlowMask;
		}
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, mod.DustType("CrucibleDust"));
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