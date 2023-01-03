using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds;

namespace Macrocosm.Common.Global.GlobalItems
{
	public class GravityGlobalItem : GlobalItem
	{
		private const float defaultGravity = 0.1f;
		private const float defaultmaxFallSpeed = 7;

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
			{
				// doubled them so it feels closer to the gravity of the rest of entities
				gravity = defaultGravity * 2f * MacrocosmSubworld.Current().GravityMultiplier;
				maxFallSpeed = defaultmaxFallSpeed * 2f * MacrocosmSubworld.Current().GravityMultiplier;
			}
		}
	}
}
