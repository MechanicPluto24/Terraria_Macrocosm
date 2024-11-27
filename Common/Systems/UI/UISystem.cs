using Macrocosm.Common.Config;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Systems.UI
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
            ClientConfig.Instance.OnConfigChanged += OnConfigChanged;
        }

        public override void Unload()
        {
            UserInterface = null;
        }

        public override void OnWorldLoad()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            /*
            UIRocketState = new RocketUIState();
            UIRocketState.Activate();

            UIAssemblyState = new AssemblyUIState();
            UIAssemblyState.Activate();
            */
        }

        public override void OnWorldUnload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Required for calling OnTabClose
            HideUI();

            /*
            UIRocketState?.Deactivate();
            UIRocketState = null;

            UIAssemblyState?.Deactivate();
            UIAssemblyState = null;
            */
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

        private void OnConfigChanged(object sender, EventArgs e)
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

            UIRocketState = new();
            UIRocketState.Rocket = rocket;
            UIRocketState.Initialize();
            UIRocketState.OnShow();
            UIRocketState.Activate();
            UserInterface.SetState(UIRocketState);
        }

        private void ShowAssemblyUI_Internal(LaunchPad launchPad)
        {
            if (Main.netMode == NetmodeID.Server || UserInterface.CurrentState is not null)
                return;

            if (UIAssemblyState is not null && UIAssemblyState.LaunchPad is not null && !(UIAssemblyState.LaunchPad.Inventory.InteractingPlayer == Main.myPlayer || UIAssemblyState.LaunchPad.Inventory.InteractingPlayer == 255))
                return;

            UIAssemblyState = new();
            UIAssemblyState.LaunchPad = launchPad;
            UIAssemblyState.LaunchPad.Inventory.InteractingPlayer = Main.myPlayer;

            UIAssemblyState.Initialize();
            UIAssemblyState.OnShow();
            UIAssemblyState.Activate();

            UserInterface.SetState(UIAssemblyState);

            Main.playerInventory = true;
        }

        private void ShowMachineUI_Internal(MachineTE machineTileEntity, MachineUI machineUI)
        {
            if (Main.netMode == NetmodeID.Server || UserInterface.CurrentState is not null)
                return;

            if
            (
                UIMachineState is not null
                && UIMachineState.MachineUI.MachineTE is IInventoryOwner inventoryOwner
                && !(inventoryOwner.Inventory.InteractingPlayer == Main.myPlayer || inventoryOwner.Inventory.InteractingPlayer == 255)
            )
            {
                return;
            }

            machineUI.MachineTE = machineTileEntity;
            UIMachineState = new();
            UIMachineState.MachineUI = machineUI;
            if (UIMachineState.MachineUI.MachineTE is IInventoryOwner inventoryOwner1)
            {
                Main.stackSplit = 600;
                inventoryOwner1.Inventory.InteractingPlayer = Main.myPlayer;
            }

            UIMachineState.Initialize();
            UIMachineState.OnShow();
            UIMachineState.Activate();

            UserInterface.SetState(UIMachineState);

            Main.playerInventory = true;
        }

        public void HideUI()
        {
            UIRocketState?.OnHide();
            UIAssemblyState?.OnHide();
            UIMachineState?.OnHide();

            if (Main.netMode != NetmodeID.Server)
                UserInterface?.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Inventory.ActiveInventory = null;

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

            if (element is IFixedUpdateable updateable)
                updateable.FixedUpdate();
        }
    }
}
