using Macrocosm.Common.Bases.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class ComplexBuffPlayer : ModPlayer
    {
        public override bool CanUseItem(Item item)
        {
            foreach (var buff in ComplexBuff.GetActive(Player))
                if (!buff.CanUseItem(Player, item))
                    return false;

            return base.CanUseItem(item);
        }

        public override void UpdateLifeRegen()
        {
            foreach (var buff in ComplexBuff.GetActive(Player))
                buff.UpdateLifeRegen(Player);
        }

        public override void UpdateBadLifeRegen()
        {
            foreach (var buff in ComplexBuff.GetActive(Player))
                buff.UpdateBadLifeRegen(Player);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            foreach (var buff in ComplexBuff.GetActive(Player))
                buff.DrawEffects(Player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
    }
}
