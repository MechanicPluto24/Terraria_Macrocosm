using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI
{
    /// <summary> Panel wrapper for <see cref="UILiquid"/> with automatic hiding of overflow and (TODO) gradations </summary>
    public class UILiquidTank : UIPanel
    {
        private readonly LiquidType liquidType;

        private UILiquid uiLiquid;

        public UILiquidTank(LiquidType liquidType) : base
        (
            customBackground: ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/SquarePanelBackground"),
            customborder: ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/SquarePanelBorder"),
            customCornerSize: 12,
            customBarSize: 4
        )
        {
            this.liquidType = liquidType;
        }

        public float LiquidLevel { get; set; }

        public float WaveAmplitude { get; set; }

        public float WaveFrequency { get; set; }

        public override void OnInitialize()
        {
            BackgroundColor = UITheme.Current.ButtonStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
            OverflowHidden = true;
            SetPadding(2f);

            uiLiquid = new(liquidType)
            {
                Width = new(0, 1f),
                Height = new(0, 1f),
                RoundCorners = true
            };

            Append(uiLiquid);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            uiLiquid.LiquidLevel = LiquidLevel;
            uiLiquid.WaveAmplitude = WaveAmplitude;
            uiLiquid.WaveFrequency = WaveFrequency;
        }
    }
}
