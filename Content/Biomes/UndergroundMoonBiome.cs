using Macrocosm.Common.Utils;
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
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		//public override string BestiaryIcon => "Macrocosm/Assets/Textures/Icons/Moon";
		//public override string BackgroundPath => "Macrocosm/Assets/Textures/MapBackgrounds/Moon";
		//public override string MapBackground => BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBgStyle>();
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Stygia");

		public override void SetStaticDefaults()
		{
		}

		public override void OnInBiome(Player player)
		{
			base.OnInBiome(player);
			player.Macrocosm().ZoneUndergroundMoon = true;
		}

		public override void OnLeave(Player player)
		{
			base.OnLeave(player);
			player.Macrocosm().ZoneUndergroundMoon = false;
		}

		public override bool IsBiomeActive(Player player) 
			=> SubworldSystem.IsActive<Moon>() && player.ZoneDirtLayerHeight && player.ZoneRockLayerHeight;

	}
}
