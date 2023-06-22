using Macrocosm.Common.Hooks;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Systems;
using Terraria;

namespace Macrocosm.Content.Biomes
{
	public class BasaltBiome : MoonBiome
	{
		//public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
		public override string BestiaryIcon => Macrocosm.EmptyTexPath;
		public override string BackgroundPath => Macrocosm.EmptyTexPath;
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
			player.Macrocosm().ZoneBasalt = true;
		}

		public override void OnLeave(Player player)
		{
			player.Macrocosm().ZoneBasalt = false;
		}

		public override bool IsBiomeActive(Player player)
			=> false; // TileCounts.Instance.BasaltCount > 40;
	
	}
}


