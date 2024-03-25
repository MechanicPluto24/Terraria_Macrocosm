using Macrocosm.Common.Bases.Machines;
using Macrocosm.Common.Config;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Systems
{
    public class UISystem : ModSystem
    {
        public static UISystem Instance => ModContent.GetInstance<UISystem>();
        public UserInterface UserInterface { get; set; }

        public RocketUIState UIRocketState { get; set; }
        public AssemblyUIState UIAssemblyState { get; set; }
        public MachineUIState UIMachineState { get; set; }

        private GameTime lastGameTime;
        public static bool DebugModeActive { get; set; } = false;

        public static void ShowRocketUI(Rocket rocket) => Instance.ShowRocketUI_Internal(rocket);
        public static void ShowAssemblyUI(LaunchPad launchPad) => Instance.ShowAssemblyUI_Internal(launchPad);
        public static void ShowMachineUI(MachineTE machineTileEntity, MachineUI machineUI) => Instance.ShowMachineUI_Internal(machineTileEntity, machineUI);
        public static void Hide() => Instance.HideUI();
        public static bool Active => Instance.UserInterface?.CurrentState is not null;
        public static bool RocketUIActive => Instance.UserInterface?.CurrentState is RocketUIState;
        public static bool AssemblyUIActive => Instance.UserInterface?.CurrentState is AssemblyUIState;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            UserInterface = new UserInterface();
            MacrocosmConfig.Instance.OnConfigChanged += OnConfigChanged;
        }

        public override void Unload()
        {
            UserInterface = null;
        }

        public override void OnWorldLoad()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            UIRocketState = new RocketUIState();
            UIRocketState.Activate();

            UIAssemblyState = new AssemblyUIState();
            UIAssemblyState.Activate();
        }

        public override void OnWorldUnload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Required for calling OnTabClose
            HideUI();

            UIRocketState?.Deactivate();
            UIRocketState = null;

            UIAssemblyState?.Deactivate();
            UIAssemblyState = null;
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (UserInterface.CurrentState is not null && UIRocketState is not null && UIRocketState.Rocket is not null)
                UIRocketState.ExecuteRecursively(RecursiveUpdate);

            if (UserInterface.CurrentState is not null && UIAssemblyState is not null && UIAssemblyState.LaunchPad is not null)
                UIAssemblyState.ExecuteRecursively(RecursiveUpdate);
        }

        public void ResetUI()
        {
            if (UIRocketState is not null)
            {
                Rocket rocket = new();
                if (UIRocketState.Rocket is not null)
                    rocket = UIRocketState.Rocket;

                UIRocketState = new RocketUIState();
                UIRocketState.Activate();

                if (UserInterface?.CurrentState != null)
                {
                    HideUI();
                    ShowRocketUI_Internal(rocket);
                }
            }

            if (UIAssemblyState is not null)
            {
                LaunchPad launchPad = new();
                if (UIAssemblyState.LaunchPad is not null)
                    launchPad = UIAssemblyState.LaunchPad;

                UIAssemblyState = new AssemblyUIState();
                UIAssemblyState.Activate();

                if (UserInterface?.CurrentState != null)
                {
                    HideUI();
                    ShowAssemblyUI_Internal(launchPad);
                }
            }
        }

        private void OnConfigChanged(object sender, System.EventArgs e)
        {
            ResetUI();
        }

        public override void OnLocalizationsLoaded()
        {
        }

        private void ShowRocketUI_Internal(Rocket rocket)
        {
            if (Main.netMode == NetmodeID.Server || UserInterface.CurrentState is not null)
                return;

            Main.playerInventory = true;

            UIRocketState.Rocket = rocket;
            UIRocketState.OnShow();

            UserInterface.SetState(UIRocketState);
        }

        private void ShowAssemblyUI_Internal(LaunchPad launchPad)
        {
            if (Main.netMode == NetmodeID.Server || UserInterface.CurrentState is not null)
                return;

            Main.playerInventory = true;

            UIAssemblyState.LaunchPad = launchPad;
            UIAssemblyState.OnShow();

            UserInterface.SetState(UIAssemblyState);
        }

        private void ShowMachineUI_Internal(MachineTE machineTileEntity, MachineUI machineUI)
        {
            if (Main.netMode == NetmodeID.Server || UserInterface.CurrentState is not null)
                return;

            Main.playerInventory = true;

            machineUI.MachineTE = machineTileEntity;
            UIMachineState = new();
            UIMachineState.MachineUI = machineUI;
            UIMachineState.OnShow();
            UIMachineState.Activate();

            UserInterface.SetState(UIMachineState);
        }

        public void HideUI()
        {
            UIRocketState?.OnHide();
            UIAssemblyState?.OnHide();

            if (Main.netMode != NetmodeID.Server)
                UserInterface?.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // press Ctrl + Shift + E to reset UI
            if (UserInterface.CurrentState is not null &&
                Main.keyState.IsKeyDown(Keys.LeftControl) &&
                Main.keyState.IsKeyDown(Keys.LeftShift) &&
                Main.keyState.IsKeyDown(Keys.E) && !Main.oldKeyState.IsKeyDown(Keys.E))
            {
                //if(DebugModeActive) {
                UITheme.Reload();
                CustomizationStorage.Reset();
                // }

                ResetUI();
                Utility.Chat("Reset UI", Color.Lime);
            }

            lastGameTime = gameTime;

            if (UserInterface?.CurrentState != null)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.mouseRightRelease = false;

                UserInterface.Update(gameTime);
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
                        if (lastGameTime != null && UserInterface?.CurrentState != null)
                            UserInterface.Draw(Main.spriteBatch, lastGameTime);
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        private void RecursiveUpdate(UIElement element)
        {
            if (element is null)
                return;

            if (element is IConsistentUpdateable updateable)
                updateable.Update();
        }
    }
}
