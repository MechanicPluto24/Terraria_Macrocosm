using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    /// <summary> Panel wrapper for <see cref="UILiquid"/> with automatic hiding of overflow and (TODO) gradations </summary>
    public class UILiquidTank : UIPanel
    {
        // temporary
        private readonly LiquidType? macrocosmLiquidType;

        private readonly int liquidType;

        private UILiquid uiLiquid;

        /// <summary> Use <see cref="WaterStyleID"/>! </summary>
        public UILiquidTank(int liquidType) : base
        (
            customBackground: ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/SquarePanelBackground"),
            customborder: ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/SquarePanelBorder"),
            customCornerSize: 12,
            customBarSize: 4
        )
        {
            this.liquidType = liquidType;
        }

        public UILiquidTank(LiquidType macrocosmLiquidType) : this(0)
        {
            this.macrocosmLiquidType = macrocosmLiquidType;
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

            if (macrocosmLiquidType.HasValue)
                uiLiquid = new(macrocosmLiquidType.Value);
            else 
                uiLiquid = new(liquidType);

            uiLiquid.Width = new(0, 1f);
            uiLiquid.Height = new(0, 1f);
            uiLiquid.RoundCorners = true;

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
