using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

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
			Item.knockBack = 8f;
			Item.value = 10000;
			Item.channel = true;
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item41;
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
		}

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position = (Vector2)(player.Center + HoldoutOffset() + new Vector2(0,-2));
			type = ProjectileID.Bullet;
		}
	}

	public class HoldOut : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
			return drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<AssaultRifle>();
		}

		private Asset<Texture2D> weaponTex;

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (weaponTex == null)
			{
				weaponTex = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/AssaultRifle");
			}

			Texture2D itemTexture = weaponTex.Value;

			ModItem weapon = drawInfo.heldItem.ModItem;

			Rectangle? sourceRect = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
			Vector2 vector3 = new(itemTexture.Width / 2, itemTexture.Height / 2);
			Vector2 vector4 = Main.DrawPlayerItemPos(drawInfo.drawPlayer.gravDir, weapon.Type);
			int num12 = (int)vector4.X;
			vector3.Y = vector4.Y;
			Vector2 origin6 = new(-num12, itemTexture.Height / 2);
			if (drawInfo.drawPlayer.direction == -1)
				origin6 = new Vector2(itemTexture.Width + num12, itemTexture.Height / 2);

			Vector2 offset = (Vector2)weapon.HoldoutOffset() - new Vector2(0,0);

			Vector2 position = new Vector2((int)(drawInfo.drawPlayer.position.X - Main.screenPosition.X + vector3.X + offset.X),
										   (int)(drawInfo.drawPlayer.position.Y - Main.screenPosition.Y + vector3.Y + offset.Y));


			float rotation = (Main.MouseWorld - drawInfo.drawPlayer.MountedCenter).ToRotation();


			DrawData drawData = new DrawData(
				itemTexture, 
				position,
				sourceRect, 
				new Color(250, 250, 250, 255),
				rotation,
				origin6, 
				1f,
				drawInfo.itemEffect, 
				0);

			drawInfo.DrawDataCache.Add(drawData);
		}
	}


}
