using Macrocosm.Common.Systems;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Bases.Machines
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
