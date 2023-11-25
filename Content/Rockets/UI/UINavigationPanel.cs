using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.UI
{
    public class UINavigationPanel : UIPanel
    {
        public UINavigationMap CurrentMap;

        private UINavigationMap earthSystem;
        private UINavigationMap solarSystemInner;
        private UINavigationMap solarSystemOuter;

        private UIHoverImageButton zoomInButton;
        private UIHoverImageButton zoomOutButton;

        private UIPanel mapBorder;

        public UINavigationPanel()
        {
            WorldDataSystem.Instance.PropertyChanged += (sender, args) => UpdateMapVisibility(context: args.PropertyName);
        }

        public override void OnInitialize()
        {
            Width.Set(0, 0.71f);
            Height.Set(0, 0.335f);
            Top.Set(0, 0.01f);
            HAlign = 0.5f;
            SetPadding(0f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            mapBorder = new();
            mapBorder.Width.Set(0, 0.88f);
            mapBorder.Height.Set(0, 0.932f);
            mapBorder.HAlign = 0.5f;
            mapBorder.VAlign = 0.5f;
            mapBorder.SetPadding(0f);
            mapBorder.BackgroundColor = new Color(27, 36, 65);

            Append(mapBorder);

            InitializePanelContent();

            CurrentMap = GetInitialNavigationMap();

            var initialTarget = CurrentMap.Targets.FirstOrDefault(target => target.Name == MacrocosmSubworld.CurrentMacrocosmID);
            if (initialTarget is not null)
                initialTarget.Selected = true;

            Append(zoomInButton);
            Append(zoomOutButton);
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
            UINavigationTarget prevTarget = prevMap.GetSelectedTarget();
            CurrentMap.ResetAllTargets();
            RemoveChild(CurrentMap);

            CurrentMap = newMap;
            CurrentMap.ResetAllTargets();
            if (prevTarget is not null && CurrentMap.TryFindTargetByName(prevTarget.Name, out UINavigationTarget target))
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
            return earthSystem;

            //if(current is Mars or Phobos or Deimos)			
            //		return MarsSystem;

            // ...
        }

        private const string navigationMapsPath = "Macrocosm/Content/Rockets/Textures/NavigationMaps/";
        private const string buttonsPath = "Macrocosm/Assets/Textures/UI/Buttons/";

        private void UpdateMapVisibility(string context)
        {
            if (context is "FoundVulcan" or "Any")
            {
                if (WorldDataSystem.Instance.FoundVulcan)
                {
                    solarSystemInner.AddTarget(new UINavigationTarget(this, new Vector2(226, 88), 12, 12, "Vulcan"));
                    solarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInnerVulcan", AssetRequestMode.ImmediateLoad).Value;
                }
                else
                {
                    solarSystemInner.RemoveTargetByName("Vulcan");
                    solarSystemInner.Texture = ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInner", AssetRequestMode.ImmediateLoad).Value;
                }
            }
        }

        private void InitializePanelContent()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

            Asset<Texture2D> zoomInTexture = ModContent.Request<Texture2D>(buttonsPath + "ZoomInButton", mode);
            Asset<Texture2D> zoomOutTexture = ModContent.Request<Texture2D>(buttonsPath + "ZoomOutButton", mode);
            Asset<Texture2D> zoomButtonBorder = ModContent.Request<Texture2D>(buttonsPath + "ZoomButtonBorder", mode);

            Texture2D outlineSmall = ModContent.Request<Texture2D>(buttonsPath + "SelectionOutlineSmall", mode).Value;
            Texture2D outlineMedium = ModContent.Request<Texture2D>(buttonsPath + "SelectionOutlineMedium", mode).Value;
            Texture2D outlineLarge = ModContent.Request<Texture2D>(buttonsPath + "SelectionOutlineLarge", mode).Value;

            zoomInButton = new(zoomInTexture, zoomButtonBorder, Language.GetText("Mods.Macrocosm.UI.Common.ZoomIn"))
            {
                Top = new(0, 0.37f),
                Left = new(0, 0.011f),
            };
            zoomInButton.OnLeftClick += (_, _) => ZoomIn();
            zoomInButton.CheckInteractible = () => CurrentMap.HasNext || CurrentMap.HasDefaultNext;

            zoomOutButton = new(zoomOutTexture, zoomButtonBorder, Language.GetText("Mods.Macrocosm.UI.Common.ZoomOut"))
            {
                Top = new(0, 0.52f),
                Left = new(0, 0.011f),
            };
            zoomOutButton.OnLeftClick += (_, _) => ZoomOut();
            zoomOutButton.CheckInteractible = () => CurrentMap.HasPrev;

            earthSystem = new(ModContent.Request<Texture2D>(navigationMapsPath + "EarthSystem", mode).Value);
            solarSystemInner = new(ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemInner", mode).Value, defaultNext: GetInitialNavigationMap());
            solarSystemOuter = new(ModContent.Request<Texture2D>(navigationMapsPath + "SolarSystemOuter", mode).Value, defaultNext: solarSystemInner);

            earthSystem.AddTarget(new UINavigationTarget(this, new Vector2(64, 24), 160, 160, "Earth", Earth.LaunchConditions, outline: outlineLarge));
            earthSystem.AddTarget(new UINavigationTarget(this, new Vector2(424, 32), 48, 48, Moon.Instance, outline: outlineMedium));
            earthSystem.Prev = solarSystemInner;

            solarSystemInner.AddTarget(new UINavigationTarget(this, new Vector2(245, 85), 32, 32, "Sun", outline: outlineMedium));
            solarSystemInner.AddTarget(new UINavigationTarget(this, new Vector2(297, 123), 12, 12, "Mercury"));
            solarSystemInner.AddTarget(new UINavigationTarget(this, new Vector2(160, 72), 12, 12, "Venus"));
            solarSystemInner.AddTarget(new UINavigationTarget(this, new Vector2(361, 54), 12, 12, "Earth", Earth.LaunchConditions), earthSystem);
            solarSystemInner.AddTarget(new UINavigationTarget(this, new Vector2(67, 117), 12, 12, "Mars"));

            solarSystemInner.Prev = solarSystemOuter;

            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(255, 95), 12, 12, "Sun", outline: outlineMedium), solarSystemInner);
            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(277, 102), 12, 12, "Jupiter"));
            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(216, 111), 12, 12, "Saturn"));
            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(329, 68), 12, 12, "Ouranos"));
            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(178, 38), 12, 12, "Neptune"));
            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(405, 151), 12, 12, "Pluto"));
            solarSystemOuter.AddTarget(new UINavigationTarget(this, new Vector2(41, 17), 12, 12, "Eris"));

            solarSystemOuter.Next = solarSystemInner;

            UpdateMapVisibility("Any");
        }
    }
}
