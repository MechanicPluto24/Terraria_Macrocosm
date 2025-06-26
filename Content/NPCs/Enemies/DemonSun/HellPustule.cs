using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.DemonSun
{
    public class HellPustule : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPC.ApplyBuffImmunity
            (
                BuffID.Bleeding,
                BuffID.BloodButcherer,
                BuffID.Poisoned,
                BuffID.Venom
            );

            NPCSets.Material[Type] = NPCMaterial.Slime;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Water);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Slime);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 22;
            NPC.height = 18;
            NPC.damage = 90;
            NPC.defense = 1;
            NPC.lifeMax = 600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.01f;
            NPC.aiStyle = 1;
            NPC.alpha = 10;
            NPC.npcSlots=0f;
            AIType = NPCID.BlueSlime;
            SpawnModBiomes = [ModContent.GetInstance<DemonSunBiome>().Type];
        }
        public override void FindFrame(int frameHeight)
        {
            int frameSpeed = 8;

            NPC.frameCounter++;

            if (NPC.frameCounter >= frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0f;

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }
        bool Spawned=false;
        public override void OnKill() {
			
            DeathEffects();
    
		}
        public void DeathEffects()
        {
            for (int i = 0; i < 30; i++)
                {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
        }
        public override bool PreAI()
        {
            NPC.life-=3;
            if(!Spawned)
            {
                Spawned=true;
                NPC.velocity.Y=Main.rand.NextFloat(-10f,0f);
                NPC.velocity.X=Main.rand.NextFloat(-10f,10);
            }
            if(Main.rand.NextBool(10)){
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f  + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            
        }


    }
}