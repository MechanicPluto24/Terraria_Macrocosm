using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon.MoonLich
{
    public class LunarChimera : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        /* 
        Old more precise Equation was :
        i*(x(Realtive to inital firing point not world pos))+j*i*sin((x*pi)/j)
        and i*(x(Realtive to inital firing point not world pos))-j*i*sin((x*pi)/j) if the player was to the left of the npc.

        i is the x postion of the target - npc position.

        j is y postion of the target/i
        But really just take the derivative of these to be of any use.
        */

        public override void SetDefaults()
        {
            NPC.width = 52;
            NPC.height = 50;
            NPC.damage = 100;
            NPC.defense = 100;
            NPC.lifeMax = 1500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
        }

        int Timer = 0;

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.1f;
            Utility.AIZombie(NPC, ref NPC.ai, false, true, velMax: 1, maxJumpTilesX: 8, maxJumpTilesY: 5, moveInterval: 0.02f);
            Timer++;
            if (Timer > 300)
            {
                for (int i = 0; i < 10; i++)//barf
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.Pi / 4) * (float)Main.rand.NextFloat(5.0f, 10.0f), ModContent.ProjectileType<LunarBarf>(), 50, 0f, -1);
                }

                Timer = 0;
            }

        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustType = Utils.SelectRandom<int>(Main.rand, ModContent.DustType<GreenBrightDust>(), DustID.Blood);

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
            }
        }
    }
}
