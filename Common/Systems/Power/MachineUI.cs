using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineUI : UIDragablePanel
    {
        public MachineTE MachineTE { get; set; }

        protected UIText title;
        protected UIPanel backgroundPanel;

        public override void OnInitialize()
        {
            Width = new(640, 0);
            Height = new(480, 0);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.WindowStyle.BorderColor;
            SetPadding(6f);
            PaddingTop = 40f;

            LocalizedText text = Language.GetOrRegister
            (
                $"Mods.Macrocosm.Machines.{MachineTE.MachineTile.GetType().Name}.DisplayName",
                () => Regex.Replace(MachineTE.MachineTile.GetType().Name, "([A-Z])", " $1").Trim()
            );

            title = new(text, 0.6f, true)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.035f,
                Top = new(-38, 0),
                TextColor = Color.White
            };

            Append(title);
        }
    }
}
