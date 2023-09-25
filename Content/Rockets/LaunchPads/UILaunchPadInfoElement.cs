using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
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

		// TODO: localize this!!! ------------------V
		public UILaunchPadInfoElement() : base("World Spawn", ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Icons/SpawnPoint"), null, null)
		{ 
		}

		// TODO: adjust this to target world not the current one ---------------------V 
		public UILaunchPadInfoElement(LaunchPad launchPad) : base(Utility.GetCompassCoordinates(launchPad.Position), null, null, null)
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
