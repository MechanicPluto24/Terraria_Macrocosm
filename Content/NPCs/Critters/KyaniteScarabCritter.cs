using Macrocosm.Common.Sets;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Critters
{
    public class KyaniteScarabCritter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            Main.npcCatchable[Type] = true;

            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.TownCritter[Type] = true;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = false;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            if (attacker.type == ModContent.NPCType<KyaniteScarabSmall>())
                return false;
            return true;
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 16;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.npcSlots = 0.1f;
            NPC.aiStyle = NPCAIStyleID.CritterWorm;
            NPC.catchItem = ModContent.ItemType<Items.Critters.KyaniteScarabCritter>();

            AIType = NPCID.Buggy;
            SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
        }

        public override bool PreAI()
        {
            NPC.spriteDirection = NPC.direction;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 8;
            NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame) * frameHeight;

            if (NPC.frameCounter++ >= (ticksPerFrame * Main.npcFrameCount[Type]) - 1)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteCritterGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteCritterGore2").Type);
            }
        }
    }
}
