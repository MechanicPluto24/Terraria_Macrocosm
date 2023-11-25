using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Global
{
    public class GravityGlobalItem : GlobalItem
    {
        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            // - doubled them so it feels closer to the gravity of the rest of entities
            // - used constants because gravity and maxFallSpeed are not reset to default values
            // - check for our subworlds in order to minimize the impact of this^ on other mods
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                gravity = Earth.ItemGravity * 2f * MacrocosmSubworld.Current.GravityMultiplier;
                maxFallSpeed = Earth.ItemMaxFallSpeed * 2f * MacrocosmSubworld.Current.GravityMultiplier;
            }
        }
    }
}
