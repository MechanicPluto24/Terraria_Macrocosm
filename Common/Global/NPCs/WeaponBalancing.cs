using Macrocosm.Content.Biomes;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{

    public class WeaponBalancing : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //Apply always for our Evil superbosses.
            if (projectile.type==ProjectileID.FinalFractal)
                    modifiers.FinalDamage*=0.16f;//Zenith is much worse now! I'm going to be honest...  I never like the weapon anyway.
                if (projectile.type==ProjectileID.Meowmere)
                    modifiers.FinalDamage*=0.60f;
                if (projectile.type==ProjectileID.StarWrath)
                    modifiers.FinalDamage*=0.5f;
                if (projectile.type==ProjectileID.Celeb2Rocket||projectile.type==ProjectileID.Celeb2RocketExplosive||projectile.type==ProjectileID.Celeb2RocketLarge||projectile.type==ProjectileID.Celeb2RocketExplosiveLarge)
                    modifiers.FinalDamage*=0.6f;//Nerfed
                if (projectile.type==ProjectileID.LastPrismLaser)
                    modifiers.FinalDamage*=0.28f;//Hit very hard. Why are ML weapons Op?
                if (projectile.type==ProjectileID.LunarFlare)
                    modifiers.FinalDamage*=0.96f;//nerfed.
                if (projectile.type==ProjectileID.StardustDragon1||projectile.type==ProjectileID.StardustDragon2||projectile.type==ProjectileID.StardustDragon3||projectile.type==ProjectileID.StardustDragon4)
                    modifiers.FinalDamage*=0.5f;//Nerfed
                if (projectile.type==ProjectileID.EmpressBlade)
                    modifiers.FinalDamage*=1.01f;//buffed slightly
        }
    }
}