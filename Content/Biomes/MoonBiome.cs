using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.WorldBuilding;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Macrocosm.Backgrounds;
using Macrocosm.Content.Subworlds.Moon;

namespace Macrocosm.Content.Biomes
{
    public class MoonBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "Assets/FilterIcons/Moon";
        public override string BackgroundPath => "Assets/Map/Moon";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBgStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();

        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MoonDay") : MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MoonNight");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Moon");
        }
 
        public override void OnInBiome(Player player)
        {
            Main.windSpeedCurrent = 0;
            player.GetModPlayer<MacrocosmPlayer>().ZoneMoon = true;
        }

        public override void OnLeave(Player player)
        {
            player.GetModPlayer<MacrocosmPlayer>().ZoneMoon = false;
        }
        
        public override bool IsBiomeActive(Player player)
        {
            return SubworldSystem.IsActive<Moon>();
        }
    }
}
