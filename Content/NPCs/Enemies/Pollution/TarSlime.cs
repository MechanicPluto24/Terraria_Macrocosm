using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Players;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Pollution
{
    public class TarSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPC.ApplyImmunity
            (
                BuffID.Bleeding,
                BuffID.BloodButcherer,
                BuffID.Poisoned,
                BuffID.Venom
            );
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 44;
            NPC.height = 30;
            NPC.damage = 10;
            NPC.defense = 2;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.alpha=10;
            AIType = -1;
            Banner = Item.NPCtoBanner(NPCID.BlueSlime);
            BannerItem = Item.BannerToItem(Banner);
            SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
        }
        

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<PollutionBiome>() ? 1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {

        }
        public override void FindFrame(int frameHeight)
        {
            int frameSpeed = 6;

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.frameCounter++ >= frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                if(NPC.velocity.Y>0)
                    NPC.frame.Y = 4 * frameHeight;
                if(NPC.velocity.Y<0)
                    NPC.frame.Y = 5 * frameHeight;
            }
        }

        public override void AI()
        { 
            Utility.AISlime(NPC, ref NPC.ai, false,false,100, 4f, -8f, 7f, -12f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if(NPC.life<1)
            {
                for(int i=0; i<4;i++)
                {
                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X+Main.rand.Next(-15,16), (int)NPC.Center.Y+Main.rand.Next(-5,6), ModContent.NPCType<TarBlob>(), 0, 0f); 
                }
            }
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }


    }
}