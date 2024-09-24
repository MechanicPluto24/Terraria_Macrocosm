using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Hermites
{
    public class Hermite : ModNPC
    {
        private float alert = 0f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 21;
            NPC.height = 32;
            NPC.damage = 65;
            NPC.defense = 100;
            NPC.lifeMax = 1300;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.scale = 3f;//I had to make them A little larger, remove this when we have offical sprites.
        }

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];

            if (Vector2.Distance(NPC.Center, player.Center) < 200f)
                alert += 0.01f;

            if (alert > 0.6f)
            {
                Utility.AIZombie(NPC, ref NPC.ai, fleeWhenDay: false, allowBoredom: false, velMax: 1, maxJumpTilesX: 5, maxJumpTilesY: 3, moveInterval: 0.03f);
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.dontTakeDamage = true;
            }
            //This was easy... I may add them hiding again when the player is out of range but I'll wait until we have the sprites.
        }

        public override void FindFrame(int frameHeight)
        {
            if (alert > 0.6f)
                NPC.frame.Y = 0 * frameHeight;
            else
                NPC.frame.Y = 1 * frameHeight;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.SpawnTileType == ModContent.TileType<Tiles.Blocks.Terrain.Regolith>() && Main.dayTime ? 0.08f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Regolith>(), 2));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}