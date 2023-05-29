using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rocket;
using Macrocosm.Content.Rocket.Navigation.InfoElements;
using Macrocosm.Content.Rocket.Navigation.LaunchChecklist;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.Navigation
{
    public class UIRocket : UIState
    {
        public int RocketID { get; set; } = -1;

        private RocketNPC Rocket => Main.npc[RocketID].ModNPC as RocketNPC;

        UIPanel UIBackgroundPanel;
        UILaunchButton UILaunchButton;
        UINavigationPanel UINavigationPanel;
        UIInfoPanel UIWorldInfoPanel;

        UIFlightChecklist UIFlightChecklist;

		private ChecklistCondition selectedLaunchCondition = new("Selected", () => false);
		private ChecklistCondition hereLaunchCondition = new("NotHere", () => false);
		private LaunchConditions genericLaunchConditions = new();

		public static void Show(int rocketId) => RocketSystem.Instance.ShowUI(rocketId);
        public static void Hide() => RocketSystem.Instance.HideUI();
        public static bool Active => RocketSystem.Instance.Interface?.CurrentState is not null;

        public override void OnInitialize()
        {
            UIBackgroundPanel = new();
            UIBackgroundPanel.Width.Set(855, 0f);
            UIBackgroundPanel.Height.Set(680, 0f);
            UIBackgroundPanel.HAlign = 0.5f;
            UIBackgroundPanel.VAlign = 0.5f;
            UIBackgroundPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(UIBackgroundPanel);

            UINavigationPanel = new();
            UIBackgroundPanel.Append(UINavigationPanel);

            UIWorldInfoPanel = new("");
            UIBackgroundPanel.Append(UIWorldInfoPanel);

            UIFlightChecklist = new(Language.GetTextValue("Mods.Macrocosm.WorldInfo.Common.Checklist"));
            UIBackgroundPanel.Append(UIFlightChecklist);

            UILaunchButton = new();
            UILaunchButton.ZoomIn = UINavigationPanel.ZoomIn;
            UILaunchButton.Launch = LaunchRocket;
            UIBackgroundPanel.Append(UILaunchButton);

			selectedLaunchCondition = new("Selected", () => target is not null);
		    hereLaunchCondition = new("NotHere", () => target is not null && !target.AlreadyHere);

		    genericLaunchConditions.Add(new ChecklistCondition("Fuel", CheckFuel));
            genericLaunchConditions.Add(new ChecklistCondition("Obstruction", () => Rocket.CheckFlightPathObstruction()));
		}

		public override void OnDeactivate()
        {
        }

        private UIMapTarget lastTarget;
        private UIMapTarget target;
        public override void Update(GameTime gameTime)
        {
            // Don't delete this or the UIElements attached to this UIState will cease to function
            base.Update(gameTime);

            Player player = Main.LocalPlayer;
            player.mouseInterface = true;

            if (!Rocket.NPC.active || player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !Main.playerInventory ||
                !player.InInteractionRange((int)Rocket.NPC.Center.X / 16, (int)Rocket.NPC.Center.Y / 16, TileReachCheckSettings.Simple) || Rocket.Launching || player.controlMount)
            {
                Hide();
                return;
            }

            lastTarget = target;
            target = UINavigationPanel.CurrentMap.GetSelectedTarget();
            player.RocketPlayer().TargetSubworldID = target is null ? "" : target.TargetID;


            GetInfoPanel();
            UpdateChecklist();
            UpdateLaunchButton();
        }

        bool initialState = false;
        private void GetInfoPanel()
        {
            // Initial state of the info panel; default to the current subworld
            if (target is null)
            {
                if (!initialState)
                {
					initialState = true;
					UIBackgroundPanel.RemoveChild(UIWorldInfoPanel);
					UIWorldInfoPanel = WorldInfoDatabase.GetValue(MacrocosmSubworld.SafeCurrentID).ProvideUI();
					UIBackgroundPanel.Append(UIWorldInfoPanel);
				}

                if(lastTarget is not null)
                {
					UIWorldInfoPanel.Remove();
					UIWorldInfoPanel = new UIInfoPanel("");
					UIBackgroundPanel.Append(UIWorldInfoPanel);
				}
               
			}

            // Update the info panel on new target 
            if (target is not null && target != lastTarget)
            {
				UIBackgroundPanel.RemoveChild(UIWorldInfoPanel);
				UIWorldInfoPanel = WorldInfoDatabase.GetValue(target.TargetID).ProvideUI();
				UIBackgroundPanel.Append(UIWorldInfoPanel);
			} 

			// variant that removes the target on deselection or navigating to the next map
			/*
				WorldInfoPanel.Remove();
				WorldInfoPanel = (target is not null) ? WorldInfoDatabase.GetValue(target.TargetID).ProvideUI() : new UIInfoPanel("");
				BackgroundPanel.Append(WorldInfoPanel);
			*/
		}


        private void UpdateChecklist()
        {
			UIFlightChecklist.ClearInfo();

			if (!selectedLaunchCondition.IsMet())
                UIFlightChecklist.Add(selectedLaunchCondition.ProvideUI(ChecklistInfoElement.IconType.QuestionMark));
 			else if(!hereLaunchCondition.IsMet()) // target is definitely not null
 				UIFlightChecklist.Add(hereLaunchCondition.ProvideUI(ChecklistInfoElement.IconType.GrayCrossmark));
            else
            {
                if (target.LaunchConditions is not null)
                    UIFlightChecklist.AddList(LaunchConditions.Merge(target.LaunchConditions, genericLaunchConditions).ProvideList());
                else
                    UIFlightChecklist.AddList(genericLaunchConditions.ProvideList());
			}
		}


        private void UpdateLaunchButton()
        {
            if (target is null)
                UILaunchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (UINavigationPanel.CurrentMap.Next != null)
                UILaunchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (target.AlreadyHere)
                UILaunchButton.ButtonState = UILaunchButton.StateType.AlreadyHere;
            else if (!target.CheckLaunchConditions())
                UILaunchButton.ButtonState = UILaunchButton.StateType.CantReach;
            else
                UILaunchButton.ButtonState = UILaunchButton.StateType.Launch;
        }

        private void LaunchRocket()
        {
            Rocket.Launch(); // launch rocket on the local sp/mp client

            if (Main.netMode == NetmodeID.MultiplayerClient)
                Rocket.SendLaunchMessage(); // send launch message to the server
        }

        private bool CheckFuel()
        {
            return Rocket.Fuel >= RocketFuelLookup.GetFuelCost(MacrocosmSubworld.SafeCurrentID, target.TargetID);
        }
    }
}
