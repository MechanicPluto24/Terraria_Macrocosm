using Macrocosm.Common.UI;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
	public class UINavigationPanel : UIPanel
    {
        public UINavigationMap CurrentMap;

        private UINavigationMap EarthSystem;
        private UINavigationMap SolarSystemInner;
        private UINavigationMap SolarSystemOuter;

        private UIHoverImageButton ZoomInButton;
        private UIHoverImageButton ZoomOutButton;

        private UIPanel MapBorder;

        public UINavigationPanel()
        {
            WorldDataSystem.Instance.PropertyChanged += (sender, args) => UpdateMapVisibility(args.PropertyName);
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
        public void ZoomOut() => UpdateCurrentMap(CurrentMap.Prev);


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
		private const string navigationMapsPath = rocketPath + "Textures/NavigationMaps/";
		private const string buttonsPath = rocketPath + "Textures/Buttons/";

		private void UpdateMapVisibility(string context)
        {
            if (context is "FoundVulcan" or "Any")
            {
                if (WorldDataSystem.Instance.FoundVulcan)
                {
					SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(226, 88), 12, 12, "Vulcan"));
					SolarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInnerVulcan", AssetRequestMode.ImmediateLoad).Value;
                }
                else
                {
                    SolarSystemInner.RemoveTargetByName("Vulcan");
                    SolarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInner", AssetRequestMode.ImmediateLoad).Value;
                }
            }
		}

        private void InitializePanelContent()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

            Asset<Texture2D> zoomInButton = ModContent.Request<Texture2D>(buttonsPath + "ZoomInButton", mode);
            Asset<Texture2D> zoomOutButton = ModContent.Request<Texture2D>(buttonsPath + "ZoomOutButton", mode);
            Asset<Texture2D> zoomButtonBorder = ModContent.Request<Texture2D>(buttonsPath + "ZoomButtonBorder", mode);

            Texture2D outlineSmall = ModContent.Request<Texture2D>(buttonsPath + "SelectionOutlineSmall", mode).Value;
            Texture2D outlineMedium = ModContent.Request<Texture2D>(buttonsPath + "SelectionOutlineMedium", mode).Value;
            Texture2D outlineLarge = ModContent.Request<Texture2D>(buttonsPath + "SelectionOutlineLarge", mode).Value;

            ZoomInButton = new(zoomInButton, zoomButtonBorder, Language.GetText("Mods.Macrocosm.UI.Common.ZoomIn"))
            {
                Top = StyleDimension.FromPercent(0.37f),
				Left = StyleDimension.FromPercent(0.011f),
			};
            ZoomInButton.OnLeftClick += (_, _) => ZoomIn();
            ZoomInButton.CheckInteractible = () => CurrentMap.HasNext || CurrentMap.HasDefaultNext;

            ZoomOutButton = new(zoomOutButton, zoomButtonBorder, Language.GetText("Mods.Macrocosm.UI.Common.ZoomOut"))
            {
				Top = StyleDimension.FromPercent(0.52f),
				Left = StyleDimension.FromPercent(0.011f),
			};
			ZoomOutButton.OnLeftClick += (_, _) => ZoomOut();
            ZoomOutButton.CheckInteractible = () => CurrentMap.HasPrev;

			EarthSystem = new(ModContent.Request<Texture2D>(navigationMapsPath + "EarthSystem", mode).Value);
            SolarSystemInner = new(ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInner", mode).Value, defaultNext: GetInitialNavigationMap());
            SolarSystemOuter = new(ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemOuter", mode).Value, defaultNext: SolarSystemInner);

			EarthSystem.AddTarget(new UIMapTarget(this, new Vector2(64, 24), 160, 160, "Earth", Earth.LaunchConditions, outline: outlineLarge));
			EarthSystem.AddTarget(new UIMapTarget(this, new Vector2(424, 32), 48, 48, Moon.Instance, outline: outlineMedium));
			EarthSystem.Prev = SolarSystemInner;

			SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(245, 85), 32, 32, "Sun", outline: outlineMedium));
			SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(297, 123), 12, 12, "Mercury"));
			SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(160, 72), 12, 12, "Venus"));
			SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(361, 54), 12, 12, "Earth", Earth.LaunchConditions), EarthSystem);
			SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(67, 117), 12, 12, "Mars"));

			SolarSystemInner.Prev = SolarSystemOuter;

			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(255, 95), 12, 12, "Sun", outline: outlineMedium), SolarSystemInner);
			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(277, 102), 12, 12, "Jupiter"));
			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(216, 111), 12, 12, "Saturn"));
			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(329, 68), 12, 12, "Ouranos"));
			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(178, 38), 12, 12, "Neptune"));
			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(405, 151), 12, 12, "Pluto"));
			SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(41, 17), 12, 12, "Eris"));

			SolarSystemOuter.Next = SolarSystemInner;

            UpdateMapVisibility("Any");
		}
    }
}
