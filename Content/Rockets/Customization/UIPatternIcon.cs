using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace Macrocosm.Common.UI
{
	public class UIPatternIcon : UIFocusIconButton, IFocusable
	{
		public Pattern Pattern { get; set; }

		public UIPatternIcon(Pattern pattern)
		: base
		(
			Macrocosm.EmptyTexAsset,
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
		) 
		{
			Pattern = pattern;
		}

		public override void OnInitialize()
		{
 			FocusContext = "PatternSelection";
			OnLeftClick += (_, _) => { HasFocus = true; };
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			var dimensions = GetOuterDimensions();
			Pattern.DrawIcon(spriteBatch, dimensions.Position());
		}
	}
}
