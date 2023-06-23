using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
	public class BasaltBiome : MoonBiome
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
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
			base.OnInBiome(player);
			player.Macrocosm().ZoneBasalt = true;
		}

		public override void OnLeave(Player player)
		{
			base.OnLeave(player);
			player.Macrocosm().ZoneBasalt = false;
		}

		public override bool IsBiomeActive(Player player)
			=> false; // TileCounts.Instance.BasaltCount > 40;
	
	}
}


