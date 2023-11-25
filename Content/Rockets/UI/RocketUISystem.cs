using Macrocosm.Common.Config;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class RocketUISystem : ModSystem
    {
        public static RocketUISystem Instance => ModContent.GetInstance<RocketUISystem>();
        public UserInterface Interface { get; set; }
        public RocketUIState UIRocketState { get; set; }

        private GameTime lastGameTime;

        public static bool DebugModeActive { get; set; } = false;

        public static void Show(Rocket rocket) => Instance.ShowUI(rocket);
        public static void Hide() => Instance.HideUI();
        public static bool Active => Instance.Interface?.CurrentState is not null;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Interface = new UserInterface();
            MacrocosmConfig.Instance.OnConfigChanged += OnConfigChanged;
        }

        public override void Unload()
        {
            Interface = null;
        }

        public override void OnWorldLoad()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            UIRocketState = new RocketUIState();
            UIRocketState.Activate();
        }

        public override void OnWorldUnload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            UIRocketState?.Deactivate();
            UIRocketState = null;
        }

        public void ResetUI()
        {
            Rocket rocket = new();

            if (UIRocketState is not null && UIRocketState.Rocket is not null)
                rocket = UIRocketState.Rocket;

            UIRocketState = new RocketUIState();
            UIRocketState.Activate();

            if(Interface?.CurrentState != null)
            {
                HideUI();
                ShowUI(rocket);
            }        
        }

        private void OnConfigChanged(object sender, System.EventArgs e)
        {
            ResetUI();
        }

        public override void OnLocalizationsLoaded()
        {
        }

        public void ShowUI(Rocket rocket)
        {
            if (Main.netMode == NetmodeID.Server || Interface.CurrentState is not null)
                return;

            Main.playerInventory = true;

            UIRocketState.Rocket = rocket;
            UIRocketState.OnShow();

            Interface.SetState(UIRocketState);
        }

        public void HideUI()
        {
            UIRocketState?.OnHide();

            if (Main.netMode != NetmodeID.Server)
                Interface?.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // press Ctrl + Shift + E to reset UI
            if (Interface.CurrentState is not null &&
                Main.keyState.IsKeyDown(Keys.LeftControl) &&
                Main.keyState.IsKeyDown(Keys.LeftShift) &&
                Main.keyState.IsKeyDown(Keys.E) && !Main.oldKeyState.IsKeyDown(Keys.E))
            {
                //if(DebugModeActive) {
                UITheme.Reload();
                CustomizationStorage.Reset();
                // }

                ResetUI();
                Utility.Chat("Reset rocket UI", Color.Lime);
            }

            lastGameTime = gameTime;

            if (Interface?.CurrentState != null)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.mouseRightRelease = false;

                Interface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Macrocosm:RocketUI",
                    () =>
                    {
                        if (lastGameTime != null && Interface?.CurrentState != null)
                            Interface.Draw(Main.spriteBatch, lastGameTime);
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
