using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public class UILaunchPadInfoElement : UIInfoElement, IFocusable
	{
		public LaunchPad LaunchPad { get; init; }

		public bool IsCurrent { get; set; }

		public UILaunchPadInfoElement(LaunchPad launchPad) : base(Utility.GetCompassCoordinates(launchPad.Position), null, null, null)
		{
			LaunchPad = launchPad;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			IsCurrent = false;
			BackgroundColor = new Color(43, 56, 101);
			BorderColor = BackgroundColor * 2f;
			uIDisplayText.TextColor = Color.White;

			if (!LaunchPad.HasRocket)
			{
				if (HasFocus)
					BorderColor = Color.Gold;
			} 
			else
			{
				if(IsCurrent)
					BorderColor = Color.Green;

				BackgroundColor = (BackgroundColor * 0.85f).WithOpacity(1f);
				uIDisplayText.TextColor = Color.LightGray;
			}
		}

		public bool HasFocus { get; set; }
		public string FocusContext { get; set; }
		public Action OnFocusGain { get; set; }
		public Action OnFocusLost { get; set; }
	}
}
