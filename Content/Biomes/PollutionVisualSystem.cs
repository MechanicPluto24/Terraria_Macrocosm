using Macrocosm.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class PollutionVisualSystem : ModSystem
{
    private static readonly Color[] SkyPalette =
    [
        new(198, 239, 126),
        new(211, 211, 82),
        new(207, 166, 49),
        new(194, 45, 3),
        new(24, 0, 63)
    ];

    public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
    {
        if (Main.gameMenu || Main.LocalPlayer is null || !Main.LocalPlayer.active)
            return;

        if (!Main.LocalPlayer.InModBiome<PollutionBiome>())
            return;

        float pollutionLevel = TileCounts.Instance.PollutionLevel;
        float intensity = MathHelper.Clamp(TileCounts.Instance.Pollution01, 0f, 1f);
        Color skyTint = SamplePollutionPalette(SkyPalette, pollutionLevel);

        backgroundColor = Color.Lerp(backgroundColor, skyTint, intensity);
        tileColor = Color.Lerp(tileColor, Color.Lerp(skyTint, Color.White, 0.35f), intensity * 0.35f);
    }

    private static Color SamplePollutionPalette(Color[] palette, float pollutionLevel)
    {
        float scaled = (pollutionLevel - TileCounts.PollutionLevelThreshold) / 100f;
        if (scaled <= 0f)
            return palette[0];

        if (scaled >= palette.Length - 1)
            return palette[^1];

        int index = (int)scaled;

        return Color.Lerp(palette[index], palette[index + 1], scaled - index);
    }
}
