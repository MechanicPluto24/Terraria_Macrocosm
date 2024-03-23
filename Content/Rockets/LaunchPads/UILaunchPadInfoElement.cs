using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public class UILaunchPadInfoElement : UIInfoElement, IFocusable
	{
		public LaunchPad LaunchPad { get; init; }

		public bool HasFocus { get; set; }
		public string FocusContext { get; set; }
		public Action OnFocusGain { get; set; }
		public Action OnFocusLost { get; set; }

		public bool IsSpawnPointDefault => LaunchPad is null;

		public bool IsCurrent { get; set; }
		public bool IsReachable { get; set; }

		public bool CanInteract => !IsCurrent && IsReachable && (IsSpawnPointDefault || !LaunchPad.HasRocket);

        public UILaunchPadInfoElement() : base(
			Language.GetText("Mods.Macrocosm.UI.Rocket.Common.WorldSpawn"),
			ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/WorldInfo/SpawnPoint", AssetRequestMode.ImmediateLoad),
			null,
			null
		)
		{ }

		public UILaunchPadInfoElement(LaunchPad launchPad) : base(launchPad.CompassCoordinates, null, null, null)
		{
			LaunchPad = launchPad;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
			BorderColor = UITheme.Current.PanelStyle.BorderColor;

			if(uIDisplayText is not null)
				uIDisplayText.TextColor = UITheme.Current.CommonTextColor;

			if (CanInteract)
			{
				if (IsMouseHovering)
				{
					BackgroundColor = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
					BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
				}

				if (HasFocus)
				{
                    BackgroundColor = UITheme.Current.ButtonStyle.BackgroundColor;
                    BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
                }
			}
			else
			{
				BackgroundColor = Color.Lerp(UITheme.Current.PanelStyle.BackgroundColor, Color.DarkGray, 0.1f);

                if (uIDisplayText is not null)
                    uIDisplayText.TextColor = Color.LightGray;
			}

			if (IsCurrent)
			{
				BorderColor = Color.LimeGreen;
			}
		}

	}
}
