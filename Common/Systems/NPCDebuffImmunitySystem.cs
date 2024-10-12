using Macrocosm.Common.Sets;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class NPCDebuffImmunitySystem : ModSystem
    {
        public override void PostSetupContent()
        {
            for (int type = 0; type < NPCLoader.NPCCount; type++)
            {
                if (NPCSets.MoonNPC[type])
                {
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.OnFire] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.CursedInferno] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Frostburn] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Confused] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Poisoned] = true;
                }
            }
        }
    }
}
