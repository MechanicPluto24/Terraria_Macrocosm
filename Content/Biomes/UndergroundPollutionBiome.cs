using Macrocosm.Common.Systems;
using Macrocosm.Content.Liquids.WaterStyles;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class UndergroundPollutionBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
    public override float GetWeight(Player player) => TileCounts.Instance.Pollution01;

    public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Pollution";
    public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Pollution";
    public override string MapBackground => BackgroundPath;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/UndergroundPollution");

    public override ModWaterStyle WaterStyle => PollutionWaterStyle.GetCurrentStyle();
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    public override bool IsBiomeActive(Player player) => !SubworldSystem.AnyActive<Macrocosm>() && (player.ZoneDirtLayerHeight||player.ZoneRockLayerHeight) && TileCounts.Instance.EnoughPollution;

    private float visualIntensity = 0f;
    public override void SpecialVisuals(Player player, bool isActive)
    {
        float level = MathHelper.Clamp(TileCounts.Instance.Pollution01, 0f, 1f);
        if (visualIntensity < level)
        {
            visualIntensity += 0.02f;
            if (visualIntensity > level)
                visualIntensity = level;
        }
        else if (visualIntensity > level)
        {
            visualIntensity -= 0.01f;
            if (visualIntensity < level)
                visualIntensity = level;
        }

        if (!isActive && visualIntensity > 0)
            visualIntensity -= 0.1f;

        Main.numClouds = (int)(200f * visualIntensity);

        if (visualIntensity > 0f)
        {
            if (!Filters.Scene["Macrocosm:Graveyard"].IsActive())
            {
                Filters.Scene.Activate("Macrocosm:Graveyard", default);
            }
            else
            {
                Filters.Scene["Macrocosm:Graveyard"].GetShader().UseTargetPosition(player.Center);
                float progress = MathHelper.Lerp(0f, 1.5f, visualIntensity);
                Filters.Scene["Macrocosm:Graveyard"].GetShader().UseProgress(progress);
                Filters.Scene["Macrocosm:Graveyard"].GetShader().UseIntensity(1.8f - 1.2f * visualIntensity);
            }
        }
        else if (Filters.Scene["Macrocosm:Graveyard"].IsActive())
        {
            Filters.Scene.Deactivate("Macrocosm:Graveyard");
        }
    }
}
