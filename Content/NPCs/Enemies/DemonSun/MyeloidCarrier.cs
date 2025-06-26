using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Environment.Debris;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Common.Drawing.Particles;

namespace Macrocosm.Content.NPCs.Enemies.DemonSun
{
    public class MyeloidCarrier : ModNPC
    {

        public ref float AI_State => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPC.despawnEncouraged = false;

            SpawnModBiomes = [ModContent.GetInstance<DemonSunBiome>().Type];

            NPC.ApplyBuffImmunity
            (
                BuffID.Confused
            );

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DemonSunNPC[Type] = true;

            NPCSets.Material[Type] = NPCMaterial.Organic;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Humanoid);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 56;
            NPC.height = 76;
            NPC.damage = 200;
            NPC.defense = 200;
            NPC.lifeMax = 1400;
          
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome<DemonSunBiome>() ? .3f : 0f;

        public override void AI()
        {
            NPC.rotation+=0.01f;
            float Dist = Utility.CastLength(NPC.Center, new Vector2(0, 1), 60f, false);
            if(Dist < 60f)
            {
                DeathEffects();
                NPC.active=false;
            }

                
        }

        bool Spawned=false;
        public override bool PreAI()
        {
            if(!Spawned)
            {
                Spawned=true;
                NPC.position.Y-=1000;
                NPC.velocity.Y=20f;
                NPC.velocity.X=Main.rand.NextFloat(-1f,1f);
            }
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }
        public override void OnKill() {
			
            DeathEffects();
    
		}
        public void DeathEffects()
        {
            var entitySource = NPC.GetSource_Death();

               
            for (int i = 0; i < 10; i++)
            {
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-15, 16), (int)NPC.Center.Y + Main.rand.Next(-5, 6), ModContent.NPCType<HellPustule>(), 0, 0f);
            }

            for (int i = 0; i < 3; i++)
            {
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-15, 16), (int)NPC.Center.Y + Main.rand.Next(-5, 6), ModContent.NPCType<Myeloid>(), 0, 0f);
            }
            int impactDustCount = Main.rand.Next(140, 160);
            Vector2 impactDustSpeed = new Vector2(3f, 10f);
            float dustScaleMin = 1f;
            float dustScaleMax = 1.6f;

            int debrisType = ModContent.ProjectileType<RegolithDebris>();
            int debrisCount = Main.rand.Next(6, 8);
            Vector2 debrisVelocity = new Vector2(0.5f, 0.8f);

            for (int i = 0; i < impactDustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.Blood,
                    Main.rand.NextFloat(-impactDustSpeed.X, impactDustSpeed.X),
                    Main.rand.NextFloat(0f, -impactDustSpeed.Y),
                    Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                );

                dust.noGravity = false;
            }

            var explosion = Particle.Create<TintableExplosion>(p =>
            {
                p.Position = NPC.Center;
                p.Color = (new Color(200, 140, 140) * Lighting.GetColor(NPC.Center.ToTileCoordinates()).GetBrightness()).WithOpacity(0.8f);
                p.Scale = new(1.7f);
                p.NumberOfInnerReplicas = 12;
                p.ReplicaScalingFactor = 0.4f;
            });
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }
}
