using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class MoonUndergroundBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override float GetWeight(Player player) => 0.7f;

        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUndergroundBackgroundStyle>();

        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/MoonUnderground";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/MoonUnderground"; 
        public override string MapBackground => BackgroundPath;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Stygia");

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player) => SubworldSystem.IsActive<Moon>() && (player.position.Y / 16 > Main.rockLayer);

        public override void OnInBiome(Player player)
        {
        }

        public override void OnEnter(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
}
