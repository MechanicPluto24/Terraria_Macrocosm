using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Subworlds;
using MonoMod.Cil;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class GoreGravityIL : ILoadable
    {
        public void Load(Mod mod)
        {
            Terraria.IL_Gore.Update += Gore_Update;
        }

        public void Unload()
        {
            Terraria.IL_Gore.Update -= Gore_Update;
        }

        private static void Gore_Update(ILContext il)
        {
            var c = new ILCursor(il);

            // matches "if (type < 411 || type > 430)" (general gores)
            if (!c.TryGotoNext(
                i => i.MatchLdfld<Gore>("type"),
                i => i.MatchLdcI4(430)
                ))
            {
                Macrocosm.Instance.Logger.Error("Failed to inject ILHook: GoreGravityIL");
            }

            // matches "velocity.Y += 0.2f" ... this might break if other mods alter gore gravity 
            if (!c.TryGotoNext(i => i.MatchLdcR4(0.2f))) return;

            c.Remove();
            c.EmitDelegate(GetGoreGravity);
        }

        // replace gravity increment with desired value 
        private static float GetGoreGravity()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                return Earth.GoreGravity * MacrocosmSubworld.Current.GravityMultiplier;

            return Earth.GoreGravity;
        }
    }
}