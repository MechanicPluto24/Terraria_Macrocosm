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
        private UINavigationMap SolarSystemOuter;

        private UIHoverImageButton ZoomInButton;
        private UIHoverImageButton ZoomOutButton;

        private UIPanel MapBorder;

        public UINavigationPanel()
        {
            Initialize();
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
					SolarSystemInner.AddTarget(new UIMapTarget(this, 0.4361f, 0.445f, 0.1f, "Vulcan"));
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
	
			EarthSystem.AddTarget(new UIMapTarget(this, 0.276f, 0.511f, 0.795f , "Earth", Earth.LaunchConditions, outline: outlineLarge));
            EarthSystem.AddTarget(new UIMapTarget(this, 0.85786f, 0.2818f, 0.25f, Moon.Instance, outline: outlineMedium));
            EarthSystem.Prev = SolarSystemInner;

			SolarSystemInner.AddTarget(new UIMapTarget(this, 0.499f, 0.5f, 0.2f, "Sun", outline: outlineMedium));
			SolarSystemInner.AddTarget(new UIMapTarget(this, 0.581f, 0.639f, 0.1f, "Mercury"));
			SolarSystemInner.AddTarget(new UIMapTarget(this, 0.319f, 0.385f, 0.1f, "Venus"));
			SolarSystemInner.AddTarget(new UIMapTarget(this, 0.702f, 0.295f, 0.1f, "Earth", Earth.LaunchConditions), EarthSystem);
			SolarSystemInner.AddTarget(new UIMapTarget(this, 0.14f, 0.61f, 0.1f, "Mars"));
			SolarSystemInner.Prev = SolarSystemOuter;

            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.5f, 0.5f, 0.1f, "Sun", outline: outlineMedium), SolarSystemInner);
            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.4255f, 0.5837f, 0.1f, "Jupiter"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.5423f, 0.534f, 0.1f, "Saturn"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.641f, 0.3635f, 0.1f, "Ouranos"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.353f, 0.224f, 0.1f, "Neptune"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.7855f, 0.7787f, 0.1f, "Pluto"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, 0.09f, 0.115f, 0.1f, "Eris"));
            SolarSystemOuter.Next = SolarSystemInner;

            UpdateMapVisibility("Any");
		}
    }
}
