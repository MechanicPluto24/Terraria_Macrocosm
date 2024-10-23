using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Minions;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{

    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    public class RyuguMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 1;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }
        public sealed override void SetDefaults()
        {
            Projectile.width = 49;
            Projectile.height = 42;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
          
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        bool foundTarget;
        bool spawned;
        float mult=0f;
        int shootTimer=0;
        NPC targetNPC;
        float distanceFromTarget;
        Vector2 targetCenter;
        public override void AI()
        {
            if(!spawned){
                spawned=true;
                mult=Main.rand.NextFloat(0.5f,1.5f);
            }
            Player owner = Main.player[Projectile.owner];
            
            if (!CheckActive(owner))
            {
                return;
            }
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            targetNPC= npc;
                            foundTarget = true;
                        }
                    }
                }
            }
            if (foundTarget)
                Utility.AIMinionFighter(Projectile, ref Projectile.ai, owner, false, 14, 14, 120, 4000, 6000, 0.04f*mult, (int)(3*mult), 14, (proj, owner) => { return targetNPC;});
            else
                Utility.AIMinionFighter(Projectile, ref Projectile.ai, owner, false, 14, 14, 120, 4000, 6000, 0.1f*mult, (int)(8*mult), 14, (proj, owner) => {return owner;});


            if (foundTarget){
                shootTimer++;
                if(shootTimer>300){
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, (targetNPC.Center - Projectile.Center).RotatedByRandom(MathHelper.Pi / 13).SafeNormalize(Vector2.UnitX) * 16f, ModContent.ProjectileType<RyuguShell>(), (int)(Projectile.damage), 1f, Main.myPlayer, 1f);
                    shootTimer=0;
                }
            }

            Visuals();
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<RyuguSummonBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<RyuguSummonBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            
            
            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
        }

     
        

    }
}
