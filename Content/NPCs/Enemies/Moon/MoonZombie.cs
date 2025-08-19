using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Sounds;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon;

public class MoonZombie : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 9;

        NPCSets.MoonNPC[Type] = true;
        
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Undead);
        Redemption.AddNPCToElementList(Type, Redemption.NPCType.Humanoid);
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 44;
        NPC.damage = 65;
        NPC.defense = 40;
        NPC.lifeMax = 2000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SFX.ZombieDeath;
        NPC.knockBackResist = 0.5f;
        NPC.aiStyle = 3;
        AIType = NPCID.ZombieMushroom;
        SpawnModBiomes = [ModContent.GetInstance<MoonNightBiome>().Type];
        Banner = Item.NPCtoBanner(NPCID.Zombie);
        BannerItem = Item.BannerToItem(Banner);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.SpawnTileY < Main.rockLayer && !Main.dayTime ? 0.1f : 0f;

    public override bool PreAI()
    {
        if (NPC.velocity.Y < 0f)
            NPC.velocity.Y += 0.1f;

        return true;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        if (NPC.velocity.Y == 0)
        {
            NPC.frameCounter += 10;
            if (NPC.frameCounter >= 48)
            {
                NPC.frameCounter -= 48;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 304)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        else
        {
            NPC.frame.Y = 8 * frameHeight;
        }
    }

    public override void ModifyNPCLoot(NPCLoot loot)
    {
        loot.Add(ItemDropRule.Common(ModContent.ItemType<RocketFuelCanister>(), 10, 1, 4));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustType = Utils.SelectRandom<int>(Main.rand, ModContent.DustType<RegolithDust>(), DustID.Blood);

                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
                dust.velocity.X *= (dust.velocity.X + +Main.rand.Next(0, 100) * 0.015f) * hit.HitDirection;
                dust.velocity.Y = 3f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                dust.noGravity = true;
            }
        }

        if (Main.dedServ)
            return; // don't run on the server

        if (NPC.life <= 0)
        {
            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieHead").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieArm").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieArm").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieLeg").Type);
            Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieLeg").Type);
        }
    }
}
