using Macrocosm.Common.Enums;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class MachineUI : UIDragablePanel
    {
        public MachineTE MachineTE { get; set; }

        protected UIText title;
        protected UIHoverImageButton powerOnIcon;
        protected UIHoverImageButton powerOffIcon;

        public override void OnInitialize()
        {
            Width = new(640, 0);
            Height = new(480, 0);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.WindowStyle.BorderColor;
            SetPadding(6f);
            PaddingTop = 42f;

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
                Top = new(-42, 0),
                TextColor = Color.White
            };

            Append(title);

            if (MachineTE is ConsumerTE)
            {
                powerOnIcon = new(ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/LightningGreen", AssetRequestMode.ImmediateLoad))
                {
                    Left = new(-36, 1f),
                    Top = new(-34, 0f)
                };

                powerOffIcon = new(ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/LightningRed", AssetRequestMode.ImmediateLoad))
                {
                    Left = new(-36, 1f),
                    Top = new(-34, 0f)
                };
            }
            else if (MachineTE is GeneratorTE or BatteryTE)
            {
                powerOnIcon = new(ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/BatteryFull", AssetRequestMode.ImmediateLoad))
                {
                    Left = new(-50, 1f),
                    Top = new(-34, 0f)
                };

                powerOffIcon = new(ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/BatteryLow", AssetRequestMode.ImmediateLoad))
                {
                    Left = new(-50, 1f),
                    Top = new(-34, 0f)
                };
            }

            powerOnIcon.SetVisibility(0f);
            powerOffIcon.SetVisibility(1f);
            Append(powerOnIcon);
            Append(powerOffIcon);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (MachineTE is ConsumerTE consumer)
            {
                if (consumer.InputPower >= consumer.RequiredPower)
                {
                    powerOnIcon.SetVisibility(1f);
                    powerOffIcon.SetVisibility(0f);
                }
                else
                {
                    powerOnIcon.SetVisibility(0f);
                    powerOffIcon.SetVisibility(1f);
                }
            }
            else if (MachineTE is GeneratorTE or BatteryTE)
            {
                if (MachineTE.PoweredOn)
                {
                    powerOnIcon.SetVisibility(1f);
                    powerOffIcon.SetVisibility(0f);
                }
                else
                {
                    powerOnIcon.SetVisibility(0f);
                    powerOffIcon.SetVisibility(1f);
                }
            }
        }
    }
}
