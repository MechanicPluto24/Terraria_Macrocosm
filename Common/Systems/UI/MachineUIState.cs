using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace Macrocosm.Common.Systems.UI
{
    public class MachineUIState : UIState
    {
        public MachineUI MachineUI;

        public MachineUIState()
        {
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

            if (Main.LocalPlayer.UICloseConditions())
                UISystem.Hide();
        }
    }
}
