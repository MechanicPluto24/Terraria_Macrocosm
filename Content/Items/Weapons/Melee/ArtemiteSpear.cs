using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
	public class ArtemiteSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
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

        private int altUseCooldown;

        public override void UpdateInventory(Player player)
        {
            if (altUseCooldown > 0)
                altUseCooldown--;
        }

        public override bool AltFunctionUse(Player player) => altUseCooldown <= 0;

        public override bool CanUseItem(Player player)
        {
            if (player.AltFunction())
            {

                altUseCooldown = 25;

				return true;
            }
            else if(player.ownedProjectileCounts[ModContent.ProjectileType<ArtemiteSpearProjectile>()] < 1)
            {
				return true;
            }

            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.AltFunction())
			{
				Projectile.NewProjectile(source, position, velocity * 25, ModContent.ProjectileType<ArtemiteSpearProjectileThrown>(), damage, knockback, Main.myPlayer, ai0: 0f);
			}
			else
			{
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArtemiteSpearProjectile>(), damage, knockback, Main.myPlayer, ai0: 0f);
            }

            return false;
        }

        public override bool? UseItem(Player player)
		{
			if (!Main.dedServ && Item.UseSound.HasValue)
			{
				SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
			}
			return null;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient<ArtemiteBar>(12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
