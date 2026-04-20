using Macrocosm.Common.Players;
using Macrocosm.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info;

public class NephelometerInfoDisplay : InfoDisplay
{
    private static readonly Color NoPollutionColor = Color.Gray;
    private static readonly Color LightPollutionColor = new(181, 214, 103);
    private static readonly Color MediumPollutionColor = new(255, 205, 86);
    private static readonly Color HeavyPollutionColor = new(255, 128, 64);
    private static readonly Color ExtremePollutionColor = new(190, 53, 53);

    public override bool Active()
    {
        return Main.LocalPlayer.GetModPlayer<InfoDisplayPlayer>().Nephelometer;
    }

    private float pollution;

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
    {
        pollution = MathHelper.Lerp(pollution, TileCounts.Instance.PollutionLevel, 0.1f);
        int pollutionValue = (int)Math.Round(pollution);

        displayColor = GetPollutionColor(pollutionValue);
        displayShadowColor = Color.Black;
        return Language.GetText("Mods.Macrocosm.Machines.Common.AirQualityIndex").Format(pollutionValue);
    }

    private static Color GetPollutionColor(int pollutionValue)
    {
        return pollutionValue switch
        {
            <= 0 => NoPollutionColor,
            <= 100 => LightPollutionColor,
            <= 200 => MediumPollutionColor,
            <= 300 => HeavyPollutionColor,
            _ => ExtremePollutionColor,
        };
    }
}
