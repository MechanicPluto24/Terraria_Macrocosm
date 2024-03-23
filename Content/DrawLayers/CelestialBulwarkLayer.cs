using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Macrocosm.Content.DrawLayers
{
	public class CelestialBulwarkLayer : PlayerDrawLayer
	{
        Asset<Texture2D> shieldMask;

        public override void Load()
        {
            shieldMask = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Accessories/CelestialBulwark_Shield_Mask");
        }

        public override Position GetDefaultPosition()
			=> new AfterParent(PlayerDrawLayers.Shield);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
			=> drawInfo.drawPlayer.shield == EquipLoader.GetEquipSlot(Macrocosm.Instance, "CelestialBulwark", EquipType.Shield);

		protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.shieldRaised)
                return;

            CelestialBulwark.GetEffectColor(drawInfo.drawPlayer, out Color drawColor, out Color? secondaryColor, out _, out bool bypassShader, out _);

            int shader = bypassShader ? -1 : drawInfo.cShield;
            //if(secondaryColor.HasValue) 
            //    drawColor = secondaryColor.Value;

            if (drawInfo.shadow > 0f)
                drawColor = (drawColor * (drawInfo.shadow / 4f)).WithOpacity(0.5f);

            Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
            Vector2 bodyVect = drawInfo.bodyVect;

            if (bodyFrame.Width != shieldMask.Width())
            {
                bodyFrame.Width = shieldMask.Width();
                bodyVect.X += bodyFrame.Width - shieldMask.Width();

                if (drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally))
                    bodyVect.X = bodyFrame.Width - bodyVect.X;
            }

            Vector2 position = Vector2.Zero + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
            DrawData item = new(shieldMask.Value, position, bodyFrame, drawColor, drawInfo.drawPlayer.bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 1)
            {
                shader = shader
            };
            drawInfo.DrawDataCache.Add(item);

            if (drawInfo.drawPlayer.mount.Cart)
                drawInfo.DrawDataCache.Reverse(drawInfo.DrawDataCache.Count - 2, 2);
        }
    }
}
