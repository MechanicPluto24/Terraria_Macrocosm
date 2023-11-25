using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class UndergroundMoonBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Stygia");


        public override string BestiaryIcon => "Macrocosm/Content/Biomes/MoonBiome_Icon";
        public override string BackgroundPath => "Macrocosm/Content/Biomes/MoonBiome_Background";
        public override string MapBackground => BackgroundPath;


        public override void SetStaticDefaults()
        {
        }

        public override void OnInBiome(Player player)
        {
            base.OnInBiome(player);
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
        }

        public override bool IsBiomeActive(Player player)
            => SubworldSystem.IsActive<Moon>() && (player.position.Y / 16 > Main.rockLayer);

    }
}
