using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

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

		public UILaunchPadInfoElement() : base(
			Language.GetText("Mods.Macrocosm.UI.Rocket.Common.WorldSpawn"), 
			ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/WorldInfo/SpawnPoint"), 
			null,
			null
		){ }

		public UILaunchPadInfoElement(LaunchPad launchPad) : base(launchPad.CompassCoordinates, null, null, null)
		{
			LaunchPad = launchPad;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			BackgroundColor = new Color(43, 56, 101);
			BorderColor = BackgroundColor * 2f;
			uIDisplayText.TextColor = Color.White;

			if (IsSpawnPointDefault || !LaunchPad.HasRocket)
			{
				if (HasFocus || IsMouseHovering)
					BorderColor = Color.Gold;
			} 
			else
			{
				BackgroundColor = (BackgroundColor * 0.85f).WithOpacity(1f);
				uIDisplayText.TextColor = Color.LightGray;
			}

			if (IsCurrent)
				BorderColor = new Color(0,255,0);
		}

	}
}
