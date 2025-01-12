using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class DianiteTome : ModItem
    {
        private static Asset<Texture2D> heldTexture;

        public override void Load()
        {
            heldTexture = ModContent.Request<Texture2D>(Texture + "_Held");
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 155;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 12;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.channel = true;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item78;
            Item.shoot = ModContent.ProjectileType<DianitePortal>();

            Item.noUseGraphic = true;
            Item.CustomDrawData().CustomHeldTexture = heldTexture;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<DianiteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 1);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, damage, knockBack, player.whoAmI);
            return false;
        }

        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
		{
			int numProj = 2 + Main.rand.Next(1);  //This defines how many projectiles to shoot

			for (int index = 0; index < numProj; ++index)
			{
				bool bigProjectile = Main.rand.NextBool(4);
				int projType = bigProjectile ? ModContent.ProjectileType<DianiteMeteor>() : type;
				damage = (int)(damage * (bigProjectile ? 1.4f : 1f));

				Vector2 playerOffset = new Vector2((float)(player.position.X + player.width * 0.5 + Main.rand.Next(201) * -player.direction + (Main.mouseX + (double)Main.screenPosition.X - player.position.X)), (float)(player.position.Y + player.height * 0.5 - 600.0));   //this defines the Projectile width, direction and position
				playerOffset.X = (float)((playerOffset.X + (double)player.Center.X) / 2.0) + Main.rand.Next(-200, 201);
				playerOffset.Y -= 100 * index;

				float posX = Main.mouseX + Main.screenPosition.X - playerOffset.X;
				float posY = Main.mouseY + Main.screenPosition.Y - playerOffset.Y;

				if ((double)posY < 0.0)
					posY *= -1f;

				if ((double)posY < 20.0)
					posY = 20f;

				float magnitude = (float)Math.Sqrt((double)posX * (double)posX + (double)posY * (double)posY);
				float normSpeed = Item.shootSpeed / magnitude;

				float SpeedX = posX * normSpeed + Main.rand.Next(-40, 41) * 0.02f;  //this defines the Projectile X position speed and randomness
				float SpeedY = posY * normSpeed + Main.rand.Next(-40, 41) * 0.02f;  //this defines the Projectile Y position speed and randomness
				Projectile.NewProjectile(source, playerOffset.X, playerOffset.Y, SpeedX, SpeedY, projType, damage, knockBack, Main.myPlayer, Main.MouseWorld.Y);
			}
			return false;
		}*/
    }
}