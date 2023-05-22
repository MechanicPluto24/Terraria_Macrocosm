using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;

namespace Macrocosm.Content.Rocket.Navigation
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

            //UIMapTarget prevTarget = prevMap.GetSelectedTarget();
            //if (prevTarget is not null && CurrentMap.TryFindTargetBy(prevTarget.TargetID, out UIMapTarget target))
            //	target.Selected = true;

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

        private void InitializePanelContent()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            string path = "Macrocosm/Content/Rocket/Navigation/";

            Texture2D zoomInButton = ModContent.Request<Texture2D>(path + "Buttons/ZoomIn", mode).Value;
            Texture2D zoomInBorder = ModContent.Request<Texture2D>(path + "Buttons/ZoomInBorder", mode).Value;
            Texture2D zoomOutButton = ModContent.Request<Texture2D>(path + "Buttons/ZoomOut", mode).Value;
            Texture2D zoomOutBorder = ModContent.Request<Texture2D>(path + "Buttons/ZoomOutBorder", mode).Value;

            Texture2D outlineSmall = ModContent.Request<Texture2D>(path + "Buttons/SelectionOutlineSmall", mode).Value;
            Texture2D outlineMedium = ModContent.Request<Texture2D>(path + "Buttons/SelectionOutlineMedium", mode).Value;
            Texture2D outlineLarge = ModContent.Request<Texture2D>(path + "Buttons/SelectionOutlineLarge", mode).Value;

            ZoomInButton = new(zoomInButton, zoomInBorder, new Vector2(6, 88));
            ZoomInButton.OnLeftClick += (_, _) => ZoomIn();
            ZoomOutButton = new(zoomOutButton, zoomOutBorder, new Vector2(6, 118));
            ZoomOutButton.OnLeftClick += (_, _) => ZoomOut();

            EarthSystem = new(ModContent.Request<Texture2D>(path + "NavigationMaps/EarthSystem", mode).Value);
            SolarSystemInner = new(ModContent.Request<Texture2D>(path + "NavigationMaps/SolarSystemInner", mode).Value, defaultNext: GetInitialNavigationMap());
            SolarSystemOuter = new(ModContent.Request<Texture2D>(path + "NavigationMaps/SolarSystemOuter", mode).Value, defaultNext: SolarSystemInner);

            EarthSystem.AddTarget(new UIMapTarget(this, new Vector2(64, 24), 160, 160, "Earth", Earth.LaunchConditions, outline: outlineLarge));
            EarthSystem.AddTarget(new UIMapTarget(this, new Vector2(427, 33), 48, 48, Moon.Instance, outline: outlineMedium));
            EarthSystem.Prev = SolarSystemInner;

            SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(246, 86), 32, 32, "Sun", outline: outlineMedium));
            SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(226, 88), 6, 6, "Vulcan"));
            SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(302, 128), 6, 6, "Mercury"));
            SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(164, 76), 6, 6, "Venus"));
            SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(366, 58), 6, 6, "Earth"), EarthSystem);
            SolarSystemInner.AddTarget(new UIMapTarget(this, new Vector2(70, 122), 6, 6, "Mars"));
            SolarSystemInner.Prev = SolarSystemOuter;

            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(256, 96), 12, 12, "InnerSolarSystem", outline: outlineMedium), SolarSystemInner);
            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(281, 104), 9, 9, "Jupiter"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(219, 114), 9, 9, "Saturn"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(333, 70), 9, 9, "Ouranos"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(181, 40), 9, 9, "Neptune"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(409, 154), 9, 9, "Pluto"));
            SolarSystemOuter.AddTarget(new UIMapTarget(this, new Vector2(43, 18), 9, 9, "Eris"));
            SolarSystemOuter.Next = SolarSystemInner;
        }
    }
}
