using System;
using Terraria;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Macrocosm.Items.Currency;
using Macrocosm.Items.Materials;

namespace Macrocosm.NPCs.Unfriendly.Enemies
{
    public class Clavite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clavite");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.MeteorHead];
        }
        public override void SetDefaults()
        {
            npc.width = 60;
            npc.height = 60;
            npc.lifeMax = 2500;
            npc.damage = 60;
            npc.defense = 60;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = 60f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            animationType = NPCID.MeteorHead;
            banner = Item.NPCtoBanner(NPCID.MeteorHead);
            bannerItem = Item.BannerToItem(banner);
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
            }

            Move(Vector2.Zero);
            bool playerActive = player != null && player.active && !player.dead;
            BaseAI.LookAt(playerActive ? player.Center : (npc.Center + npc.velocity), npc, 0);
        }
        public void Move(Vector2 offset, float speed = 3f, float turnResistance = 0.5f)
        {
            Player player = Main.player[npc.target];
            Vector2 moveTo = player.Center + offset; // Gets the point that the npc will be moving to.
            Vector2 move = moveTo - npc.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            npc.velocity = move;
        }

        private float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(mod.BuffType("SuitBreach"), 600, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.spawnTileType == ModContent.TileType<Tiles.Regolith>() ? .1f : 0f;
        }
        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicDust>());
            if (Main.rand.NextFloat() < .0625)
                Item.NewItem(npc.getRect(), ModContent.ItemType<ArtemiteOre>(), 1 + Main.rand.Next(5));
            if (Main.rand.NextFloat() < .0625)
                Item.NewItem(npc.getRect(), ModContent.ItemType<ChandriumOre>(), 1 + Main.rand.Next(5));
            if (Main.rand.NextFloat() < .0625)
                Item.NewItem(npc.getRect(), ModContent.ItemType<SeleniumOre>(), 1 + Main.rand.Next(5));
            if (Main.rand.NextFloat() < .0625)
                Item.NewItem(npc.getRect(), ModContent.ItemType<DianiteOre>(), 1 + Main.rand.Next(5));
            Item.NewItem(npc.getRect(), ModContent.ItemType<UnuCredit>(), 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustType = 1;
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}
