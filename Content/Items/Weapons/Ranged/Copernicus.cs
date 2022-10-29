using Macrocosm.Assets.Sounds;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
	public class Copernicus : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Copernicus");
			Tooltip.SetDefault("Right click to launch a plasma ball grenade");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 70;
			Item.height = 26;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true;
			Item.knockBack = 8f;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<MoonRarityT2>();
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
		}

		private int altUseCooldown = 30;
		private int altUseCounter = 30;

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override bool AltFunctionUse(Player player) => altUseCounter == altUseCooldown && ItemUtils.ToRocketProjectileID(player, ItemID.GrenadeLauncher) != 0;

		public override bool CanUseItem(Player player) => true;

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
 				SoundEngine.PlaySound(SFX.GrenadeLauncherThunk with { Volume = 0.7f });
 			else
 				SoundEngine.PlaySound(SFX.AssaultRifle with { Volume = 0.7f });
 
			return true;
		}

		public override void UpdateInventory(Player player)
		{
			if (player.altFunctionUse == 2 || altUseCounter < altUseCooldown)
				altUseCounter--;

			if (altUseCounter == 0)
				altUseCounter = altUseCooldown;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			return true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{

			int defaulType = type;

			if (player.altFunctionUse == 2)
			{
				//type = ItemUtils.ToRocketProjectileID(player, ItemID.GrenadeLauncher);
				type = ModContent.ProjectileType<PlasmaGrenade>();
				position.Y += 2;
				velocity /= 3f;
			}
			else
			{
				type = defaulType;
				position -= new Vector2(2 * player.direction, 2); // so bullets line up with the muzzle
			}
		}
	}
}
