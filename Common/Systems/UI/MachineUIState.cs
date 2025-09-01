using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.UI;

namespace Macrocosm.Common.Systems.UI
{
    public class MachineUIState : UIState
    {
        public MachineUI MachineUI { get; }
        public MachineUIState(MachineUI machineUI)
        {
            MachineUI = machineUI;
        }

        public override void OnInitialize()
        {
        }

        public void OnShow()
        {
            Deactivate();
            Activate();

            this.RemoveAllChildrenWhere((element) => element is MachineUI);
            Append(MachineUI);
        }

        public void OnHide()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var machine = MachineUI.MachineTE;
            Rectangle rect = new(machine.Position.X * 16, machine.Position.Y * 16, machine.MachineTile.Width * 16, machine.MachineTile.Height * 16);
            if (Main.LocalPlayer.UICloseConditions() || !rect.InPlayerInteractionRange(TileReachCheckSettings.Simple))
                UISystem.Hide();
        }
    }
}
