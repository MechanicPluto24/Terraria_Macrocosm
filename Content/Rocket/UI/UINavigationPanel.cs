using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Earth;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UINavigationPanel : UIElement
	{

		public UINavigationMap CurrentMap;

		private UINavigationMap EarthSystem;
		private UINavigationMap SolarSystemOuter;
		private UINavigationMap SolarSystemInner;

		public override void OnInitialize()
		{
			Width.Set(606, 0);
			Height.Set(234, 0);
			HAlign = 0.5f;
			VAlign = 0.1f;
			UIPanel panel = new();
			panel.Width.Set(Width.Pixels, 0f);
			panel.Height.Set(Height.Pixels, 0f);

			InitializePanelContent();
			CurrentMap = GetInitialNavigationMap();

			panel.Append(CurrentMap);
			Append(panel);
		}


		public void InitializePanelContent()
		{
			EarthSystem = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationMap_Earth", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);

			EarthSystem.AddTarget(new UIMapTarget(new Vector2(60, 20), 170, 174, () => SubworldSystem.AnyActive<Macrocosm>(), Earth.SubworldData));
			EarthSystem.AddTarget(new UIMapTarget(new Vector2(428, 34), 48, 50, () => Moon.Instance.CanTravelTo(), Moon.Instance.SubworldData));
			EarthSystem.Prev = SolarSystemInner;

			SolarSystemInner = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationMap_Inner", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
			SolarSystemInner.Prev = SolarSystemOuter;

			SolarSystemOuter = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationMap_Outer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		}

		/// <summary> Use this determining the default navigation map based on the current subworld </summary>
		public UINavigationMap GetInitialNavigationMap()
		{
			// if(SubworldSystem.IsActive<Moon>() || !SubworldSystem.AnyActive<Macrocosm>())
			return EarthSystem;

			// if(Mars or Phobos or Deimos)			
			//		return MarsSystem;

			// ...
		}

		public void ZoomIn()
		{
			CurrentMap = CurrentMap.Next ?? CurrentMap;
		}

		public void ZoomOut()
		{
			CurrentMap = CurrentMap.Prev ?? CurrentMap;
		}
	}
}
