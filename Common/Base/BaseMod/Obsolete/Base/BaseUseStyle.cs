using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Macrocosm;

namespace Macrocosm
{
    public class BaseUseStyle
    {
        //------------------------------------------------------//
        //----------------BASE USE STYLE CLASS------------------//
        //------------------------------------------------------//
        // Contains methods relating to custom use styles.      //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

		/*
		 * Simulates the Harpoon useStyle.
		 * 
		 * projType : The type of the Projectile to be aiming at.
		 * isIndex : If true, takes projType as the index in the Projectile array instead of as a type.
		 */
		public static void SetStyleHarpoon(Player player, Item item, int projType, bool isIndex = false)
		{
			int projID = (isIndex ? projType : BaseAI.GetProjectile(player.Center, projType, player.whoAmI, default(int[])));
			if(projID != -1)
			{
				Vector2 center = Main.Projectile[projID].Center;
				float distX = center.X - player.Center.X;
				float distY = center.Y - player.Center.Y;
				player.direction = (center.X > player.Center.X ? 1 : -1);
				player.itemRotation = (float)Math.Atan2(distY * player.direction, distX * player.direction);

				if (player.whoAmI == Main.myPlayer && Main.netMode != 0)
				{
					NetMessage.SendData(13, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0f, 0f, 0f, 0);
					NetMessage.SendData(41, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0f, 0f, 0f, 0);
				}
			}
			MoveItemLocationGun(player, item);
		}

		/*
		 * Simulates useStyle 4, ie most boss summoning items.
		 * 
		 * useItemHitbox: if true, uses the item's hitbox for offsetting instead of the texture's width and height.
		 * center: if true, centers the Item.
		 */
		public static void SetStyleBoss(Player player, Item item, bool useItemHitbox = false, bool center = false)
		{
			Rectangle hitbox = (useItemHitbox || Main.netMode == 2 || Main.dedServ ? Item.Hitbox : new Rectangle(0, 0, Main.itemTexture[Item.type].Width, Main.itemTexture[Item.type].Height));
			player.itemRotation = 0f;
			player.itemLocation.X = player.position.X + (float)player.width * 0.5f + ((center ? 0f : (float)hitbox.Width * 0.5f) - 9f - player.itemRotation * 14f * (float)player.direction - 4f) * (float)player.direction;
			player.itemLocation.Y = player.position.Y + (float)hitbox.Height * 0.5f + 4f;
			if (player.gravDir == -1f)
			{
				player.itemRotation = -player.itemRotation;
				player.itemLocation.Y = player.position.Y + (float)player.height + (player.position.Y - player.itemLocation.Y);
			}
			if (Main.myPlayer == player.whoAmI && Main.netMode != 0)
			{
				NetMessage.SendData(13, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0f, 0f, 0f, 0);
				NetMessage.SendData(41, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0f, 0f, 0f, 0);
			}
		}

		/*
		 * Change the arm frame in the same way that useStyle 4 would.
		 */
		public static void SetFrameBoss(Player player, Item item)
		{
			player.bodyFrame.Y = player.bodyFrame.Height * 2;
		}

        /*
         * Simulates useStyle 5, ie most guns.
		 * 
		 * NOTE: call this method in the Projectile's PreShoot method as well to prevent strange flip problems
         */
        public static void SetStyleGun(Player player, Item item, bool ignoreItemTime = false)
        {
            if (player.whoAmI == Main.myPlayer && (ignoreItemTime || player.itemTime == Item.useTime - 1))
            {
                float distX = Main.mouseX + Main.screenPosition.X - player.Center.X;
                float distY = Main.mouseY + Main.screenPosition.Y - player.Center.Y; 
                player.itemRotation = (float)Math.Atan2(distY * player.direction, distX * player.direction);

                if(Main.netMode != 0)
                {
                    NetMessage.SendData(13, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0f, 0f, 0f, 0);
                    NetMessage.SendData(41, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0f, 0f, 0f, 0);
                }
            }
            MoveItemLocationGun(player, item);
        }

        /*
         * Simulates useStyle 5, ie most guns.
         */
        public static void SetStyleGun(Vector2 target, Vector2 center, ref Vector2 itemLocation, ref float itemRotation, Item item, int direction, int itemTime, int useTime, bool ignoreItemTime = false)
        {
            if(ignoreItemTime || itemTime == useTime - 1)
            {
				float distX = target.X - center.X;
				float distY = target.Y - center.Y;
				itemRotation = (float)Math.Atan2(distY * direction, distX * direction);// +(float)Math.PI * -direction;
            }
            itemLocation = MoveItemLocationGun(center, itemLocation, direction, item);
        }

        /*
         * Simulates useStyle 1, ie most swords.
         */
        public static void SetStyleSword(Player player, Item item, bool basedOnRot = false)
        {
			player.itemRotation = ((float)player.itemAnimation / player.itemAnimationMax - 0.5f) * -player.direction * 3.5f - player.direction * 0.3f;
			if (player.gravDir == -1f) { player.itemRotation *= -1; }
			if (Main.myPlayer == player.whoAmI && Main.netMode != 0)
			{
				NetMessage.SendData(13, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0.0f, 0.0f, 0.0f, 0);
                NetMessage.SendData(41, -1, -1, NetworkText.FromLiteral(""), player.whoAmI, 0.0f, 0.0f, 0.0f, 0);
            }
			MoveItemLocationSword(player, item, basedOnRot);
        }

        /*
         * Simulates useStyle 1, ie most swords. (NPC Compatible)
         */
        public static void SetStyleSword(Vector2 position, int width, int height, ref Vector2 itemLocation, ref float itemRotation, int itemAnimation, int itemAnimationMax, int direction, float gravDir, Item item, bool basedOnRot = false)
        {
            itemRotation = ((float)itemAnimation / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            if (gravDir == -1f) { itemRotation *= -1; }
            itemLocation = MoveItemLocationSword(position, width, height, itemLocation, itemAnimation, itemAnimationMax, itemRotation, direction, gravDir, item, basedOnRot);
        }

        public static void MoveItemLocationGun(Player player, Item item)
        {
            player.itemLocation = MoveItemLocationGun(player.Center, player.itemLocation, player.direction, item);
        }

        /*
         * Moves the rectangle Player.itemLocation in the same way that useStyle 5 would. (NPC Compatible)
         */
        public static Vector2 MoveItemLocationGun(Vector2 center, Vector2 itemLocation, int direction, Item item)
        {
            itemLocation.X = center.X - (Main.netMode == 2 || Main.dedServ ? Item.width * 0.5f : Main.itemTexture[Item.type].Width * 0.5f) - direction * 2;
			itemLocation.Y = center.Y - (Main.netMode == 2 || Main.dedServ ? Item.height * 0.5f : Main.itemTexture[Item.type].Height * 0.5f);
            return itemLocation;
        }

        /*
         * Moves the itemLocation in the same way that useStyle 1 would.
         * basedOnRot : If true, changes it to use itemRotation instead of itemAnimation. 
         */		
        public static void MoveItemLocationSword(Player player, Item item, bool basedOnRot = false)
        {
            player.itemLocation = MoveItemLocationSword(player.position, player.width, player.height, player.itemLocation, player.itemAnimation, player.itemAnimationMax, player.itemRotation, player.direction, player.gravDir, item, basedOnRot);
        }

        /*
         * Moves the itemLocation in the same way that useStyle 1 would. (NPC Compatible)
         * basedOnRot : If true, changes it to use itemRotation instead of itemAnimation. 
         */
        public static Vector2 MoveItemLocationSword(Vector2 position, int width, int height, Vector2 itemLocation, int itemAnimation, int itemAnimationMax, float itemRotation, int direction, float gravDir, Item item, bool basedOnRot = false)
        {
            float rot30 = !basedOnRot ? 0f : ((float)(itemAnimationMax * 0.33f) * 0.75f / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            float rot60 = !basedOnRot ? 0f : ((float)(itemAnimationMax * 0.66f) * 0.75f / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            bool is30 = itemRotation > rot30;
            bool is60 = itemRotation > rot60;

            if (!basedOnRot ? itemAnimation < itemAnimationMax * 0.33f : ((gravDir == 1f && ((direction == 1 && is30) || (direction == -1 && !is30))) || (gravDir == -1f && ((direction == 1 && !is30) || (direction == -1 && is30)))))
            {
                float OffsetX = 10.0f;
                if (Main.itemTexture[Item.type].Width > 64) OffsetX = 28.0f;
                else if (Main.itemTexture[Item.type].Width > 32) OffsetX = 14.0f;
                itemLocation.X = position.X + width * 0.5f + (Main.itemTexture[Item.type].Width * 0.5f - OffsetX) * direction;
                itemLocation.Y = position.Y + 24;
            }
            else if (!basedOnRot ? itemAnimation < itemAnimationMax * 0.66f : ((gravDir == 1f && ((direction == 1 && is60) || (direction == -1 && !is60))) || (gravDir == -1f && ((direction == 1 && !is60) || (direction == -1 && is60)))))
            {
                float OffsetX = 10.0f;
                if (Main.itemTexture[Item.type].Width > 64) OffsetX = 28.0f; 
                else if (Main.itemTexture[Item.type].Width > 32)  OffsetX = 18.0f;
                itemLocation.X = position.X + width * 0.5f + (Main.itemTexture[Item.type].Width * 0.5f - OffsetX) * direction;
                OffsetX = 10.0f;
                if (Main.itemTexture[Item.type].Height > 64) OffsetX = 14.0f;
                else if (Main.itemTexture[Item.type].Height > 32) OffsetX = 8.0f;

                itemLocation.Y = position.Y + OffsetX;
            }else
            {
                float OffsetX = 6.0f;

                if (Main.itemTexture[Item.type].Width > 64) OffsetX = 28.0f;
                else if (Main.itemTexture[Item.type].Width > 32) OffsetX = 14.0f;
                itemLocation.X = position.X + width * 0.5f - (Main.itemTexture[Item.type].Width * 0.5f - OffsetX) * direction;
                OffsetX = 10.0f;
                if (Main.itemTexture[Item.type].Height > 64) OffsetX = 14.0f;
                itemLocation.Y = position.Y + OffsetX;
            }
            if (gravDir == -1.0f)
            {
                itemLocation.Y = position.Y + height + (position.Y - itemLocation.Y);
            }
            return itemLocation;
        }

        public static void SetFrameGun(Player player, Item item)
        {
            player.bodyFrame = SetFrameGun(player.bodyFrame, player.itemRotation, player.direction, player.gravDir, item);
        }

        /*
         * Change the arm frame in the same way that useStyle 5 would.
         */
        public static Rectangle SetFrameGun(Rectangle bodyFrame, float itemRotation, int direction, float gravDir, Item item)
        {
            float FacingRotation = itemRotation * direction;
            bodyFrame.Y = bodyFrame.Height * 3;
            if (FacingRotation < -0.75f)
            {
                bodyFrame.Y = bodyFrame.Height * 2;
                if (gravDir == -1.0f) bodyFrame.Y = bodyFrame.Height * 4;
            }
            if (FacingRotation > 0.6f)
            {
                bodyFrame.Y = bodyFrame.Height * 4;
                if (gravDir == -1.0f) bodyFrame.Y = bodyFrame.Height * 2;
            }
            return bodyFrame;
        }

        public static void SetFrameSword(Player player, Item item, bool basedOnRot = false)
        {
            player.bodyFrame = SetFrameSword(player.bodyFrame, player.itemAnimation, player.itemAnimationMax, player.itemRotation, player.direction, player.gravDir, item, basedOnRot);
        }

        /*
         * Change the arm frame in the same way that useStyle 1 would.
         * basedOnRot : If true, changes it to use itemRotation instead of itemAnimation.
         */
        public static Rectangle SetFrameSword(Rectangle bodyFrame, int itemAnimation, int itemAnimationMax, float itemRotation, int direction, float gravDir, Item item, bool basedOnRot = false)
        {
            float rot30 = !basedOnRot ? 0f : ((float)(itemAnimationMax * 0.33f) * 0.75f / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            float rot60 = !basedOnRot ? 0f : ((float)(itemAnimationMax * 0.66f) * 0.75f / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            bool is30 = itemRotation > rot30;
            bool is60 = itemRotation > rot60;
            if (!basedOnRot ? itemAnimation < itemAnimationMax * 0.33f : ((gravDir == 1f && ((direction == 1 && is30) || (direction == -1 && !is30))) || (gravDir == -1f && ((direction == 1 && !is30) || (direction == -1 && is30)))))
                bodyFrame.Y = bodyFrame.Height * 3;
            else if (!basedOnRot ? itemAnimation < itemAnimationMax * 0.66f : ((gravDir == 1f && ((direction == 1 && is60) || (direction == -1 && !is60))) || (gravDir == -1f && ((direction == 1 && !is60) || (direction == -1 && is60)))))
                bodyFrame.Y = bodyFrame.Height * 2;
            else 
                bodyFrame.Y = bodyFrame.Height;
            return bodyFrame;
        }


        public static Rectangle UpdateHitBoxSword(Player player, Item item, Rectangle ItemRect, bool basedOnRot = false)
        {
            return UpdateHitBoxSword(player.itemAnimation, player.itemAnimationMax, player.itemRotation, player.direction, player.gravDir, item, ItemRect, basedOnRot);
        }

        /*
         * Updates the HitBox in the same way that useStyle 1 would.
         * basedOnRot : If true, changes it to use Player.itemRotation instead of Player.itemAnimation.
         * scale : If not 0f, can be used to manually scale the hitbox independent of the graphics.
         */
        public static Rectangle UpdateHitBoxSword(int itemAnimation, int itemAnimationMax, float itemRotation, int direction, float gravDir, Item item, Rectangle ItemRect, bool basedOnRot = false)
        {
            float rot30 = !basedOnRot ? 0f : ((float)(itemAnimationMax * 0.33f) * 0.75f / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            float rot60 = !basedOnRot ? 0f : ((float)(itemAnimationMax * 0.66f) * 0.75f / itemAnimationMax - 0.5f) * -direction * 3.5f - direction * 0.3f;
            bool is30 = itemRotation > rot30;
            bool is60 = itemRotation > rot60;
            if (!basedOnRot ? itemAnimation < itemAnimationMax * 0.33f : ((gravDir == 1f && ((direction == 1 && is30) || (direction == -1 && !is30))) || (gravDir == -1f && ((direction == 1 && !is30) || (direction == -1 && is30)))))
            {
                if (direction == -1)
                    ItemRect.X -= (int)(ItemRect.Width * 1.4f - ItemRect.Width);

                ItemRect.Width = (int)(ItemRect.Width * 1.4f);
                ItemRect.Height = (int)(ItemRect.Height * 1.1f);
                ItemRect.Y += (int)(ItemRect.Height * 0.5f * gravDir);
            }else if (!basedOnRot ? itemAnimation < itemAnimationMax * 0.66f : ((gravDir == 1f && ((direction == 1 && is60) || (direction == -1 && !is60))) || (gravDir == -1f && ((direction == 1 && !is60) || (direction == -1 && is60)))))
            {
                if (direction == 1)
                    ItemRect.X -= (int)(ItemRect.Width * 1.2f);

                ItemRect.Y -= (int)((ItemRect.Height * 1.4f - ItemRect.Height) * gravDir);
                ItemRect.Width *= 2;
                ItemRect.Height = (int)(ItemRect.Height * 1.4f);
            }
            return ItemRect;
        }
    }
}