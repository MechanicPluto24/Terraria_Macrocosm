using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;
using Macrocosm.Common.Drawing;
using Macrocosm.Content.Systems;

namespace Macrocosm.Content.DrawLayers
{
	public class CelestialBulwarkLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition()
			=> new AfterParent(PlayerDrawLayers.Shield);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
			=> drawInfo.drawPlayer.shield == EquipLoader.GetEquipSlot(Macrocosm.Instance, "CelestialBulwark", EquipType.Shield);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.shieldRaised)
				return;

			Texture2D texture = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark/CelestialBulwark_Shield_Mask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Color drawColor = MacrocosmWorld.CelestialColor;

			if (drawInfo.shadow > 0f)
				drawColor = (drawColor * (drawInfo.shadow / 4f)).NewAlpha(0.5f);

			Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
			Vector2 bodyVect = drawInfo.bodyVect;

			if (bodyFrame.Width != texture.Width)
			{
				bodyFrame.Width = texture.Width;
				bodyVect.X += bodyFrame.Width - texture.Width;

				if (drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally))
					bodyVect.X = bodyFrame.Width - bodyVect.X;
			}

			Vector2 position = Vector2.Zero + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
			DrawData item = new DrawData(texture, position, bodyFrame, drawColor, drawInfo.drawPlayer.bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 1);
			item.shader = drawInfo.cShield;
			drawInfo.DrawDataCache.Add(item);

			if (drawInfo.drawPlayer.mount.Cart)
				drawInfo.DrawDataCache.Reverse(drawInfo.DrawDataCache.Count - 2, 2);

		}
	}
}
