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

namespace Macrocosm.Content.UI.Rocket
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
			string path = "Macrocosm/Content/UI/Rocket/";

			Texture2D zoomInButton = ModContent.Request<Texture2D>(path + "Buttons/ZoomIn", mode).Value;
			Texture2D zoomInBorder = ModContent.Request<Texture2D>(path + "Buttons/ZoomInBorder", mode).Value;
			Texture2D zoomOutButton = ModContent.Request<Texture2D>(path + "Buttons/ZoomOut", mode).Value;
			Texture2D zoomOutBorder = ModContent.Request<Texture2D>(path + "Buttons/ZoomOutBorder", mode).Value;

			Texture2D outlineSmall = ModContent.Request<Texture2D>(path + "Buttons/SelectionOutlineSmall", mode).Value;
			Texture2D outlineMedium = ModContent.Request<Texture2D>(path + "Buttons/SelectionOutlineMedium", mode).Value;
			Texture2D outlineLarge = ModContent.Request<Texture2D>(path + "Buttons/SelectionOutlineLarge", mode).Value;

			ZoomInButton = new(zoomInButton, zoomInBorder, new Vector2(6, 88));
			ZoomInButton.OnClick += (_, _) => ZoomIn();
			ZoomOutButton = new(zoomOutButton, zoomOutBorder, new Vector2(6, 118));
			ZoomOutButton.OnClick += (_, _) => ZoomOut();

			EarthSystem = new(ModContent.Request<Texture2D>(path + "Maps/EarthSystem", mode).Value);
			SolarSystemInner = new(ModContent.Request<Texture2D>(path + "Maps/SolarSystemInner", mode).Value, defaultNext: GetInitialNavigationMap());
			SolarSystemOuter = new(ModContent.Request<Texture2D>(path + "Maps/SolarSystemOuter", mode).Value, defaultNext: SolarSystemInner);

			EarthSystem.AddTarget(new UIMapTarget(new Vector2(64, 24), 160, 160, Earth.WorldInfo, () => SubworldSystem.AnyActive<Macrocosm>(), outline: outlineLarge)); 
			EarthSystem.AddTarget(new UIMapTarget(new Vector2(427, 33), 48, 48, Moon.Instance, outline: outlineMedium));
			EarthSystem.Prev = SolarSystemInner;

			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(247, 86), 32, 32, WorldInfoStorage.Sun, outline: outlineMedium));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(227, 88), 6, 6, WorldInfoStorage.Vulcan));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(303, 128), 6, 6, WorldInfoStorage.Mercury));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(165, 76), 6, 6, WorldInfoStorage.Venus));
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(367, 58), 6, 6, Earth.WorldInfo), EarthSystem);
			SolarSystemInner.AddTarget(new UIMapTarget(new Vector2(71, 122), 6, 6, WorldInfoStorage.Mars));
			SolarSystemInner.Prev = SolarSystemOuter;

			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(257, 96), 12, 12, WorldInfoStorage.InnerSolarSystem, outline: outlineMedium), SolarSystemInner);
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(282, 105), 9, 9, WorldInfoStorage.Jupiter));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(220, 115), 9, 9, WorldInfoStorage.Saturn));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(334, 71), 9, 9, WorldInfoStorage.Ouranos));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(182, 41), 9, 9, WorldInfoStorage.Neptune));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(410, 155), 9, 9, WorldInfoStorage.Pluto));
			SolarSystemOuter.AddTarget(new UIMapTarget(new Vector2(44, 19), 9, 9, WorldInfoStorage.Eris));
			SolarSystemOuter.Next = SolarSystemInner;
		}
	}
}
