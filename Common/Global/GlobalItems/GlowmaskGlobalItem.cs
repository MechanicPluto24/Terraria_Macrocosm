using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Global.GlobalItems
{
	/// <summary>
	/// Adds a glowmask field to GlobalItems.
	/// Code by Hallam 
	/// </summary>
	public class GlowmaskGlobalItem : GlobalItem
	{
		public Texture2D texture = null;
		public int glowOffsetY = 0;
		public int glowOffsetX = 0;
		public override bool InstancePerEntity => true;
		public override GlobalItem Clone(Item item, Item itemClone)
		{
			return base.Clone(item, itemClone);
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (texture != null)
			{
				spriteBatch.Draw
				(
					texture,
					new Vector2
					(
						item.position.X - Main.screenPosition.X + item.width * 0.5f,
						item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
					),
					new Rectangle(0, 0, texture.Width, texture.Height),
					Color.White,
					rotation,
					texture.Size() * 0.5f,
					scale,
					SpriteEffects.None,
					0f
				);
			}
			base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
		}
	}
}
