using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Rockets.UI
{
	public class UIDetailIcon : UIPanelIconButton, IFocusable
	{
		public Detail Detail { get; set; }

		public UIDetailIcon(Detail detail)
		: base
		(
            ModContent.RequestIfExists(detail.IconTexturePath, out Asset<Texture2D> icon) ? icon : Macrocosm.EmptyTexAsset,
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
		)
		{
			Detail = detail;
		}

		public override void OnInitialize()
		{
			FocusContext = "DetailSelection";
			OnLeftClick += (_, _) => { HasFocus = true; };
			HoverText = Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Customization.Details." + Detail.Name, () => Detail.Name);
        }

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			/*
			var dimensions = GetOuterDimensions();
			Texture2D texture = ModContent.Request<Texture2D>(Detail.TexturePath + "_Icon").Value;
			spriteBatch.Draw(texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 0.995f, SpriteEffects.None, 0f);
			*/
		}
	}
}
