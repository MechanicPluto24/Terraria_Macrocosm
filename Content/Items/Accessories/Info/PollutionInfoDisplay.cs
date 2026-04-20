using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info;

public class PollutionInfoDisplay : InfoDisplay
{
    public override bool Active()
    {
        return Main.LocalPlayer.GetModPlayer<InfoDisplayPlayer>().PollutionMeter;
    }

    private float pollution;

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
    {
        pollution = MathHelper.Lerp(pollution, TileCounts.Instance.PollutionLevel, 0.1f);
        string text = $"{(pollution / TileCounts.Instance.PollutionLevelMax) * 100f:F0}";
        displayColor = Color.White;
        return text;
    }
}
