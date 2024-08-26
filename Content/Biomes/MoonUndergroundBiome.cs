using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class MoonUndergroundBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => base.Priority + 1;

        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUndergroundBackgroundStyle>();
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Stygia");

        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/MoonUnderground";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon"; // Needs proper background
        public override string MapBackground => BackgroundPath;

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player) => base.IsBiomeActive(player) && (player.position.Y / 16 > Main.rockLayer);

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
