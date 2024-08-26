using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Global.NPCs
{
   
    public class WeaponBalancing : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers){
            if (!Main.LocalPlayer.InModBiome<EarthBiome>())//Only applies if not on earth because idk.
            {
                if (projectile.type==ProjectileID.FinalFractal)
                    modifiers.FinalDamage*=0.17f;//Zenith is much worse now! I'm going to be honest...  I never like the weapon anyway.
                if (projectile.type==ProjectileID.Meowmere)
                    modifiers.FinalDamage*=0.4f;
                if (projectile.type==ProjectileID.StarWrath)
                    modifiers.FinalDamage*=0.4f;
                if (projectile.type==ProjectileID.LastPrismLaser)
                    modifiers.FinalDamage*=0.16f;//Hit very hard. Why are ML weapons Op?
                if (projectile.type==ProjectileID.LunarFlare)
                    modifiers.FinalDamage*=0.9f;//Nerfed
                if (projectile.type==ProjectileID.StardustDragon1||projectile.type==ProjectileID.StardustDragon2||projectile.type==ProjectileID.StardustDragon3||projectile.type==ProjectileID.StardustDragon4)
                    modifiers.FinalDamage*=0.5f;//Nerfed
                if (projectile.type==ProjectileID.EmpressBlade)
                    modifiers.FinalDamage*=0.8f;//Nerfed


            }

        }
    }
}