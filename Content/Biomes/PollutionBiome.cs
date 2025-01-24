using Macrocosm.Common.Systems;
using Macrocosm.Content.Liquids.WaterStyles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Capture;
using SubworldLibrary;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class PollutionBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
        public override float GetWeight(Player player) => TileCounts.Instance.PollutionLevel / TileCounts.Instance.MaxPollutionLevel;

        public override string BestiaryIcon => Macrocosm.TexturesPath +"Icons/Pollution";
        public override string BackgroundPath => Macrocosm.TexturesPath +"MapBackgrounds/Pollution";
        public override string MapBackground => BackgroundPath;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutionWaterStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        public override bool IsBiomeActive(Player player) => TileCounts.Instance.EnoughLevelForPollution&&!SubworldSystem.AnyActive<Macrocosm>();

        private float visualIntensity = 0f;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            float level = MathHelper.Clamp(TileCounts.Instance.PollutionLevel / TileCounts.Instance.MaxPollutionLevel, 0, 1);
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

            if (visualIntensity > 0f)
            {
                if (!Filters.Scene["Macrocosm:Graveyard"].IsActive())
                {
                    Filters.Scene.Activate("Macrocosm:Graveyard", default);
                }
                else
                {
                    Filters.Scene["Macrocosm:Graveyard"].GetShader().UseTargetPosition(player.Center);
                    float progress = MathHelper.Lerp(0f, 0.75f, visualIntensity);
                    Filters.Scene["Macrocosm:Graveyard"].GetShader().UseProgress(progress);
                    Filters.Scene["Macrocosm:Graveyard"].GetShader().UseIntensity(1.2f);
                }
            }
            else if (Filters.Scene["Macrocosm:Graveyard"].IsActive())
            {
                Filters.Scene.Deactivate("Macrocosm:Graveyard");
            }
        }
    }
}
