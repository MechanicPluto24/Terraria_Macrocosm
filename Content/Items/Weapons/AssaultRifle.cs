using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Items.Weapons
{
	public class AssaultRifle : ModItem
	{
		public override void SetStaticDefaults() 
		{
            DisplayName.SetDefault("Dovah's Assault Rifle");
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
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item41; // do it in UseItem?
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
        }

		private const int altUseCooldown = 30;
		private int altUseCounter = altUseCooldown;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override bool AltFunctionUse(Player player) => altUseCounter == altUseCooldown;

        public override bool CanUseItem(Player player) => true;

		public override bool? UseItem(Player player)
		{
			//if (player.altFunctionUse == 2)
			//{
			//	Item.useAmmo = AmmoID.Rocket;
			//}
            //else
            //{
			//	Item.useAmmo = AmmoID.Bullet;
            //}

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
				type = ProjectileID.RocketI;
				velocity /= 3f;
			}
            else
            {
				type = defaulType;
			}

			position.Y -= 2;
		}
	}
}
