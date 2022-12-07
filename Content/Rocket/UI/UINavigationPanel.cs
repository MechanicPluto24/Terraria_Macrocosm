using Macrocosm.Common.Drawing;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Earth;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UINavigationPanel : UIElement
	{
		public UINavigationMap CurrentMap;

		private UIPanel BackgroundPanel;

		private UINavigationMap EarthSystem;
		private UINavigationMap SolarSystemOuter;
		private UINavigationMap SolarSystemInner;

		private UINavigationZoomButton ZoomInButton;
		private UINavigationZoomButton ZoomOutButton;

		public override void OnInitialize()
		{
			Width.Set(586, 0);
			Height.Set(224, 0);
			Top.Set(10, 0);
			HAlign = 0.5f;
			BackgroundPanel = new();
			BackgroundPanel.Width.Set(Width.Pixels, 0f);
			BackgroundPanel.Height.Set(Height.Pixels, 0f);

			InitializePanelContent();
			CurrentMap = GetInitialNavigationMap();

			Append(BackgroundPanel);
			Append(ZoomInButton);
			Append(ZoomOutButton);
			Append(CurrentMap);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		/// <summary>
		/// Zooms in to the next map
		/// </summary>
		/// <param name="useDefault"> Whether to use the default map instead, it it exists </param>
		public void ZoomIn(bool useDefault = true)
		{
			UINavigationMap nextMap = CurrentMap.Next;

			if (useDefault)
				nextMap ??= CurrentMap.DefaultNext;
		
			UpdateCurrentMap(nextMap);
 		}

		/// <summary>
		/// Zooms out towards the previous map
		/// </summary>
		public void ZoomOut()
		{
			UpdateCurrentMap(CurrentMap.Prev);
		}

		/// <summary>
		/// Update to the target navigation map
		/// </summary>
		/// <param name="newMap"> The new map instance </param>
		public void UpdateCurrentMap(UINavigationMap newMap)
		{
			if (newMap == null)
				return;

			UINavigationMap prevMap = CurrentMap;
			RemoveChild(CurrentMap);
			CurrentMap = newMap;
			CurrentMap.ShowAnimation(prevMap.Background);
			Append(CurrentMap);

			Activate();
		}

		/// <summary> This method determines the default navigation map based on the current subworld </summary>
		private UINavigationMap GetInitialNavigationMap()
		{
			// if(SubworldSystem.IsActive<Moon>() || !SubworldSystem.AnyActive<Macrocosm>())
			return EarthSystem;

			// if(Mars or Phobos or Deimos)			
			//		return MarsSystem;

			// ...
		}

		private void InitializePanelContent()
		{
			AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

			Texture2D zoomInButton = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationZoomIn", mode).Value;
			Texture2D zoomInBorder = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationZoomInBorder", mode).Value;
			Texture2D zoomOutButton = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationZoomOut", mode).Value;
			Texture2D zoomOutBorder = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationZoomOutBorder", mode).Value;

			ZoomInButton = new(zoomInButton, zoomInBorder, new Vector2(6, 88));
			ZoomInButton.OnClick += (_, _) => ZoomIn();
			ZoomOutButton = new(zoomOutButton, zoomOutBorder, new Vector2(6, 118));
			ZoomOutButton.OnClick += (_, _) => ZoomOut();

			EarthSystem = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationMap_Earth", mode).Value);
			SolarSystemInner = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationMap_Inner", mode).Value, defaultNext: GetInitialNavigationMap());
			SolarSystemOuter = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UINavigationMap_Outer", mode).Value, defaultNext: SolarSystemInner);

			EarthSystem.AddTarget(new UIMapTarget(new Vector2(64, 24), 160, 160, Earth.WorldInfo, () => SubworldSystem.AnyActive<Macrocosm>()));
			EarthSystem.AddTarget(new UIMapTarget(new Vector2(428, 34), 48, 48, Moon.Instance));
			EarthSystem.Prev = SolarSystemInner;

			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(246, 86), 32, 32, WorldInfoStorage.Sun));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(226, 88), 6, 6, WorldInfoStorage.Vulcan));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(302, 128), 6, 6, WorldInfoStorage.Mercury));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(164, 76), 6, 6, WorldInfoStorage.Venus));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(366, 58), 6, 6, Earth.WorldInfo), EarthSystem);
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(70, 122), 6, 6, WorldInfoStorage.Mars));
			SolarSystemInner.Prev = SolarSystemOuter;

			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(256, 96), 12, 12, WorldInfoStorage.InnerSolarSystem), SolarSystemInner);
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(282, 106), 9, 9, WorldInfoStorage.Jupiter));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(220, 116), 9, 9, WorldInfoStorage.Saturn));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(334, 72), 9, 9, WorldInfoStorage.Ouranos));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(182, 42), 9, 9, WorldInfoStorage.Neptune));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(410, 156), 9, 9, WorldInfoStorage.Pluto));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(44, 20), 9, 9, WorldInfoStorage.Eris));
			SolarSystemOuter.Next = SolarSystemInner;
		}
	}
}
