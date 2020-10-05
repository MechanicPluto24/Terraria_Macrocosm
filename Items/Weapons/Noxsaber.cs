using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Dusts;

namespace Macrocosm.Items.Weapons
{
	public class Noxsaber : ModItem
	{
		public static short customGlowMask = 0;

		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("The Noxsaber"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("An ancient black energy saber, stolen from the temple of a long dead cult");
		}
        public override bool UseItem(Player player)
        {
			if (Main.netMode != NetmodeID.Server)
			{
				Texture2D[] glowMasks = new Texture2D[Main.glowMaskTexture.Length + 1];
				for (int i = 0; i < Main.glowMaskTexture.Length; i++)
				{
					glowMasks[i] = Main.glowMaskTexture[i];
				}
				glowMasks[glowMasks.Length - 1] = mod.GetTexture("Items/Weapons/" + GetType().Name + "_Glow");
				customGlowMask = (short)(glowMasks.Length - 1);
				Main.glowMaskTexture = glowMasks;
			}
			item.glowMask = customGlowMask;
			return base.UseItem(player);
        }
        public override void SetDefaults() 
		{
			item.damage = 777;
			item.melee = true;
			item.width = 100;
			item.height = 100;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.SwingThrow; // 1 = sword
			item.knockBack = 6f;
			item.value = 10000;
			item.rare = ItemRarityID.Red;
			item.UseSound = SoundID.Item15;
			item.autoReuse = true; // Lets you use the item without clicking the mouse repeatedly (i.e. swinging swords)
			item.glowMask = customGlowMask;
		}
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			//if (Main.rand.Next(2) == 0)
			//{
				//int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, mod.DustType("CrucibleDust"));
			//}
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.85f * Main.essScale);
		}

		public override void AddRecipes() 
		{
			//ModRecipe recipe = new ModRecipe(mod);
			//recipe.AddIngredient(ItemID.HellstoneBar, 20);
			//recipe.AddIngredient(ItemID.SoulofFright, 10);
			//recipe.AddTile(TileID.WorkBenches);
			//recipe.SetResult(this);
			//recipe.AddRecipe();
		}
	}
}