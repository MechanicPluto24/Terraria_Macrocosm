using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds;
using Macrocosm.Common.Subworlds;

namespace Macrocosm.Common.Global.GlobalItems
{
	public class GravityGlobalItem : GlobalItem
	{
		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			// - doubled them so it feels closer to the gravity of the rest of entities
			// - used constants because gravity and maxFallSpeed are not reset to default values
			// - check for our subworlds in order to minimize the effect of this^ on other mods
			if (MacrocosmSubworld.AnyActive)
			{
				gravity = Earth.ItemGravity * 2f * MacrocosmSubworld.Current.GravityMultiplier;
				maxFallSpeed = Earth.ItemMaxFallSpeed * 2f * MacrocosmSubworld.Current.GravityMultiplier;
			}
		}
	}
}
