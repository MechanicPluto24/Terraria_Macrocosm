using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Macrocosm.Content.Subworlds;
using Macrocosm.Common.UI;
using Terraria.Localization;
using Macrocosm.Content.Systems;
using System.ComponentModel;
using System.IO;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UINavigationPanel : UIPanel
    {
        public UINavigationMap CurrentMap;

        private UINavigationMap EarthSystem;
        private UINavigationMap SolarSystemInner;
        private UINavigationMap SolarSystemInnerVulcan;
        private UINavigationMap SolarSystemOuter;

        private UIHoverImageButton ZoomInButton;
        private UIHoverImageButton ZoomOutButton;

        private UIPanel MapBorder;

        public UINavigationPanel()
        {
            WorldDataSystem.Instance.PropertyChanged += (sender, args) => UpdateVulcanVisibility(args.PropertyName);
        }

		public override void OnInitialize()
        {
            Width.Set(0, 0.71f);
            Height.Set(0, 0.335f);
			Top.Set(0, 0.01f);
			HAlign = 0.5f;
            SetPadding(0f);
            BackgroundColor = new Color(53, 72, 135);
			BorderColor = new Color(89, 116, 213, 255);

            MapBorder = new();
            MapBorder.Width.Set(0, 0.88f);
			MapBorder.Height.Set(0, 0.932f);
			MapBorder.HAlign = 0.5f;
			MapBorder.VAlign = 0.5f;
            MapBorder.SetPadding(0f);
            MapBorder.BackgroundColor = new Color(27, 36, 65);

			Append(MapBorder);

			InitializePanelContent();

            CurrentMap = GetInitialNavigationMap();

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
			UIMapTarget prevTarget = prevMap.GetSelectedTarget();
            CurrentMap.ResetAllTargets();
            RemoveChild(CurrentMap);

            CurrentMap = newMap;
			CurrentMap.ResetAllTargets();
			if (prevTarget is not null && CurrentMap.TryFindTargetByName(prevTarget.Name, out UIMapTarget target))
 				target.Selected = true;

            CurrentMap.ShowAnimation(prevMap);

			Append(CurrentMap);
			Activate();
        }

        /// <summary> This method determines the default navigation map based on the current subworld </summary>
        private UINavigationMap GetInitialNavigationMap()
        {
            //MacrocosmSubworld current = MacrocosmSubworld.Current;

            // if(current is Moon || MacrocosmSubworld.EarthActive)
            return EarthSystem;

            //if(current is Mars or Phobos or Deimos)			
            //		return MarsSystem;

            // ...
        }

        private const string rocketPath = "Macrocosm/Content/Rockets/";
		private const string navigationMapsPath = rocketPath + "Navigation/NavigationMaps/";

		private void UpdateVulcanVisibility(string context)
        {
            if (context != "FoundVulcan")
                return;

            if (WorldDataSystem.Instance.FoundVulcan)
            {
				//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(226, 88), 6, 6, "Vulcan"));
                SolarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInnerVulcan", AssetRequestMode.ImmediateLoad).Value;
			}
            else
            {
                //SolarSystemInner.RemoveTargetByName("Vulcan");
				SolarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInner", AssetRequestMode.ImmediateLoad).Value;
			}
		}

        private void InitializePanelContent()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

            Asset<Texture2D> zoomInButton = ModContent.Request<Texture2D>(rocketPath + "Buttons/ZoomInButton", mode);
            Asset<Texture2D> zoomOutButton = ModContent.Request<Texture2D>(rocketPath + "Buttons/ZoomOutButton", mode);
            Asset<Texture2D> zoomButtonBorder = ModContent.Request<Texture2D>(rocketPath + "Buttons/ZoomButtonBorder", mode);

            Texture2D outlineSmall = ModContent.Request<Texture2D>(rocketPath + "Buttons/SelectionOutlineSmall", mode).Value;
            Texture2D outlineMedium = ModContent.Request<Texture2D>(rocketPath + "Buttons/SelectionOutlineMedium", mode).Value;
            Texture2D outlineLarge = ModContent.Request<Texture2D>(rocketPath + "Buttons/SelectionOutlineLarge", mode).Value;

            ZoomInButton = new(zoomInButton, zoomButtonBorder, Language.GetText("Mods.Macrocosm.RocketUI.Common.ZoomIn"))
            {
                Top = StyleDimension.FromPercent(0.37f),
				Left = StyleDimension.FromPercent(0.011f),
			};
            ZoomInButton.OnLeftClick += (_, _) => ZoomIn();
            ZoomInButton.CheckInteractible = () => CurrentMap.HasNext || CurrentMap.HasDefaultNext;

            ZoomOutButton = new(zoomOutButton, zoomButtonBorder, Language.GetText("Mods.Macrocosm.RocketUI.Common.ZoomOut"))
            {
				Top = StyleDimension.FromPercent(0.52f),
				Left = StyleDimension.FromPercent(0.011f),
			};
			ZoomOutButton.OnLeftClick += (_, _) => ZoomOut();
            ZoomOutButton.CheckInteractible = () => CurrentMap.HasPrev;

			EarthSystem = new(ModContent.Request<Texture2D>(navigationMapsPath + "EarthSystem", mode).Value);
            SolarSystemInner = new(ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInner", mode).Value, defaultNext: GetInitialNavigationMap());
            SolarSystemOuter = new(ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemOuter", mode).Value, defaultNext: SolarSystemInner);
	
			EarthSystem.AddTarget(new UIMapTarget(this, 0.12f, 0.1f, 0.308f, 0.795f , "Earth", Earth.LaunchConditions, outline: outlineLarge));
            EarthSystem.AddTarget(new UIMapTarget(this, 0.812f, 0.161f,  0.088f, 0.23f, Moon.Instance, outline: outlineMedium));
            EarthSystem.Prev = SolarSystemInner;

			//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(246, 86), 32, 32, "Sun", outline: outlineMedium));
			//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(302, 128), 6, 6, "Mercury"));
			//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(164, 76), 6, 6, "Venus"));
			//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(366, 58), 6, 6, "Earth", Earth.LaunchConditions), EarthSystem);
			//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(70, 122), 6, 6, "Mars"));
			if (WorldDataSystem.Instance.FoundVulcan)
			{
				//SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(226, 88), 6, 6, "Vulcan"));
				SolarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInnerVulcan", AssetRequestMode.ImmediateLoad).Value;
			}
			SolarSystemInner.Prev = SolarSystemOuter;

            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(256, 96), 12, 12, "Sun", outline: outlineMedium), SolarSystemInner);
            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(280.5f, 104.5f), 9, 9, "Jupiter"));
            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(219, 114), 9, 9, "Saturn"));
            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(333, 70), 9, 9, "Ouranos"));
            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(181, 40), 9, 9, "Neptune"));
            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(409, 154), 9, 9, "Pluto"));
            //SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(43, 18), 9, 9, "Eris"));
            SolarSystemOuter.Next = SolarSystemInner;
        }
    }
}
