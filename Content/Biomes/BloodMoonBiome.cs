using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
	public class BloodMoonBiome : ModBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/BloodMoon";
        public override string BackgroundPath => "Macrocosm/Content/Biomes/MoonBiome_Background";
        public override string MapBackground => BackgroundPath;

        //public override Color? BackgroundColor => base.BackgroundColor;
        //public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBgStyle>();
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();
        //public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem");

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
			=> SubworldSystem.IsActive<Moon>() && Main.bloodMoon;

	}
}
