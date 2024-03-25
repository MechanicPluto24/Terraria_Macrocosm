using Macrocosm.Common.Systems;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
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

        private UIText title;
        private UIDragablePanel window;

        public MachineUIState()
        {
        }

        private const string buttonsPath = "Macrocosm/Assets/Textures/UI/Buttons/";


        public override void OnInitialize()
        {
            window = new();
            window.Width.Set(535f, 0f);
            window.Height.Set(375f, 0f);
            window.HAlign = 0.5f;
            window.VAlign = 0.5f;
            window.SetPadding(6f);
            window.PaddingTop = 40f;

            window.BackgroundColor = UITheme.Current.WindowStyle.BackgroundColor;
            window.BorderColor = UITheme.Current.WindowStyle.BorderColor;

            Append(window);

            title = new(Language.GetText("Machine"), 0.6f, true)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.005f,
                Top = new(-34, 0),
                TextColor = Color.White
            };
            window.Append(title);
        }

        public void OnShow()
        {
            Deactivate();
            Activate();
            this.RemoveAllChildrenWhere((element) => element is MachineUI);
            window.Append(MachineUI);
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
