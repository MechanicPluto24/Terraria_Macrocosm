using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;

namespace Macrocosm.Content.Items.GlobalItems
{
	public class GravityGlobalItem : GlobalItem
	{
		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				// base values (0.1, 7) divided by 6a
				gravity = 0.0333f;
				maxFallSpeed = 2.333f;
			}
		}
	}
}
