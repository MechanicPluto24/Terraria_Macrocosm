using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Clouds.Pollution
{
	// This class showcases advanced usage of a modded cloud. A ModCloud class is only needed for clouds with custom logic.
	// Typical clouds can be autoloaded automatically from any "Clouds" folder or manually loaded via CloudLoader.AddCloudFromTexture method for greater control.
	public abstract class BasePollutionCloud : ModCloud
    {
		public override bool RareCloud => false;

		public override float SpawnChance() {
			
			if (!Main.gameMenu && Main.LocalPlayer.InModBiome<PollutionBiome>()) {
				return 25f;
			}

			return 0f;
		}

		public override void OnSpawn(Cloud cloud) {
		}
		int despawnTimer=0;
		public override bool Draw(SpriteBatch spriteBatch, Cloud cloud, int cloudIndex, ref DrawData drawData) {
			if(!Main.LocalPlayer.InModBiome<PollutionBiome>())
			{
				despawnTimer++;
				drawData.color*=0.9f;
				if(despawnTimer>60)
					cloud.active=false;
			}
			else
				despawnTimer=0;
			return true;
		}
	}
	public class PollutionCloud1 : BasePollutionCloud
    {
	}
	public class PollutionCloud2 : BasePollutionCloud
    {
	}
	public class PollutionCloud3 : BasePollutionCloud
    {
	}
	public class PollutionCloud4 : BasePollutionCloud
    {
	}
	public class PollutionCloud5 : BasePollutionCloud
    {
	}
	public class PollutionCloud6 : BasePollutionCloud
    {
	}
	public class PollutionCloud7 : BasePollutionCloud
    {
	}
	public class PollutionCloud8 : BasePollutionCloud
    {
	}
}
