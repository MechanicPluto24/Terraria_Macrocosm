using Macrocosm.Content.Projectiles.Friendly.Buff;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class BoosterPlayer : ModPlayer
    {
        public bool Booster { get; set; }

        public override void ResetEffects()
        {
            Booster = false;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.NextBool(5) && Booster && proj.DamageType == DamageClass.Magic)
            {
                Projectile.NewProjectile(new EntitySource_Misc("Booster"), target.Center, Vector2.Zero, ModContent.ProjectileType<MageBoosterProjectile>(), 0, 0, Main.myPlayer);
            }
        }

    }
}
