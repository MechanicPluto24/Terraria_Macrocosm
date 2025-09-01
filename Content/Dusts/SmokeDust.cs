using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    /// <summary>
    /// Using vanilla texture of DustID.Smoke
    /// </summary>
    public class SmokeDust : ModDust
    {
        public override string Texture => null;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Utility.VanillaDustFrame(DustID.Smoke);
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 0.99f;

            if (dust.scale <= 0.4f)
            {
                dust.active = false;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.05f * (dust.dustIndex % 2 == 0 ? -1 : 1);

            return false;
        }
    }
}