using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Projectiles.Friendly.Misc;
using Terraria.DataStructures;
using Macrocosm.Common.Utils;
using Terraria.ID;
using Macrocosm.Content.Projectiles.Friendly.Misc.BoosterPotion;

namespace Macrocosm.Common.Players
{
    public class BoosterPotionPlayer : ModPlayer
    {
        public bool Booster { get; set; }

        public override void ResetEffects()
        {
            Booster = false;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if(Main.rand.NextBool(5)&&Booster&&proj.DamageType==DamageClass.Magic){
                Projectile.NewProjectile(new EntitySource_Misc("Booster"), target.Center, Vector2.Zero, ModContent.ProjectileType<BoosterPotionBooster>(), 0, 0, Main.myPlayer);
            }
        }
        
    }
}
