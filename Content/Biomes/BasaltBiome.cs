using Macrocosm.Common.Hooks;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Systems;
using Terraria;

namespace Macrocosm.Content.Biomes
{
	public class BasaltBiome : MoonBiome
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Basalt");
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
			=> TileCountSystem.TileCounts.RegolithCount > 40;
	
	}
}


